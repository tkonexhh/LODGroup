using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOD = CustomLODGroup.InutanLOD;
using LODGroup = CustomLODGroup.InutanLODGroup;

namespace CustomLODGroup
{
    //LOD 加载接口
    public interface ILODStreaming
    {
        void LoadAssetAsync(LOD lod, LODGroupBase lodGroup, int willLOD = -1);
        bool UnloadAsset(LOD lod);//资源卸载
    }
}
