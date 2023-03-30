using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomLODGroup
{
    //总共支持8级LOD 每个分量存储了各级的屏占比
    //如果自定义LODGroup不需要这么多级的话 可以适当减少分量
    public struct Float8
    {
        //最后两个参数是切换缓冲使用
        public float v0, v1, v2, v3, v4, v5, v6, v7;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return v0;
                    case 1: return v1;
                    case 2: return v2;
                    case 3: return v3;
                    case 4: return v4;
                    case 5: return v5;
                    case 6: return v6;
                    case 7: return v7;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: v0 = value; break;
                    case 1: v1 = value; break;
                    case 2: v2 = value; break;
                    case 3: v3 = value; break;
                    case 4: v4 = value; break;
                    case 5: v5 = value; break;
                    case 6: v6 = value; break;
                    case 7: v7 = value; break;
                }
            }
        }
    }


    public struct LODJobCalcResult
    {
        public float distance;
        public float relative;//当前屏占比
        public int lodLevel;
    }
}
