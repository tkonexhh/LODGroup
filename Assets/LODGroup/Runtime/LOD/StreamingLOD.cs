using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOD = CustomLODGroup.InutanLOD;
using LODGroup = CustomLODGroup.InutanLODGroup;

namespace CustomLODGroup
{
    public static class StreamingLOD
    {
        public static bool SetState(bool active, LOD lod, LODGroupBase lodGroup, float distance, int willLOD = -1)
        {
            bool onceLoaded = false;
            switch (lod.CurrentState)
            {
                case LODState.None:
                case LODState.UnLoaded:
                    if (active == true)
                    {
                        LoadAsset(lod, lodGroup, distance, willLOD);
                    }
                    break;
                case LODState.Loading:
                    if (active == false)
                    {
                        UnLoaded(lod);
                    }
                    break;
                case LODState.Loaded:
                    if (active == false)
                    {
                        UnLoaded(lod);
                    }
                    else if (lod.LastState == LODState.Loading)
                    {
                        onceLoaded = true;
                    }
                    break;
            }
            lod.LastState = lod.CurrentState;
            return onceLoaded;
        }

        private static void LoadAsset(LOD lod, LODGroupBase lodGroup, float distance, int willLOD = -1)
        {
            if (string.IsNullOrEmpty(lod.streamingPath))
                return;

            lod.CurrentState = LODState.Loading;

            //编辑器下走直接加载
            //运行模式下需要走池

            LODGroupManager.Instance.lodStreaming.LoadAssetAsync(lod, lodGroup, willLOD);
        }

        public static void UnLoaded(LOD lod)
        {
            LODGroupManager.Instance.lodStreaming.UnloadAsset(lod);

            lod.CurrentState = LODState.UnLoaded;
            lod.streamingResult = null;
        }
    }
}
