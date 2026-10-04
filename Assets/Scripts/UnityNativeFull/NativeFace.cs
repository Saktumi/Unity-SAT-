using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("NativeFace: Edge={Edge}")] //显示NativeForce结构体对的Edge字段
public struct NativeFace
{
  public int Edge ; /*该面上起始边的索引 :在 3D 中，一个面是由许多条边围成的环。只要找到了这个面上的任意一条边，顺着边的 Next 指针一路找下去，就能遍历这个面上所有的边和顶点。这样极大地节省了内存，也方便动态修改拓扑结构。*/

}
