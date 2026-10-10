using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
public class GetAxis
{
    public static List<UnityEngine.Vector2> GetAxes(UnityEngine.Vector2[] shapes1)
    {
        var axes = new List<UnityEngine.Vector2>();
        for (int i = 0 ;i<shapes1.Length;i++)
        {
            UnityEngine.Vector2 p1 = shapes1[i];
            UnityEngine.Vector2 p2 = shapes1[(i+1)%shapes1.Length];
            UnityEngine.Vector2 axesChildren = p1-p2; //edge
             if (axesChildren.sqrMagnitude < 0.0001f) continue; //axis
            UnityEngine.Vector2 axesAns =new UnityEngine.Vector2(-axesChildren.y,axesChildren.x);
            axes.Add(axesAns.normalized);
        }
        return axes;
    }

    static UnityEngine.Vector2 Shadow(UnityEngine.Vector2[] poly, UnityEngine.Vector2 axis)
    {
    float min = UnityEngine.Vector2.Dot(poly[0], axis);   
    float max = min;

    for (int i = 1; i < poly.Length; i++)     
    {
        float p = UnityEngine.Vector2.Dot(poly[i], axis); 

        if (p < min) min = p;                 
        if (p > max) max = p;                
    }

    return new UnityEngine.Vector2(min, max);             
    }

    public static bool IsColliding (UnityEngine.Vector2[] poly1,UnityEngine.Vector2[] poly2)
    {
         var axes = new List<UnityEngine.Vector2>();
         axes.AddRange(GetAxes(poly1));
         axes.AddRange(GetAxes(poly2));

         foreach(var x in axes)
        {
            var shadow1 = Shadow(poly1,x);
            var shadow2 = Shadow(poly2,x);

            if (shadow1.y<shadow2.x || shadow2.y<shadow1.x)
            {
                return false;
            }
        }

        return true;
    }
}