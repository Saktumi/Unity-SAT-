using System.Diagnostics;
using Palmmedia.ReportGenerator.Core;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

[DebuggerDisplay("NativePlane:{Normal},{Offset}")] //显示NativeForce结构体对的Edge字段
public unsafe struct NativePlane //用于表示一个平面 点积代表“两个向量有多平行”（算出一个数字）；叉积代表“两个向量构成什么平面”（生成一个垂直的新向量）。N*P=d
{
  public float3 Normal ; //平面相对于原点的方向
  public float Offset ; //平面相对于原点的距离

  public NativePlane(float3 normal , float Offset)
    {
        Normal = normal;
        this.Offset = Offset ; //初始化平面的法向量和偏移量
    }
  
  public float Distance(float3 point) //计算某一点到平面的距离
    {
        return dot(Normal,point) - Offset; //从原点算的point去和法线做点乘,点到平面的垂直距离
    }
  public float3 ClosestPoint(float3 point) //计算某一点到平面的最近点
    {
        return point - Distance(point)*normalize(Normal); //位移向量
    }

  public static NativePlane operator *(RigidTransform t, NativePlane plane) 
    {
        float3 normal = mul(t.rot,plane.Normal); //把原始法线按照刚体变换的旋转部分进行旋转，得到新的法线方向
        return new NativePlane(normal,plane.Offset + dot(normal,t.pos));//返回平面的位置和法向量经过刚体变换后的平面，新偏移量 = 旧偏移量 + 新法向量与刚体平移部分的点积
    }

}
