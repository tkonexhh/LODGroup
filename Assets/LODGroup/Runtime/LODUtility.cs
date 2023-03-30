using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOD = CustomLODGroup.InutanLOD;
using LODGroup = CustomLODGroup.InutanLODGroup;

namespace CustomLODGroup
{

    public struct LODVisualizationInformation
    {
        public int triangleCount;
        public int vertexCount;
        public int rendererCount;
        public int submeshCount;

        public int activeLODLevel;
        public float activeLODFade;
        public float activeDistance;
        public float activeRelativeScreenSize;
        public float activePixelSize;
        public float worldSpaceSize;
    }

    public static class LODUtility
    {
        /// <summary>
        /// 根据百分比反算相机距离
        /// </summary>
        public static float CalculateDistance(Camera camera, float relativeScreenHeight, LODGroup group)
        {
            //DistanceToRelativeHeight 的逆运算
            var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
            return (group.size * 0.5F) / (relativeScreenHeight * halfAngle);
        }

        static float GetWorldSpaceScale(Transform t)
        {
            var scale = t.lossyScale;
            float largestAxis = Mathf.Abs(scale.x);
            largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.y));
            largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.z));
            return largestAxis;
        }

        static float GetWorldSpaceSize(LODGroup lodGroup)
        {
            return GetWorldSpaceScale(lodGroup.transform) * lodGroup.size;
        }

        static float DistanceToRelativeHeight(Camera camera, float distance, float size)
        {
            if (camera.orthographic)
                return size * 0.5F / camera.orthographicSize;

            var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
            var relativeHeight = size * 0.5F / (distance * halfAngle);
            return relativeHeight;
        }

        public static float GetRelativeHeight(this LODGroup lodGroup, Camera camera)
        {
            var distance = (lodGroup.transform.TransformPoint(lodGroup.localReferencePoint) - camera.transform.position).magnitude;
            // var distance = (lodGroup.transform.position - camera.transform.position).magnitude;
            return DistanceToRelativeHeight(camera, distance, GetWorldSpaceSize(lodGroup));
        }

        static int GetCurrentLOD(LOD[] lods, int maxLOD, float relativeHeight, Camera camera = null)
        {
            var lodIndex = -1;//默认为-1 culled

            for (var i = 0; i < lods.Length; i++)
            {
                var lod = lods[i];

                if (relativeHeight >= lod.screenRelativeHeight)
                {
                    lodIndex = i;
                    break;
                }
            }

            return lodIndex;
        }

        public static int GetCurrentLOD(LODGroup lodGroup, Camera camera = null)
        {
            var lods = lodGroup.GetLODs();
            var relativeHeight = lodGroup.GetRelativeHeight(camera ?? Camera.current);

            var lodIndex = GetCurrentLOD(lods, GetMaxLOD(lodGroup), relativeHeight, camera);

            return lodIndex;
        }

        public static int GetCurrentLODByDistance(LODGroup lodGroup, Camera camera = null)
        {
            return 0;
        }

        public static LODVisualizationInformation CalculateVisualizationData(Camera camera, LODGroup group, int lodLevel, LODGroupMode mode = LODGroupMode.screenRelativeHeight)
        {
            float size = group.size;
            float distance = Vector3.Distance(camera.transform.position, group.transform.position);

            float relativeHeight;
            if (camera.orthographic)
                relativeHeight = size * 0.5F / camera.orthographicSize;

            var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
            relativeHeight = size * 0.5F / (distance * halfAngle);

            LODVisualizationInformation info = new LODVisualizationInformation();
            info.activeRelativeScreenSize = relativeHeight;
            info.worldSpaceSize = GetWorldSpaceSize(group);//需要响应物体缩放
            if (mode == LODGroupMode.screenRelativeHeight)
                info.activeLODLevel = GetCurrentLOD(group, camera);
            else
                info.activeLODLevel = GetCurrentLODByDistance(group, camera);
            return info;
        }

        private static float GetDistance(Camera camera, LODGroup group)
        {
            return Vector3.Distance(camera.transform.position, group.transform.position);
        }

        public static Vector3 CalculateWorldReferencePoint(LODGroup group)
        {
            return group.transform.TransformPoint(group.localReferencePoint);
            // return group.transform.position + group.localReferencePoint;
        }

        //TODO 当前LODGroup是否需要刷新Bounds
        public static bool NeedUpdateLODGroupBoundingBox(LODGroup group)
        {
            return true;
        }

        public static int GetMaxLOD(LODGroup lodGroup)
        {
            return lodGroup.lodCount - 1;
        }
    }
}
