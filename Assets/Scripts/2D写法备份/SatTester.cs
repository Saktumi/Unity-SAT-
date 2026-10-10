using UnityEngine;

public class SatTester : MonoBehaviour
{
    [Header("形状 A（顶点按顺序绕一圈，必须是凸的）")]
    public Vector2 centerA = new Vector2(-2f, 0f);
    public float   angleA  = 0f;
    public Vector2[] shapeA =
    {
        new Vector2( 0.0f,  0.0f),
        new Vector2( 2.4f,  0.3f),
        new Vector2( 2.8f,  2.0f),
        new Vector2( 1.0f,  3.2f),
        new Vector2(-0.4f,  1.6f),
    };

    [Header("形状 B（顶点按顺序绕一圈，必须是凸的）")]
    public Vector2 centerB = new Vector2(1.5f, 0.5f);
    public float   angleB  = 0f;
    public Vector2[] shapeB =
    {
        new Vector2(-1.2f, -0.5f),
        new Vector2( 1.0f, -1.3f),
        new Vector2( 1.6f,  0.8f),
        new Vector2( 0.0f,  1.5f),
        new Vector2(-1.5f,  0.6f),
    };

    [Header("显示选项")]
    public bool showAxes = true;   // 是否画出 A 的候选轴

    bool lastHit;
    bool warnedConcave;

    void Update()
    {
        Vector2[] a = Build(shapeA, centerA, angleA);
        Vector2[] b = Build(shapeB, centerB, angleB);

        bool hit = GetAxis.IsColliding(a, b);

        if (hit != lastHit)                    // 只在结果变化时打印，免得刷屏
        {
            Debug.Log(hit ? "碰撞了" : "没碰上");
            lastHit = hit;
        }
    }

    void OnDrawGizmos()
    {
        Vector2[] a = Build(shapeA, centerA, angleA);
        Vector2[] b = Build(shapeB, centerB, angleB);

        // 凸性检查：形状一凹，SAT 的结论就不可信，用洋红色提醒
        bool ok = IsConvex(a) && IsConvex(b);

        if (!ok && !warnedConcave)
        {
            Debug.LogWarning("形状是凹的（顶点顺序或位置有问题），SAT 结果不可信");
            warnedConcave = true;
        }

        Gizmos.color = !ok ? Color.magenta
                      : (GetAxis.IsColliding(a, b) ? Color.red : Color.green);

        DrawPoly(a);
        DrawPoly(b);

        if (showAxes) DrawAxes(a, centerA);
    }

    // 把形状旋转 + 平移到指定位置（顶点是相对形状自己原点的局部坐标）
    static Vector2[] Build(Vector2[] shape, Vector2 center, float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        var world = new Vector2[shape.Length];
        for (int i = 0; i < shape.Length; i++)
        {
            Vector2 p = shape[i];
            // 先旋转，再平移
            world[i] = new Vector2(p.x * cos - p.y * sin,
                                   p.x * sin + p.y * cos) + center;
        }

        return world;
    }

    // 画多边形的轮廓
    static void DrawPoly(Vector2[] poly)
    {
        for (int i = 0; i < poly.Length; i++)
        {
            Vector3 p1 = poly[i];
            Vector3 p2 = poly[(i + 1) % poly.Length];
            Gizmos.DrawLine(p1, p2);
        }
    }

    // 画出所有候选轴（过形状中心，黄色细线）
    static void DrawAxes(Vector2[] poly, Vector2 center)
    {
        var axes = GetAxis.GetAxes(poly);

        Color old = Gizmos.color;
        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.5f);

        foreach (Vector2 axis in axes)
        {
            Gizmos.DrawLine(center - axis * 6f, center + axis * 6f);
        }

        Gizmos.color = old;
    }

    // 顶点是否凸：相邻两条边的叉积方向必须始终一致
    static bool IsConvex(Vector2[] poly)
    {
        int sign = 0;

        for (int i = 0; i < poly.Length; i++)
        {
            Vector2 a = poly[i];
            Vector2 b = poly[(i + 1) % poly.Length];
            Vector2 c = poly[(i + 2) % poly.Length];

            float cross = (b.x - a.x) * (c.y - b.y) - (b.y - a.y) * (c.x - b.x);

            if (Mathf.Abs(cross) < 1e-6f) continue;   // 三点共线，跳过

            int s = cross > 0 ? 1 : -1;
            if (sign == 0) sign = s;
            else if (s != sign) return false;         // 拐弯方向反了 → 有凹角
        }

        return true;
    }
}