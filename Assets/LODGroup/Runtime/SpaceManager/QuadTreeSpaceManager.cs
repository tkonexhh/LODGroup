using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace CustomLODGroup
{
    public static class QuadTreeSpaceManager
    {
        public static LODJobCalcResult SettingCameraJob(Vector3 center, float size, Vector3 cameraPosition, float preRelative)
        {
            LODJobCalcResult result = new LODJobCalcResult();
            result.distance = GetDistance(center, cameraPosition);
            result.relative = size * preRelative / result.distance;
            return result;
        }

        public static void SettingCamera(bool orthographic, float orthographicSize, float fieldOfView, float lodBias, out float preRelative)
        {
            if (orthographic)
            {
                preRelative = 0.5f / orthographicSize;
            }
            else
            {
                float halfAngle = Mathf.Tan(Mathf.Deg2Rad * fieldOfView * 0.5F);
                preRelative = 0.5f / halfAngle;
            }
            preRelative = preRelative * lodBias;
        }


        private static float GetDistance(Vector3 boundsPos, Vector3 camPos)
        {
            return (boundsPos - camPos).magnitude;
        }
    }
}
