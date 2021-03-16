/*
    Initialises the Object Positions, and updates our virtual bot position based on realbot.
    Note: Perhaps realbot translation could be transferred over into bot script?
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    public Mesh virtualMesh;
    public Mesh realMesh;
    public Transform virtualPlane;
    public Transform realPlane;
    public TextAsset virtualJson;
    public TextAsset realJson;
    public Transform virtualbot;
    public Transform realbot;
    public Transform camera;
    public Transform[] flags;
    public bool homogenous;
    public bool polygon_manipulation;
    public bool lattice_crushing;
    // public int n;
    
    // Default Values, replaced in script with mesh.
    public float real_width_m;
    public float scale;

    private float offsetX;
    private float offsetZ;

    /*// What if I initialise the botscript within worldscript?
    private Bot bot;
    public float bot_speed;
    public int bot_numTargets;
    */

    void Start()
    {
        readMeshFiles();

        positionObjs(); //Position the flags and camera correctly
        
        //Then create our botscript (constructor running into problems, will hardcode scale for now)
        // bot = gameObject.AddComponent(typeof(Bot(flags, virtualbot, realbot, bot_speed, bot_numTargets)));        
    }


    void positionObjs()
    {
        float dist = scale * (real_width_m / 2f - real_width_m * 0.1f); //Distance of Flag
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
        float pos_avg = -0.5f * real_width_m;
        camera.position = new Vector3(pos_avg, scale * (0.8f * real_width_m) + 1, camera.position.z);
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

        //Updates the real_width_m (real space width in metres) from mesh
        float real_max = (float) realM.getMax();
        real_width_m = real_max * 2f;
        //Updates the scaling factor 
        scale = (float)virtualM.getMax() / real_max;
        Debug.Log("The Scale is: " + scale);

        gameObject.GetComponent<Bot>().setScale(scale);
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
        List<decimal> P = new List<decimal> {(decimal)pos.position.x + (decimal)offsetX, (decimal)pos.position.z + (decimal)offsetZ};

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
