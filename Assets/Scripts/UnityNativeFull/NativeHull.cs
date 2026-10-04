using System.Diagnostics;
using Palmmedia.ReportGenerator.Core;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Common;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using System;

public unsafe struct NativeHull : IDisposable//定义凸体
{
public int VertexCount;//顶点数量
public int FaceCount; //面数量
public int EdgeCount; //边数量

// Unity 为 NativeArray、NativeList 等 Native 容器提供了一种叫做 泄漏检测 (Leak Detection) 的机制，
// 以下用于告诉 NativeArray 在构造时跳过 LeakDetection 注册，用来提高速度。
public NativeArrayNoLeakDetection<float3> VerticesNative;  // 顶点数组
public NativeArrayNoLeakDetection<NativeFace> FacesNative;  // 面数组
public NativeArrayNoLeakDetection<NativePlane> PlanesNative;  // 平面数组
public NativeArrayNoLeakDetection<NativeHalfEdge> EdgesNative;  // 半边数组

// 以下是直接指向内存的指针，供高效访问使用。
[NativeDisableUnsafePtrRestriction] //相当于一个属性，适用于burst编译器，允许使用unsafe指针并绕过安全检测
public float3* Vertices;  // 顶点指针

[NativeDisableUnsafePtrRestriction]
public NativeFace* Faces;  // 面指针

[NativeDisableUnsafePtrRestriction]
public NativePlane* Planes;  // 平面指针

[NativeDisableUnsafePtrRestriction]
public NativeHalfEdge* Edges;  // 半边指针

private int _isCreated;  // 标记该结构体是否已创建
private int _isDisposed; // 标记该结构体是否已释放

// 判断结构体是否已创建
public bool IsCreated
{
    get => _isCreated == 1;
    set => _isCreated = value ? 1 : 0;
}

// 判断结构体是否已释放
public bool IsDisposed
{
    get => _isDisposed == 1;
    set => _isDisposed = value ? 1 : 0;
}

// 判断该结构体是否有效（即已创建且未释放）
public bool IsValid => IsCreated && !IsDisposed;

// 释放内存
public void Dispose()
{
    if (_isDisposed == 0)
    {
        _isDisposed = 1;

        if (VerticesNative.IsCreated)
            VerticesNative.Dispose();

        if (FacesNative.IsCreated)
            FacesNative.Dispose();

        if (PlanesNative.IsCreated)
            PlanesNative.Dispose();

        if (EdgesNative.IsCreated)
            EdgesNative.Dispose();

        Vertices = null;
        Faces = null;
        Planes = null;
        Edges = null;
    }
}
}