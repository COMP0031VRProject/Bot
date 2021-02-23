﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    public Mesh virtualMesh;
    public Mesh realMesh;
    public Transform virtualbot;
    public Transform realbot;



    void Start()
    {
        //virtualPosition = startPosition;
        simulate();

    }

    void Update()
    {
        (double x, double z) = real2Virtual(realbot);
        virtualbot.position = new Vector3((float)x,0.5f,(float)z);
        //Debug.Log("x: " + x);
        //Debug.Log("z: " + z);
    }

    void simulate()
    {
        Utils util = new Utils();
        virtualMesh = util.generate_embedded_polygon_mesh(20, 5, 2, (0, 0));
        realMesh = virtualMesh;
        (double, double) center = realMesh.verts[0];
        int k = 2;
        for (int i = 0; i < realMesh.verts.Count; i++)
        {
            double x = (realMesh.verts[i].Item1 - center.Item1) / (k + center.Item1);
            double y = (realMesh.verts[i].Item2 - center.Item2) / (k + center.Item2);
            realMesh.verts[i] = (x, y);
        }
    }

    (double, double) real2Virtual(Transform pos)
    {
        Utils util = new Utils();
        (double, double) P = (pos.position.x, pos.position.z);
        double i1, i2;
        foreach ((int t1, int t2, int t3) in realMesh.tInd)
        {
            (double, double) A, B, C;
            A = virtualMesh.verts[t1];
            B = virtualMesh.verts[t2];
            C = virtualMesh.verts[t3];
            //Debug.Log(A);

            if (util.is_point_in_triangle(P, A, B, C))
            {
                (double alpha, double beta, double gamma) = util.barycentric_coordinates(P, A, B, C);
                (double, double) vA, vB, vC;
                vA = virtualMesh.verts[t1];
                vB = virtualMesh.verts[t2];
                vC = virtualMesh.verts[t3];
                Debug.Log(vA);
                //Debug.Log(alpha);
                //Debug.Log(beta);
                //Debug.Log(gamma);

                vA.Item1 = vA.Item1 * alpha;
                vA.Item2 = vA.Item2 * alpha;
                vB.Item1 = vB.Item1 * beta;
                vB.Item2 = vB.Item2 * beta;
                vC.Item1 = vC.Item1 * gamma;
                vC.Item2 = vC.Item2 * gamma;

                //Debug.Log("A: " + vA);
                //Debug.Log("B: " + vB);
                //Debug.Log("c: " + vC);

                i1 = vA.Item1 + vB.Item1 + vC.Item1;
                i2 = vA.Item2 + vB.Item2 + vC.Item2;

                //Debug.Log(i1);
                //Debug.Log(i2);

                return (i1, i2);
            }

        }
        return (0,0);
    }

    (double, double) virtual2Real(Transform pos)
    {
        Utils util = new Utils();
        (double, double) P = (pos.position.x, pos.position.z);
        foreach ((int t1, int t2, int t3) in virtualMesh.tInd)
        {
            (double, double) A, B, C;
            A = virtualMesh.verts[t1];
            B = virtualMesh.verts[t2];
            C = virtualMesh.verts[t3];

            if(util.is_point_in_triangle(P, A, B, C))
            {
                (double alpha, double beta, double gamma) = util.barycentric_coordinates(P, A, B, C);
                (double, double) vA, vB, vC;
                vA = realMesh.verts[t1];
                vB = realMesh.verts[t2];
                vC = realMesh.verts[t3];

                vA.Item1 = vA.Item1* alpha;
                vA.Item2 = vA.Item2 * alpha;
                vB.Item1 = vB.Item1 * beta;
                vB.Item2 = vB.Item2 * beta;
                vC.Item1 = vC.Item1 * gamma;
                vC.Item2 = vC.Item2 * gamma;

                double i1 = vA.Item1 + vB.Item1 + vC.Item1;
                double i2 = vA.Item2 + vB.Item2 + vC.Item2;
               
                return (i1, i2);
            }
        }
        return (0,0);
    }

}
