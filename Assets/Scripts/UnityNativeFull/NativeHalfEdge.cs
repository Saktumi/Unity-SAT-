using System.Diagnostics;
using JetBrains.Annotations;

[DebuggerDisplay("NativeHalfEdge: Origin = {Origin}, Face = {Face},Twin = {Twin},[Prev{Prev} Next={Next}]")]
public struct NativeHalfEdge //物理上的边被拆成了两条方向相反的“半边” 例子:一个立方体有 12 条物理边。但在半边结构中，它会存储 24 条半边（每个面 4 条，6 个面共 24 条）。 当你站在立方体的一个面上（面A），你沿着 Next 走，会绕这个面一圈。 当你走到面A和面B的交界处时，你顺着当前半边的 Twin 指针跳过去，就会跳到面B的对应半边，然后你就可以沿着面B的 Next 继续绕面B走。
{
    public int Prev; //面环中前一个边的索引
    public int Next; //面环中下一个边的索引
    public int Twin ;//与此边相对的另一条边的索引(在不同的面环中) ，用于在相邻的两个面中穿梭
    public int Face;//该边所属面的索引
    public int Origin;//该边起点的顶点索引

}