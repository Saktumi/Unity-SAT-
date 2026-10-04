using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;
using Unity.Collections;
using Common;
using Debug = UnityEngine.Debug;
using Unity.VisualScripting;
using UnityEditor;

[ExecuteInEditMode] //在编辑模式下执行

public class HullTester : MonoBehaviour
{
    public List<Transform> Transforms ;//要测试的节点列表

    [Header("可视化选项")]
    public bool DrawIsCollided ; //绘制碰撞状态
    public bool DrawIntersection ; //绘制相交区域

    [Header("控制台日志")]
    public bool LogContact ; //记录接触日志

    private Dictionary<int,TestShape> Hulls ;//凸包字典

    void Update()
    {
        HandleTransformChanged() ; //处理节点变化
        HandleHullCollisions() ; //处理凸包碰撞
    }
    
    //处理变换变化
    private void HandleTransformChanged()
    {
        //Transforms.ToList()仅为得到副本
        var transforms = Transforms.ToList().Distinct().Where(t => t.gameObject.activeSelf).ToList();
        var newTransformFound = false;
        var transformCount = 0;

        //过滤下有没有新的物体，看看要不要重建
    if (Hulls != null)
    {
    for (var i = 0; i < transforms.Count; i++)
    {
        var t = transforms[i];
        if (t == null)
            continue;

        transformCount++;

        var foundNewHull = !Hulls.ContainsKey(t.GetInstanceID());
        if (foundNewHull)
        {
            newTransformFound = true;
            break;
        }
    }

    if (!newTransformFound && transformCount == Hulls.Count)
        return;
    }
      //重建对象
      Debug.Log("重建对象");
      //安全地释放资源
      EnsureDestroyed();

      // 保存下不为空的节点，InstanceID作为key，创建的TestShape作为value记录在字典里，方便后续使用
      Hulls = transforms.Where(t => t != null).ToDictionary(k => k.GetInstanceID(), CreateShape);
      
      //强制编辑器重新绘制场景视图
      SceneView.RepaintAll();

    }

    //创建测试形状
    private TestShape CreateShape(Transform t)
    {
        var hull = CreateHull(t);
        return new TestShape
        {
            Id = t.GetInstanceID(),
            Hull = hull ,
        };
    }
    //根据变换创建凸体
   private NativeHull CreateHull(Transform v)
{
    var collider = v.GetComponent<Collider>();
    if (collider is MeshCollider meshCollider)
    {
        //return 一个NativeHull, 然后create用meshCollider的sharedMesh
    }
    var mf = v.GetComponent<MeshFilter>();
    if (mf != null && mf.sharedMesh != null)
    {
        //return 一个NativeHull, 然后create用meshCollider的sharedMesh
    }
    throw new InvalidOperationException($"无法从游戏对象 '{v?.name}' 创建凸包");
}
    //处理凸包碰撞检测
    private void HandleHullCollisions()
    {
        
    }
    
void OnDestroy() => EnsureDestroyed();
void OnDisable() => EnsureDestroyed();

// 确保资源被销毁
private void EnsureDestroyed()
{
    if (Hulls == null)
        return;

    foreach(var kvp in Hulls)
    {
        if (kvp.Value.Hull.IsValid)
        {
            kvp.Value.Hull.Dispose();
        }
    }

    Hulls.Clear();
}

}