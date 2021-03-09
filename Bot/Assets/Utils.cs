using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    /*
    public Mesh generate_polygon_mesh(int n, int size, (decimal x, decimal y) center)
    {
        double theta = 2 * Math.PI/n;
        decimal sin_theta = (decimal)Math.Sin(theta);
        decimal cos_theta = (decimal)Math.Cos(theta);
        Func<List<decimal>,List<decimal>> relative2world = coord => (coord[0] + center.x, coord[1] + center.y);
        Func<List<decimal>, List<decimal>> rotate = coord => (coord[0] * cos_theta - coord[1] * sin_theta, coord[0] * sin_theta + coord[1] * cos_theta);
        List<decimal> first = (0, size);
        List<List<decimal>> verts = new List<List<decimal>>{ (0,0), first };
        List<(int, int, int)> tInd = new List<(int, int, int)>();
        List<decimal> previous = first;
        for (int i = 1; i < n; i++)
        {
            List<decimal> next_v = rotate(previous);
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
        Func<List<decimal>, List<decimal>> relative2world = coord => (coord[0] + center.x, coord[1] + center.y);
        Func<List<decimal>, List<decimal>> rotate = coord => (coord[0] * cos_theta - coord[1] * sin_theta, coord[0] * sin_theta + coord[1] * cos_theta);
        List<decimal> first = (0, size);
        List<List<decimal>> verts = new List<List<decimal>> { (0, 0), first };

        List<(int, int, int)> tInd = new List<(int, int, int)>();
        List<decimal> previous = first;
        for (int i = 1; i < n; i++)
        {
            List<decimal> next_v = rotate(previous);
            verts.Add(next_v);
            tInd.Add((0, i, i + 1));
            previous = next_v;
        }
        tInd.Add((0, n, 1));
        verts.Add((0, size * M));

        for (int i = 1; i < n; i++)
        {
            int last = verts.Count - 1;
            List<decimal>next_v = (verts[i + 1][0] * M, verts[i + 1][1]* M);
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
    } */

    Func<List<decimal>, decimal> line_implicit_equation(List<decimal> A, List<decimal>B)
    {
        decimal _A = A[1] - B[1];
        decimal _B = B[0] - A[0];
        decimal _C = A[0] * B[1] - B[0] * A[1];
        Func<List<decimal>, decimal> value = p => (_A* p[0] +_B * p[1] + _C);
        return value;
    }

    public bool is_point_in_triangle(List<decimal> P, List<decimal> A, List<decimal> B, List<decimal> C)
    {
        float x = (float)line_implicit_equation(A, B)(P);
        float y = (float)line_implicit_equation(B, C)(P);
        float z = (float)line_implicit_equation(C, A)(P);

        return x >= 0 && y >= 0 && z >= 0;
    }

    public (decimal, decimal, decimal) barycentric_coordinates(List<decimal> P, List<decimal> A, List<decimal> B, List<decimal> C)
    {
        Func<List<decimal>, decimal> ab_line = line_implicit_equation(A, B);
        decimal gamma = ab_line(P) / ab_line(C);
        Func<List<decimal>, decimal> ca_line = line_implicit_equation(C, A);
        decimal beta = ca_line(P) / ca_line(B);
        Func<List<decimal>, decimal> bc_line = line_implicit_equation(B, C);
        decimal alpha = bc_line(P) / bc_line(A);
        return (alpha, beta, gamma);
    }
    
}
