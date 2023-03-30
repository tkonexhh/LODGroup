using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomLODGroup
{
    public enum LODState
    {
        None,
        UnLoading,
        UnLoaded,
        Loading,
        Loaded,
        Failed
    }



    [Serializable]
    public class InutanLOD
    {
        public float distance;//距离
        public float screenRelativeHeight;//屏占比[0-1]

        [SerializeField]
        public InutanLOD.Renderer[] renderers;


        //当前状态
        [SerializeField]
        private LODState m_CurrentState;
        //上一帧状态
        [SerializeField]
        private LODState m_LastState;

        //流式加载
        public bool isStreaming;//是否是流式LOD
        public string streamingPath;//流式加载地址
        public GameObject streamingResult;

        //TODO GPU Instance支持



        public LODState CurrentState { get => m_CurrentState; set => m_CurrentState = value; }
        public LODState LastState { get => m_LastState; set => m_LastState = value; }

        public InutanLOD(float screenRelativeHeight, Renderer[] renderers)
        {
            this.distance = 0;
            this.screenRelativeHeight = screenRelativeHeight;
            this.renderers = renderers;
            isStreaming = false;
            streamingPath = "";
            m_CurrentState = LODState.None;
            m_LastState = LODState.None;
        }

        //返回true表示刚加载完成，否则返回false
        public bool SetState(bool active, LODGroupBase lodGroup, float distance, int willLOD = -1)
        {
            if (isStreaming)
            {
                return StreamingLOD.SetState(active, this, lodGroup, willLOD);
            }
            else
            {
                NormalLOD.SetState(active, this, lodGroup, willLOD);
                return true;
            }

        }

        /// <summary>
        /// 为了和Unity序列化保持一致 额外添加的一层
        /// unity自带的LODGroup 直接使用的Renderer[] 序列化的结果确实 - renderer : {fileID: 0}
        /// 如果我不包这一层的话 就是 - : {fileID: 0} 缺失一层 导致编辑器脚本大量报错
        /// </summary>
        [System.Serializable]
        public class Renderer
        {
            public UnityEngine.Renderer renderer;
        }
    }



}
