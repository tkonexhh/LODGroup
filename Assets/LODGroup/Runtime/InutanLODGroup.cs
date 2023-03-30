using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOD = CustomLODGroup.InutanLOD;

namespace CustomLODGroup
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class InutanLODGroup : LODGroupBase
    {
        private void Start()
        {
#if UNITY_EDITOR
            foreach (var lod in GetLODs())
            {
                LODGroupManager.Instance.lodStreaming.UnloadAsset(lod);
                lod.CurrentState = LODState.None;
            }
#endif
        }

        public void Reset()
        {
            var lods = new LOD[2];
            lods[0] = new LOD(0.7f, new InutanLOD.Renderer[] { });
            lods[1] = new LOD(0.5f, new InutanLOD.Renderer[] { });
            SetLODs(lods);
        }
    }
}
