using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public Mesh generate_polygon_mesh(int n, int size, (int x, int y) center)
    { 
        float theta = 2 * Mathf.PI/n;
        float sin_theta = Mathf.Sin(theta);
        float cos_theta = Mathf.Cos(theta);
        Func<(float, float),(float,float)> relative2world = coord => (coord.Item1 + center.x, coord.Item2 + center.y);
        Func<(float, float), (float,float)> rotate = coord => (coord.Item1 * cos_theta - coord.Item2 * sin_theta, coord.Item1 * sin_theta + coord.Item2 * cos_theta);
        (float, float) first = (0, size);
        List<(float, float)> verts = new List<(float, float)>{ (0,0), first };
        List<(int, int, int)> tInd = new List<(int, int, int)>();
        (float, float) previous = first;
        for (int i = 1; i < n; i++)
        {
            (float, float) next_v = rotate(previous);
            verts.Add(next_v);
            tInd.Add((0, i, i + 1));
            previous = next_v;
        }
        tInd.Add((0, n, 1));

        for (int i = 1; i < n; i++)
        {
            verts[i] = relative2world(verts[i]);
        }


        Mesh mesh = new Mesh(verts, tInd);
        return mesh;
    }

    public Mesh generate_embedded_polygon_mesh(int n, int M, int size, (int x, int y) center)
    {
        float theta = 2 * Mathf.PI / n;
        float sin_theta = Mathf.Sin(theta);
        float cos_theta = Mathf.Cos(theta);
        Func<(float, float), (float, float)> relative2world = coord => (coord.Item1 + center.x, coord.Item2 + center.y);
        Func<(float, float), (float, float)> rotate = coord => (coord.Item1 * cos_theta - coord.Item2 * sin_theta, coord.Item1 * sin_theta + coord.Item2 * cos_theta);
        (float, float) first = (0, size);
        List<(float, float)> verts = new List<(float, float)> { (0, 0), first };

        List<(int, int, int)> tInd = new List<(int, int, int)>();
        (float, float) previous = first;
        for (int i = 1; i < n; i++)
        {
            (float, float) next_v = rotate(previous);
            verts.Add(next_v);
            tInd.Add((0, i, i + 1));
            previous = next_v;
        }
        tInd.Add((0, n, 1));
        verts.Add((0, size * M));

        for (int i = 1; i < n; i++)
        {
            int last = verts.Count - 1;
            (float, float)next_v = (verts[i + 1].Item2 * M, verts[i + 1].Item2* M);
            verts.Add(next_v);
            tInd.Add((i, last, last + 1));
            tInd.Add((i, last + 1, i + 1));
        }

        tInd.Add((1, n, n + 1));
        tInd.Add((n, n + n, n + 1));

        for (int i = 1; i < n; i++)
        {
            verts[i] = relative2world(verts[i]);
        }

        Mesh mesh = new Mesh(verts, tInd);
        return mesh;
    }

    Func<(float, float), float> line_implicit_equation((float,float) A, (float,float)B)
    {
        float _A = A.Item2 - B.Item1;
        float _B = B.Item1 - A.Item1;
        float _C = A.Item1 * B.Item2 - B.Item2 * A.Item1;
        Func<(float,float), float> value = p => (_A* p.Item1 +_B * p.Item2 + _C);
        return value;
    }

    public bool is_point_in_triangle((float,float) P, (float,float) A, (float,float) B, (float,float) C)
    {
        float x = line_implicit_equation(A, B)(P);
        float y = line_implicit_equation(B, C)(P);
        float z = line_implicit_equation(C, A)(P);
        return x >= 0 && y >= 0 && z >= 0;
    }
    
    
}
