using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    public Mesh virtualMesh;
    public Mesh realMesh;
    public Transform virtualbot;
    public Transform realbot;
    public Transform[] flags;
    public bool homogenous;
    public bool polygon_manipulation;
    public bool lattice_crushing;
    public int n;
    public int size;
    public int scale;



    void Start()
    {
        // Create all meshes for the environment
        generate_meshes();

    }

    void FixedUpdate()
    {
        // Map the virtual bot to real bot 50 frames per second
        (decimal x, decimal z) = virtual2Real(virtualbot);
        if (x != 0 | z!= 0)
        {
            realbot.position = new Vector3((float)x, 0.5f, (float)z);
        }
    }

    // TODO: Create mesh around each of the 6 flags and apply homogenous to rest of area
    void generate_meshes()
    {
        Utils util = new Utils();

        virtualMesh = util.generate_embedded_polygon_mesh(20, 5, 2, (0, 0));
        realMesh = util.generate_embedded_polygon_mesh(20, 5, 2, (0, 0));


        // Homogenous or polygon manipulation
        (decimal, decimal) center = realMesh.verts[0];

        for (int i = 0; i < realMesh.verts.Count; i++)
        {
            decimal x = (realMesh.verts[i].Item1 - center.Item1) / (scale + center.Item1);
            decimal y = (realMesh.verts[i].Item2 - center.Item2) / (scale + center.Item2);
            realMesh.verts[i] = (x, y);
        }

        if (polygon_manipulation)
        {
            for (int i = 0; i <= n; i++)
            {
                (decimal x, decimal y) = realMesh.verts[i];
                realMesh.verts[i] = ((x - center.Item1) * scale + center.Item1, (y - center.Item2) * scale + center.Item2);
            }
        }
        
    }

    (decimal, decimal) real2Virtual(Transform pos)
    {

        Utils util = new Utils();
        (decimal, decimal) P = ((decimal)pos.position.x, (decimal)pos.position.z);
        decimal i1, i2;
        //Debug.Log(realMesh.tInd.Count);
        foreach ((int t1, int t2, int t3) in realMesh.tInd)
        {
            (decimal, decimal) A, B, C;
            A = realMesh.verts[t1];
            B = realMesh.verts[t2];
            C = realMesh.verts[t3];
            

            if (util.is_point_in_triangle(P, A, B, C))
            {
                (decimal alpha, decimal beta, decimal gamma) = util.barycentric_coordinates(P, A, B, C);
                (decimal, decimal) vA, vB, vC;
                vA = virtualMesh.verts[t1];
                vB = virtualMesh.verts[t2];
                vC = virtualMesh.verts[t3];
                //Debug.Log(vB);
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

    (decimal, decimal) virtual2Real(Transform pos)
    {
        Utils util = new Utils();
        (decimal, decimal) P = ((decimal)pos.position.x, (decimal)pos.position.z);
        foreach ((int t1, int t2, int t3) in virtualMesh.tInd)
        {
            (decimal, decimal) A, B, C;
            A = virtualMesh.verts[t1];
            B = virtualMesh.verts[t2];
            C = virtualMesh.verts[t3];

            if(util.is_point_in_triangle(P, A, B, C))
            {
                //Debug.Log("IS in triangle " + P);
                (decimal alpha, decimal beta, decimal gamma) = util.barycentric_coordinates(P, A, B, C);
                (decimal, decimal) vA, vB, vC;
                vA = realMesh.verts[t1];
                vB = realMesh.verts[t2];
                vC = realMesh.verts[t3];

                vA.Item1 = vA.Item1* alpha;
                vA.Item2 = vA.Item2 * alpha;
                vB.Item1 = vB.Item1 * beta;
                vB.Item2 = vB.Item2 * beta;
                vC.Item1 = vC.Item1 * gamma;
                vC.Item2 = vC.Item2 * gamma;

                decimal i1 = vA.Item1 + vB.Item1 + vC.Item1;
                decimal i2 = vA.Item2 + vB.Item2 + vC.Item2;
               
                return (i1, i2);
            }
        }
        return (0,0);
    }

}
