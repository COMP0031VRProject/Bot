using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    public Mesh virtualMesh;
    public Mesh realMesh;
    public Transform bot;
    public Transform startPosition;
    public Transform virtualPosition;
    public Utils util;

    void Start()
    {
        virtualPosition = startPosition;

    }

    object real2Virtual((float, float) P)
    {
        float i1, i2;
        foreach ((int t1, int t2, int t3) in realMesh.tInd)
        {
            (float, float) A, B, C;
            A = virtualMesh.verts[t1];
            B = virtualMesh.verts[t2];
            C = virtualMesh.verts[t3];

            if (util.is_point_in_triangle(P, A, B, C))
            {
                (float alpha, float beta, float gamma) = util.barycentric_coordinates(P, A, B, C);
                (float, float) vA, vB, vC;
                vA = virtualMesh.verts[t1];
                vB = virtualMesh.verts[t2];
                vC = virtualMesh.verts[t3];

                vA.Item1 = vA.Item1 * alpha;
                vA.Item2 = vA.Item2 * alpha;
                vB.Item1 = vB.Item1 * beta;
                vB.Item2 = vB.Item2 * beta;
                vC.Item1 = vC.Item1 * gamma;
                vC.Item2 = vC.Item2 * gamma;

                i1 = vA.Item1 + vB.Item1 + vC.Item1;
                i2 = vA.Item2 + vB.Item2 + vC.Item2;

                return (i1, i2);
            }

        }
        return null;
    }

    object virtual2Real((float, float) P)
    {
        float i1, i2;
        foreach ((int t1, int t2, int t3) in virtualMesh.tInd)
        {
            (float, float) A, B, C;
            A = virtualMesh.verts[t1];
            B = virtualMesh.verts[t2];
            C = virtualMesh.verts[t3];

            if(util.is_point_in_triangle(P, A, B, C))
            {
                (float alpha, float beta, float gamma) = util.barycentric_coordinates(P, A, B, C);
                (float, float) vA, vB, vC;
                vA = realMesh.verts[t1];
                vB = realMesh.verts[t2];
                vC = realMesh.verts[t3];

                vA.Item1 = vA.Item1* alpha;
                vA.Item2 = vA.Item2 * alpha;
                vB.Item1 = vB.Item1 * beta;
                vB.Item2 = vB.Item2 * beta;
                vC.Item1 = vC.Item1 * gamma;
                vC.Item2 = vC.Item2 * gamma;

                i1 = vA.Item1 + vB.Item1 + vC.Item1;
                i2 = vA.Item2 + vB.Item2 + vC.Item2;
               
                return (i1, i2);
            }

        }
        return null;
    }

}
