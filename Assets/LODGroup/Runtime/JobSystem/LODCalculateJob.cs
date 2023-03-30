using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using LODGroup = CustomLODGroup.InutanLODGroup;

namespace CustomLODGroup
{
    [BurstCompile(CompileSynchronously = true)]
    public struct LODCalculateJob : IJobParallelFor
    {
        [ReadOnly]
        public float preRelative;
        [ReadOnly]
        public Vector3 cameraPositionWS;

        [ReadOnly]
        public NativeArray<Float8> lodRelatives;
        [ReadOnly]
        public NativeArray<Vector3> centers;
        [ReadOnly]
        public NativeArray<float> size;
        [ReadOnly]
        public bool openBuffer;   //是否开启切换lod缓冲

        //返回
        public NativeArray<LODJobCalcResult> result;

        public void Execute(int index)
        {
            LODJobCalcResult r = QuadTreeSpaceManager.SettingCameraJob(centers[index], size[index], cameraPositionWS, preRelative);

            var lastResult = result[index];
            //计算上一次的位置看看是否需要切换LOD
            Float8 f8 = lodRelatives[index];
            for (int i = 0; i < 8; i++)
            {
                float lodRelative = f8[i];

                if (lodRelative == 0 && i != 0)
                {
                    r.lodLevel = -1;
                    break;
                }
                if (openBuffer && lastResult.lodLevel == i)
                    lodRelative = lodRelative * 0.9f;
                if (r.relative > lodRelative)
                {
                    r.lodLevel = i;
                    break;
                }
            }
            result[index] = r;
        }
    }


    public struct JobValueMode
    {
        public NativeArray<Vector3> centers;
        public NativeArray<float> sizes;
        public NativeArray<Float8> lodRelative;
        public bool openBuffer;//
        public NativeArray<LODJobCalcResult> result;

        public bool vaild;
    }

    public struct JobValueView
    {
        public void Refesh(ref JobValueMode mode, ref HashSet<LODGroupBase> lodGroups)
        {
            if (mode.centers.IsCreated && mode.centers.Length > 0)
            {
                mode.centers.Dispose();
                mode.sizes.Dispose();
                mode.lodRelative.Dispose();
                mode.result.Dispose();
            }

            mode.centers = new NativeArray<Vector3>(lodGroups.Count, Allocator.Persistent);
            mode.sizes = new NativeArray<float>(lodGroups.Count, Allocator.Persistent);
            mode.lodRelative = new NativeArray<Float8>(lodGroups.Count, Allocator.Persistent);
            mode.result = new NativeArray<LODJobCalcResult>(lodGroups.Count, Allocator.Persistent);

#if UNITY_EDITOR
            mode.openBuffer = Application.isPlaying;
#else
            mode.openBuffer = true;
#endif

            int j = 0;
            foreach (var item in lodGroups)
            {
                mode.centers[j] = item.transform.position + item.localReferencePoint;
                mode.sizes[j] = item.size;

                var lods = item.GetLODs();
                int count = lods.Length;
                Float8 f8 = new Float8();
                for (int i = 0; i < count; i++)
                {
                    var v = lods[i].screenRelativeHeight;
                    f8[i] = v;
                }

                mode.lodRelative[j] = f8;

                j++;
            }

            mode.vaild = true;
        }

        public void OnDispose(ref JobValueMode mode)
        {
            mode.vaild = false;
            if (mode.centers.IsCreated) mode.centers.Dispose();
            if (mode.sizes.IsCreated) mode.sizes.Dispose();
            if (mode.lodRelative.IsCreated) mode.lodRelative.Dispose();
            if (mode.result.IsCreated) mode.result.Dispose();
        }
    }
}
