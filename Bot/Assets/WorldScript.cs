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
    public Transform camera;
    public Transform[] flags;
    public bool homogenous;
    public bool polygon_manipulation;
    public bool lattice_crushing;
    public int n;
    public int size;
    public int scale;

    private decimal offsetX = 10.5m;
    private decimal offsetZ = 7.5m;

    void Start()
    {
        readMeshFiles();

        positionObjs(); //Position the flags and camera correctly
        // Create all meshes for the environment
        //generate_meshes();

    }


    void positionObjs()
    {
        int dist = scale; //Distance of flag {2m if scale = 2, 4m if scale = 4, etc...}
        camera.position = new Vector3(camera.position.x, scale * 2 + 1, camera.position.z);
        for (int i = 0; i < flags.Length; i++) {
            float x = Mathf.Cos(Mathf.PI/3 * i) * dist;
            float z = Mathf.Sin(Mathf.PI/3 * i) * dist;
            flags[i].position = new Vector3(x, flags[i].position.y, z);
        }
    }



    void FixedUpdate()
    {
        // Map the real bot to virtual bot 50 frames per second
        List<decimal> coordinates = new List<decimal>();
        coordinates = real2Virtual(realbot);
        if (coordinates != null) {
            virtualbot.position = new Vector3((float)coordinates[0], virtualbot.position.y, (float)coordinates[1]);
        }
    }

    void readMeshFiles()
    {
        // Create meshes from json files 
        Mesh realM = Newtonsoft.Json.JsonConvert.DeserializeObject<Mesh>(realJson.text);
        realMesh = new Mesh(realM.verts, realM.tInd);
        Mesh virtualM = Newtonsoft.Json.JsonConvert.DeserializeObject<Mesh>(virtualJson.text);
        virtualMesh = new Mesh(virtualM.verts, virtualM.tInd);

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
        //Add an extra offset here... (NOTE this is not added in V2R)
        List<decimal> P = new List<decimal> {(decimal)pos.position.x + offsetX, (decimal)pos.position.z + offsetZ};

        decimal i1, i2;
        //Debug.Log(realMesh.tInd.Count);
        foreach (List<int> t in realMesh.tInd)
        {
            List<decimal> A = new List<decimal>();
            List<decimal> B = new List<decimal>();
            List<decimal> C = new List<decimal>();
            A = realMesh.verts[t[0]];
            B = realMesh.verts[t[1]];
            C = realMesh.verts[t[2]];

            // Check if in triangle (anti-clockwise order)
            if (util.is_point_in_triangle(P, A, B, C))
            {
                //Debug.Log("IS INTHE TRIANGLE");
                (decimal alpha, decimal beta, decimal gamma) = util.barycentric_coordinates(P, A, B, C);
                List<decimal> vA = new List<decimal>(virtualMesh.verts[t[0]]);
                List<decimal> vB = new List<decimal>(virtualMesh.verts[t[1]]);
                List<decimal> vC = new List<decimal>(virtualMesh.verts[t[2]]);

                //Debug.Log(vA[0]);
                //Debug.Log(alpha);
                //Debug.Log(beta);
                //Debug.Log(gamma);

                vA[0] = vA[0] * alpha;
                vA[1] = vA[1] * alpha;
                vB[0] = vB[0] * beta;
                vB[1] = vB[1] * beta;
                vC[0] = vC[0] * gamma;
                vC[1] = vC[1] * gamma;


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
