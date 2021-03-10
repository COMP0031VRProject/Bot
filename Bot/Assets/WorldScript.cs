using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    public Mesh[] virtualMeshs;
    public Mesh[] realMeshs;
    public Mesh virtualMesh;
    public Mesh realMesh;
    public Transform virtualPlane;
    public Transform realPlane;
    public Transform virtualbot;
    public Transform realbot;
    public Transform camera;
    public Transform[] flags;
    public bool homogenous;
    public bool polygon_manipulation;
    public bool lattice_crushing;
    public int n;
    public int real_width_m;
    public int scale;

    private float offsetX;
    private float offsetZ;

    void Start()
    {
        positionObjs(); //Position the flags and camera correctly
        
        // Create all meshes for the environment
        generate_meshes();

    }

    void positionObjs()
    {
        int dist = scale * 4; //Distance of flag {2m if scale = 2, 4m if scale = 4, etc...}
        for (int i = 0; i < flags.Length; i++) {
            float x = Mathf.Cos(Mathf.PI/3 * i) * dist;
            float z = Mathf.Sin(Mathf.PI/3 * i) * dist;
            flags[i].position = new Vector3(x, flags[i].position.y, z);
        }

        // Init Plane Positions and Sizes
        float virtual_width = real_width_m * scale / 10f;
        virtualPlane.localScale = new Vector3(virtual_width, 1f, virtual_width);
        
        float real_width = real_width_m / 10f;
        realPlane.localScale = new Vector3(real_width, 1f, real_width);
        
        offsetX = 0.5f * 10f * (real_width + virtual_width);
        offsetZ = 0.5f * 10f * (virtual_width - real_width);
        
        realPlane.position = new Vector3(-offsetX, 0f, -offsetZ);

        // Position Bots in Center of spaces
        virtualbot.position = virtualPlane.position + new Vector3(0, 0.9f, 0);
        realbot.position = realPlane.position + new Vector3(0, 0.9f, 0);

        // Place Camera
        float pos_avg = 0.5f * (realPlane.position.x + virtualPlane.position.x);
        camera.position = new Vector3(pos_avg + 2.5f, scale * 8 + 1, camera.position.z);
    }

    void placeMeshes()
    {
        Utils util = new Utils();
        for (int i = 0; i < flags.Length; i++)
        {
            (float x, float y) center = (flags[i].position.x, flags[i].position.z);
            Mesh new_mesh = util.generate_embedded_polygon_mesh(20, 5, 2, (0, 0));

        }
    }

    void FixedUpdate()
    {
        // Map the real bot to virtual bot 50 frames per second
        (decimal x, decimal z) = real2Virtual(realbot);
        if (x != 0 | z != 0) {
            virtualbot.position = new Vector3((float)x, virtualbot.position.y, (float)z);
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
        //Add an extra offset here... (NOTE this is not added in V2R)
        (decimal, decimal) P = ((decimal)pos.position.x + (decimal)offsetX, (decimal)pos.position.z + (decimal)offsetZ);
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
