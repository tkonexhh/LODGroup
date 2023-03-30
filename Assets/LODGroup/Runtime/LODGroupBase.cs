using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOD = CustomLODGroup.InutanLOD;

namespace CustomLODGroup
{
    public enum LODGroupMode
    {
        screenRelativeHeight,
        Distance,
    }


    public abstract class LODGroupBase : MonoBehaviour
    {
        [SerializeField]
        private LODFadeMode m_FadeMode;
        [SerializeField]
        private bool m_AnimateCrossFading;
        [SerializeField]
        private LODGroupMode m_Mode;
        [SerializeField]
        private float m_Size = 1;
        [SerializeField]
        private Vector3 m_LocalReferencePoint;//LOD的中心点
        [SerializeField]
        private LOD[] m_LODs;
        protected int m_CurrentLOD = -1;
        protected int m_LoadingLOD = -1;


        public Vector3 localReferencePoint { get => m_LocalReferencePoint; set => m_LocalReferencePoint = value; }
        public float size => m_Size;
        public int lodCount { get => m_LODs == null ? 0 : m_LODs.Length; }


        private void OnEnable()
        {
            LODGroupManager.Instance.AddLODGroup(this);
        }

        private void OnDisable()
        {
            LODGroupManager.Instance.RemoveLODGroup(this);
            //TODO 关闭组件后 强制指定LOD0
        }

        public void SetLODs(LOD[] lods)
        {
            if (lods != null && lods.Length > 0)
            {
                m_LODs = lods;
            }
        }

        public LOD[] GetLODs() => m_LODs;

        //从新计算包围盒
        public void RecalculateBounds()
        {
            List<Renderer> all = new List<Renderer>();
            foreach (var lod in m_LODs)
            {
                if (lod.renderers != null)
                {
                    foreach (var renderer in lod.renderers)
                    {
                        all.Add(renderer.renderer);
                    }
                }
            }
            UnityEngine.Bounds bounds;
            if (all.Count <= 0)
            {
                bounds = new UnityEngine.Bounds(Vector3.zero, Vector3.one);
            }
            else
            {
                bounds = all[0].bounds;
                for (int i = 1; i < all.Count; i++)
                {
                    bounds.Encapsulate(all[i].bounds);
                }
                //相对于当前节点的位置
                bounds.center = bounds.center - transform.position;
                var maxSize = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
                bounds.size = Vector3.one * maxSize;
            }

            m_LocalReferencePoint = bounds.center;
            m_Size = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
        }

        public void UpdataState(LODJobCalcResult calcResult, CameraType cameraType)
        {
            if (calcResult.lodLevel == m_CurrentLOD && calcResult.lodLevel == m_LoadingLOD)
                return;

#if UNITY_EDITOR
            //如果正在运行中 如果当前相机是scene相机的话
            if (Application.isPlaying && cameraType != CameraType.Game)
                return;
#endif

            //如果是cull状态
            if (calcResult.lodLevel == -1)
            {
                //当前还有正在加载的LOD
                if (m_LoadingLOD != -1)
                {
                    m_LODs[m_LoadingLOD].SetState(false, this, calcResult.distance);
                    m_LoadingLOD = -1;
                }

                //需要卸载当前LOD
                if (m_CurrentLOD != -1)
                {
                    m_LODs[m_CurrentLOD].SetState(false, this, calcResult.distance);
                    m_CurrentLOD = -1;
                }
                return;
            }

            var lod = m_LODs[calcResult.lodLevel];
            bool result = false;
            result = lod.SetState(true, this, calcResult.distance, calcResult.lodLevel);

            //如果当前还有在加载的LOD的话
            if (m_LoadingLOD != -1 && m_LoadingLOD != calcResult.lodLevel && m_LoadingLOD != m_CurrentLOD)
            {
                m_LODs[m_LoadingLOD].SetState(false, this, calcResult.distance);
            }
            m_LoadingLOD = calcResult.lodLevel;

        }


        public void OnDisableCurrentLOD(int willLOD = -1)
        {
            if (m_CurrentLOD != -1 && m_CurrentLOD != willLOD)
            {
                m_LODs[m_CurrentLOD].SetState(false, this, 0);
            }
            m_CurrentLOD = willLOD;
        }
    }
}
