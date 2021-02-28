using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    
    public Mesh generate_polygon_mesh(int n, int size, (decimal x, decimal y) center)
    {
        double theta = 2 * Math.PI/n;
        decimal sin_theta = (decimal)Math.Sin(theta);
        decimal cos_theta = (decimal)Math.Cos(theta);
        Func<(decimal, decimal),(decimal,decimal)> relative2world = coord => (coord.Item1 + center.x, coord.Item2 + center.y);
        Func<(decimal, decimal), (decimal,decimal)> rotate = coord => (coord.Item1 * cos_theta - coord.Item2 * sin_theta, coord.Item1 * sin_theta + coord.Item2 * cos_theta);
        (decimal, decimal) first = (0, size);
        List<(decimal, decimal)> verts = new List<(decimal, decimal)>{ (0,0), first };
        List<(int, int, int)> tInd = new List<(int, int, int)>();
        (decimal, decimal) previous = first;
        for (int i = 1; i < n; i++)
        {
            (decimal, decimal) next_v = rotate(previous);
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
        double theta = 2 * Math.PI / n;
        decimal sin_theta = (decimal)(Math.Sin(theta));
        decimal cos_theta = (decimal)(Math.Cos(theta));
        Func<(decimal, decimal), (decimal, decimal)> relative2world = coord => (coord.Item1 + center.x, coord.Item2 + center.y);
        Func<(decimal, decimal), (decimal, decimal)> rotate = coord => (coord.Item1 * cos_theta - coord.Item2 * sin_theta, coord.Item1 * sin_theta + coord.Item2 * cos_theta);
        (decimal, decimal) first = (0, size);
        List<(decimal, decimal)> verts = new List<(decimal, decimal)> { (0, 0), first };

        List<(int, int, int)> tInd = new List<(int, int, int)>();
        (decimal, decimal) previous = first;
        for (int i = 1; i < n; i++)
        {
            (decimal, decimal) next_v = rotate(previous);
            verts.Add(next_v);
            tInd.Add((0, i, i + 1));
            previous = next_v;
        }
        tInd.Add((0, n, 1));
        verts.Add((0, size * M));

        for (int i = 1; i < n; i++)
        {
            int last = verts.Count - 1;
            (decimal, decimal)next_v = (verts[i + 1].Item1 * M, verts[i + 1].Item2* M);
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

    Func<(decimal, decimal), decimal> line_implicit_equation((decimal,decimal) A, (decimal,decimal)B)
    {
        decimal _A = A.Item2 - B.Item2;
        decimal _B = B.Item1 - A.Item1;
        decimal _C = A.Item1 * B.Item2 - B.Item1* A.Item2;
        Func<(decimal,decimal), decimal> value = p => (_A* p.Item1 +_B * p.Item2 + _C);
        return value;
    }

    public bool is_point_in_triangle((decimal,decimal) P, (decimal,decimal) A, (decimal,decimal) B, (decimal,decimal) C)
    {
        float x = (float)line_implicit_equation(A, B)(P);
        float y = (float)line_implicit_equation(B, C)(P);
        float z = (float)line_implicit_equation(C, A)(P);


        return x >= 0 && y >= 0 && z >= 0;
    }

    public (decimal, decimal, decimal) barycentric_coordinates((decimal, decimal) P, (decimal, decimal) A, (decimal, decimal) B, (decimal, decimal) C)
    {
        Func<(decimal, decimal), decimal> ab_line = line_implicit_equation(A, B);
        decimal gamma = ab_line(P) / ab_line(C);
        Func<(decimal, decimal), decimal> ca_line = line_implicit_equation(C, A);
        decimal beta = ca_line(P) / ca_line(B);
        Func<(decimal, decimal), decimal> bc_line = line_implicit_equation(B, C);
        decimal alpha = bc_line(P) / bc_line(A);
        return (alpha, beta, gamma);
    }

}
