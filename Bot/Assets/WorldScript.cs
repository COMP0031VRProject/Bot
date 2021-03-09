using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    public Mesh virtualMesh;
    public Mesh realMesh;
    public TextAsset virtualJson;
    public TextAsset realJson;
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
        //generate_meshes();
        readMeshFiles();

    }

   /* void placeMeshes()
    {
        Utils util = new Utils();
        for (int i = 0; i < flags.Length; i++)
        {
            (float x, float y) center = (flags[i].position.x, flags[i].position.z);
            Mesh new_mesh = util.generate_embedded_polygon_mesh(20, 5, 2, (0, 0));

        }
    } */

    void FixedUpdate()
    {
       // Map the virtual bot to real bot 50 frames per second
       List<decimal>coordinates  = virtual2Real(virtualbot);
       if (coordinates != null)
       {
           realbot.position = new Vector3((float)coordinates[0], 0.5f, (float)coordinates[1]);
       }
       else
       {
            Debug.Log("damn");
       }
    }

    void readMeshFiles()
    {
        // Create meshes from json files
        realMesh = Newtonsoft.Json.JsonConvert.DeserializeObject<Mesh>(realJson.text);
        virtualMesh = Newtonsoft.Json.JsonConvert.DeserializeObject<Mesh>(virtualJson.text);
    }

    
   /*
    void generate_meshes()
    {
        Utils util = new Utils();

        virtualMesh = util.generate_embedded_polygon_mesh(20, 5, 2, (0, 0));
        realMesh = util.generate_embedded_polygon_mesh(20, 5, 2, (0, 0));


        // Homogenous or polygon manipulation
        List<decimal> center = realMesh.verts[0];

        for (int i = 0; i < realMesh.verts.Count; i++)
        {
            decimal x = (realMesh.verts[i][0] - center[0]) / (scale + center[0]);
            decimal y = (realMesh.verts[i][1] - center[1]) / (scale + center[1]);
            realMesh.verts[i] = (x, y);
        }

        if (polygon_manipulation)
        {
            for (int i = 0; i <= n; i++)
            {
                (decimal x, decimal y) = realMesh.verts[i];
                realMesh.verts[i] = ((x - center[0]) * scale + center[0], (y - center[1]) * scale + center[1]);
            }
        }
        
    } */

    List<decimal> real2Virtual(Transform pos)
    {

        Utils util = new Utils();
        List<decimal> P = new List<decimal> { (decimal)pos.position.x, (decimal)pos.position.z };
        decimal i1, i2;
        //Debug.Log(realMesh.tInd.Count);
        foreach (List<int> t in realMesh.tInd)
        {
            List<decimal> A, B, C;
            A = realMesh.verts[t[0]];
            B = realMesh.verts[t[1]];
            C = realMesh.verts[t[2]];
            Debug.Log(P[0]);

            if (util.is_point_in_triangle(P, A, B, C))
            {
                (decimal alpha, decimal beta, decimal gamma) = util.barycentric_coordinates(P, A, B, C);
                List<decimal> vA, vB, vC;
                vA = virtualMesh.verts[t[0]];
                vB = virtualMesh.verts[t[1]];
                vC = virtualMesh.verts[t[2]];
                //Debug.Log(vB);
                //Debug.Log(alpha);
                //Debug.Log(beta);
                //Debug.Log(gamma);

                vA[0] = vA[0] * alpha;
                vA[1] = vA[1] * alpha;
                vB[0] = vB[0] * beta;
                vB[1] = vB[1] * beta;
                vC[0] = vC[0] * gamma;
                vC[1] = vC[1] * gamma;

                //Debug.Log("A: " + vA);
                //Debug.Log("B: " + vB);
                //Debug.Log("c: " + vC);

                i1 = vA[0] + vB[0] + vC[0];
                i2 = vA[1] + vB[1] + vC[1];

                //Debug.Log(i1);
                //Debug.Log(i2);  

                return new List<decimal> {i1, i2};
            }

        }

        return null;
    }

    List<decimal> virtual2Real(Transform pos)
    {
        Utils util = new Utils();
        List<decimal> P = new List<decimal>{(decimal)pos.position.x, (decimal)pos.position.z};

        foreach (List<int> t in virtualMesh.tInd)
        {
            List<decimal> A, B, C;
            A = virtualMesh.verts[t[0]];
            B = virtualMesh.verts[t[1]];
            C = virtualMesh.verts[t[2]];
            Debug.Log(P[0]);
            Debug.Log(P[1]);

            if (util.is_point_in_triangle(P, A, B, C))
            {
                
                (decimal alpha, decimal beta, decimal gamma) = util.barycentric_coordinates(P, A, B, C);
                List<decimal> vA, vB, vC;
                vA = realMesh.verts[t[0]];
                vB = realMesh.verts[t[1]];
                vC = realMesh.verts[t[2]];

                vA[0] = vA[0] * alpha;
                vA[1] = vA[1] * alpha;
                vB[0] = vB[0] * beta;
                vB[1] = vB[1] * beta;
                vC[0] = vC[0] * gamma;
                vC[1] = vC[1] * gamma;

                decimal i1 = vA[0] + vB[0] + vC[0];
                decimal i2 = vA[1] + vB[1] + vC[1];

                Debug.Log(i1);
                Debug.Log(i2);



                return new List<decimal> { i1, i2 };
            }
        }
        return null;
    } 

}
