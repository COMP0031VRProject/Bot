using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    
    public Mesh generate_polygon_mesh(int n, int size, (int x, int y) center)
    {
        double theta = 2 * Math.PI/n;
        double sin_theta = Math.Sin(theta);
        double cos_theta = Math.Cos(theta);
        Func<(double, double),(double,double)> relative2world = coord => (coord.Item1 + center.x, coord.Item2 + center.y);
        Func<(double, double), (double,double)> rotate = coord => (coord.Item1 * cos_theta - coord.Item2 * sin_theta, coord.Item1 * sin_theta + coord.Item2 * cos_theta);
        (double, double) first = (0, size);
        List<(double, double)> verts = new List<(double, double)>{ (0,0), first };
        List<(int, int, int)> tInd = new List<(int, int, int)>();
        (double, double) previous = first;
        for (int i = 1; i < n; i++)
        {
            (double, double) next_v = rotate(previous);
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
        double sin_theta = Math.Sin(theta);
        double cos_theta = Math.Cos(theta);
        Func<(double, double), (double, double)> relative2world = coord => (coord.Item1 + center.x, coord.Item2 + center.y);
        Func<(double, double), (double, double)> rotate = coord => (coord.Item1 * cos_theta - coord.Item2 * sin_theta, coord.Item1 * sin_theta + coord.Item2 * cos_theta);
        (double, double) first = (0, size);
        List<(double, double)> verts = new List<(double, double)> { (0, 0), first };

        List<(int, int, int)> tInd = new List<(int, int, int)>();
        (double, double) previous = first;
        for (int i = 1; i < n; i++)
        {
            (double, double) next_v = rotate(previous);
            verts.Add(next_v);
            tInd.Add((0, i, i + 1));
            previous = next_v;
        }
        tInd.Add((0, n, 1));
        verts.Add((0, size * M));

        for (int i = 1; i < n; i++)
        {
            int last = verts.Count - 1;
            (double, double)next_v = (verts[i + 1].Item2 * M, verts[i + 1].Item2* M);
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

    Func<(double, double), double> line_implicit_equation((double,double) A, (double,double)B)
    {
        double _A = A.Item2 - B.Item1;
        double _B = B.Item1 - A.Item1;
        double _C = A.Item1 * B.Item2 - B.Item2 * A.Item1;
        Func<(double,double), double> value = p => (_A* p.Item1 +_B * p.Item2 + _C);
        return value;
    }

    public bool is_point_in_triangle((double,double) P, (double,double) A, (double,double) B, (double,double) C)
    {
        double x = line_implicit_equation(A, B)(P);
        double y = line_implicit_equation(B, C)(P);
        double z = line_implicit_equation(C, A)(P);
        return x >= 0 && y >= 0 && z >= 0;
    }

    public (double, double, double) barycentric_coordinates((double, double) P, (double, double) A, (double, double) B, (double, double) C)
    {
        Func<(double, double), double> ab_line = line_implicit_equation(A, B);
        double gamma = ab_line(P) / ab_line(C);
        Func<(double, double), double> ca_line = line_implicit_equation(C, A);
        double beta = ca_line(P) / ca_line(B);
        Func<(double, double), double> bc_line = line_implicit_equation(B, C);
        double alpha = bc_line(P) / bc_line(A);
        return (alpha, beta, gamma);
    }

}
