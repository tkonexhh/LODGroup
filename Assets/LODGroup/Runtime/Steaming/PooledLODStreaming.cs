// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace CustomLODGroup
// {
//     //运行时使用Yooasset才动态加载资源 但是Yooasset却是需要初始化的
//     public class PooledLODStreaming : ILODStreaming
//     {
//         public void LoadAssetAsync(InutanLOD lod, LODGroupBase lodGroup, int willLOD = -1)
//         {
//             if (!Application.isPlaying)
//                 return;
//             //这里的问题在于YooAsset可能没有初始化完成
//             lod.CurrentState = LODState.Loading;
//             ResManager.LoadAsync<GameObject>(lod.streamingPath, (prefab) =>
//             {
//                 lod.CurrentState = LODState.Loaded;
//                 lod.streamingResult = AssetManager.Instantiate(prefab, lodGroup.transform.position, lodGroup.transform.rotation, false, true);
//                 lod.streamingResult.transform.SetParent(lodGroup.transform);
//                 lodGroup.OnDisableCurrentLOD(willLOD);
//             });
//         }

//         public bool UnloadAsset(InutanLOD lod)
//         {
//             if (!Application.isPlaying)
//                 return false;

//             AssetManager.Destroy(lod.streamingResult);
//             return true;
//         }
//     }
// }
