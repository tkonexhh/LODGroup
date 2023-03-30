using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CustomLODGroup
{
    public class LODGroupManager
    {
        static LODGroupManager _Instance;
        public static LODGroupManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LODGroupManager();
                    RenderPipelineManager.beginFrameRendering -= _Instance.BeginCameraRendering;
                    RenderPipelineManager.beginFrameRendering += _Instance.BeginCameraRendering;
                }
                return _Instance;
            }
        }

        public ILODStreaming editorLODStreaming = new EditorLODStreaming();
        // public ILODStreaming runtimeLODStreaming = new PooledLODStreaming();

        public ILODStreaming lodStreaming
        {
            get
            {
                // if (Application.isPlaying)
                //     return runtimeLODStreaming;
                // else
                return editorLODStreaming;
            }
        }

        static float s_CullInterval = 0.1f;//计算间隔

        HashSet<LODGroupBase> m_AllLODGroup = new HashSet<LODGroupBase>();

        private JobValueMode m_JobValueMode;
        private JobValueView m_JobValueView;

        private bool m_Dirty = false;

        public bool Dirty { get => m_Dirty; set => m_Dirty = value; }//可以用于强制刷新
        public int totalCount => m_AllLODGroup.Count;

        private class CameraCullData
        {
            public float lastCullTime = -1;
            public Vector3 lastCameraPosition;
            public Quaternion lastCameraRotation;
            public float lastFOV;
            public float lastLODBias;
        }

        private Dictionary<Camera, CameraCullData> m_CullData = new Dictionary<Camera, CameraCullData>();

        public void AddLODGroup(LODGroupBase lodGroup)
        {
            bool result = m_AllLODGroup.Add(lodGroup);
            if (result)
                m_Dirty = true;
        }

        public bool RemoveLODGroup(LODGroupBase lodGroup)
        {
            bool result = m_AllLODGroup.Remove(lodGroup);
            if (result)
                m_Dirty = true;

            if (m_AllLODGroup.Count == 0)
            {
                m_JobValueView.OnDispose(ref m_JobValueMode);
            }
            return result;
        }

        static Unity.Profiling.ProfilerMarker p = new Unity.Profiling.ProfilerMarker("LODGroupCalulate");

        private void BeginCameraRendering(ScriptableRenderContext context, Camera[] cameras)
        {
            int count = m_AllLODGroup.Count;
            if (count == 0)
                return;

            foreach (var camera in cameras)
            {
                UpdateCamera(camera);
            }
        }

        private void UpdateCamera(Camera camera)
        {
            int count = m_AllLODGroup.Count;
            if (count == 0)
                return;

            bool cameraDirty = false;
            CameraCullData data;
            if (!m_CullData.TryGetValue(camera, out data))
            {
                data = new CameraCullData();
                m_CullData.Add(camera, data);
            }

            if (data.lastCullTime == -1)
            {
                //第一次进入 强制刷新
                cameraDirty = true;
            }
            else
            {
                // 刷新间隔没到，不做任何处理
                if (Application.isPlaying && data.lastCullTime + s_CullInterval > Time.realtimeSinceStartup)
                {
                    return;
                }
                //判断摄像机参数是否有变化
                //位置,旋转,FOV,LOD bias
                CheckDirty(camera, data, ref cameraDirty);
            }

            data.lastCullTime = Time.realtimeSinceStartup;


#if UNITY_EDITOR
            //没运行的时候实时刷新
            if (!Application.isPlaying)
                cameraDirty = true;

#endif
            if (!cameraDirty)
                return;

            if (m_Dirty)//LODGroup 发生变化
            {
                m_Dirty = false;
                m_JobValueView.Refesh(ref m_JobValueMode, ref m_AllLODGroup);
            }

            if (!m_JobValueMode.vaild)
                return;

            //只需要在相机Dirty的时候执行
            float preRelative;
            QuadTreeSpaceManager.SettingCamera(camera.orthographic, camera.orthographicSize, camera.fieldOfView, QualitySettings.lodBias, out preRelative);

            var job = new LODCalculateJob()
            {
                preRelative = preRelative,
                cameraPositionWS = camera.transform.position,
                centers = m_JobValueMode.centers,
                size = m_JobValueMode.sizes,
                lodRelatives = m_JobValueMode.lodRelative,
                openBuffer = m_JobValueMode.openBuffer,
                result = m_JobValueMode.result
            };
            JobHandle jobHandle = job.Schedule(count, 30);
            jobHandle.Complete();

            int i = 0;
            var result = m_JobValueMode.result;
            foreach (var item in m_AllLODGroup)
            {
                item.UpdataState(result[i++], camera.cameraType);
            }
        }

        private void CheckDirty(Camera camera, CameraCullData cullData, ref bool dirty)
        {
            var cameraPosition = camera.transform.position;
            if (cullData.lastCameraPosition != cameraPosition)
            {
                cullData.lastCameraPosition = cameraPosition;
                dirty = true;
                return;
            }

            var cameraRotation = camera.transform.rotation;
            if (cullData.lastCameraRotation != cameraRotation)
            {
                cullData.lastCameraRotation = cameraRotation;
                dirty = true;
                return;
            }

            if (cullData.lastFOV != camera.fieldOfView)
            {
                cullData.lastFOV = camera.fieldOfView;
                dirty = true;
                return;
            }

            if (cullData.lastLODBias != QualitySettings.lodBias)
            {
                cullData.lastLODBias = QualitySettings.lodBias;
                dirty = true;
            }
        }
    }
}
