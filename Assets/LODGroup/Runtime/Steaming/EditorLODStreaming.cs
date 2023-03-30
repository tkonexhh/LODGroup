using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOD = CustomLODGroup.InutanLOD;
using LODGroup = CustomLODGroup.InutanLODGroup;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomLODGroup
{
    /// <summary>
    /// 编辑器模式使用流式LOD加载方式 只在非play模式下使用
    /// </summary>
    public class EditorLODStreaming : ILODStreaming
    {
        public void LoadAssetAsync(LOD lod, LODGroupBase lodGroup, int willLOD = -1)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;

            //编辑器模式直接使用AssetDataBase加载
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(lod.streamingPath);
            var target = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            target.transform.SetParent(lodGroup.transform);
            target.transform.localPosition = Vector3.zero;
            target.transform.localScale = Vector3.one;
            target.transform.localRotation = Quaternion.identity;
            lod.CurrentState = LODState.Loaded;
            lod.streamingResult = target;
            //加载完毕才OnDisableCurrentLOD
            lodGroup.OnDisableCurrentLOD(willLOD);
#endif
        }

        public bool UnloadAsset(LOD lod)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject.DestroyImmediate(lod.streamingResult);
                return true;
            }
#endif
            return false;
        }
    }
}
