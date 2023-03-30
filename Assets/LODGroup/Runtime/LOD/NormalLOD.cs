using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOD = CustomLODGroup.InutanLOD;
using LODGroup = CustomLODGroup.InutanLODGroup;

namespace CustomLODGroup
{
    public static class NormalLOD
    {
        public static void SetState(bool active, LOD lod, LODGroupBase lodGroup, int willLOD = -1)
        {
            switch (lod.CurrentState)
            {
                case LODState.None:
                case LODState.UnLoaded:
                case LODState.UnLoading:
                    if (active == true)
                    {
                        ChangeRendererState(active, lod);
                        lodGroup.OnDisableCurrentLOD(willLOD);
                    }
                    break;
                case LODState.Loaded:
                    if (active == false)
                    {
                        ChangeRendererState(active, lod);
                    }
                    break;
            }
        }

        public static void ChangeRendererState(bool state, LOD lod)
        {
            if (lod.renderers == null)
                return;
            var renderers = lod.renderers;
            var count = renderers.Length;
            for (int i = 0; i < count; i++)
            {
                var rd = renderers[i];
                //TODO 这里换成改变layer更好 自行选择吧
                if (rd != null)
                    rd.renderer.enabled = state;
            }
            // if (lod.Colliers != null)
            // {
            //     var colliders = lod.Colliers;
            //     count = renderers.Length;
            //     for (int i = 0; i < count; i++)
            //     {
            //         var c = colliders[i];
            //         if (c != null)
            //             c.enabled = state;
            //     }
            // }

            lod.CurrentState = state == true ? LODState.Loaded : LODState.UnLoaded;
        }
    }
}
