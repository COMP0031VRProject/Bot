/* 
    This is the script that handles most of the bot movement, and recording metrics.
    Translation of the real to virtual position for the bot is currently performed in Worldscript.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public Transform[] flags;
    public Transform virtualbot;
    public Transform realbot;
    public Transform center;
    public float speed;
    public TextAsset testJson;
    public string tAcronym;

    // private int flag_i; //Unused in current situation
    private float speed_scale;
    private float dist;
    private Vector3 prev_pos;
    private Vector3 prev_location;
    private float prev_dist;

    private List<int> flag_seq = new List<int>();
    private Transform target;
    private int ind;
    private bool done = true;
    private bool rotating = false;

    // Metric variables 
    // Metrics 
    private List<float> T_AD;   // Angle error
    private float T_TCT;  // Task completion time
    private float T_RD;   // Real distance travelled
    private float T_VD;   // Virtual distance travelled
    private List<Metric> metricList;
    private Vector3 lastRealPosition;
    private Vector3 lastVirtualPosition;
    private float optimumPath;
    private float T_D; // (T_VD / optimumPath)
    private float T_SF; // Scaling factor

    //Test suite
    private TestSuite suite;
    private int trial_no = 1;

    private Vector3 virtualStart;
    private Vector3 realStart;

    // Coords Vars
    private List<CoordRec> coordRecs = new List<CoordRec>();
    private List<List<decimal>> coords_R;
    private List<List<decimal>> coords_V;

    public void setScale(float scale) {
        T_SF = scale;
        Debug.Log("This is the scale being set in bot: " + T_SF);
    }

    void readTestSuite() {
        //Do deserialisation and setting appropriate variables here...
        suite = Newtonsoft.Json.JsonConvert.DeserializeObject<TestSuite>(testJson.text);
        // Debug.Log("Number of Trials in Suite: " + suite.trials.Count); 
        /*
        Debug.Log("Suite ID: " + suite.suite_id);
        Debug.Log("Trials 1 id: " + suite.trials[0].id);
        Debug.Log("Trials 1 sequence [0]: " + suite.trials[0].sequence[0]);
        Debug.Log("Trials 1 sequence [1]: " + suite.trials[0].sequence[1]);
        Debug.Log("Trials 1 sequence [2]: " + suite.trials[0].sequence[2]);
        */
    }
    

    void resetMetrics()
    {
        T_TCT = 0.0f;
        T_RD = 0.0f;
        T_VD = 0.0f;
        T_D = 0.0f;
        optimumPath = 0.0f;
        T_AD = new List<float>();
        lastRealPosition = realbot.position;
        lastVirtualPosition = virtualbot.position;
    }

    void updateMetrics()
    {
        Vector3 realPath = realbot.position - lastRealPosition;
        Vector3 virtualPath = virtualbot.position - lastVirtualPosition;
        float angleError = Vector3.Angle(virtualPath, realPath);
        T_AD.Add(angleError);

        T_TCT += Time.fixedDeltaTime;

        T_RD += Vector3.Distance(lastRealPosition, realbot.position);
        lastRealPosition = realbot.position;

        T_VD += Vector3.Distance(lastVirtualPosition, virtualbot.position);
        lastVirtualPosition = virtualbot.position;
    }

    // Load current tests' metrics into list
    void saveMetrics()
    {
        T_D = (float)System.Math.Round((decimal)T_VD / (decimal)optimumPath, 3);

        Metric metrics = new Metric
        {
            id = trial_no,
            T_SF = T_SF,
            T_TCT = T_TCT,
            T_RD = T_RD,
            T_AD = T_AD,
            T_D = T_D
        };

        metricList.Add(metrics);
        /*Debug.Log("T_TCT: " + T_TCT);
        Debug.Log("T_RD: " + T_RD);
        Debug.Log("T_VD: " + T_VD);
        Debug.Log("Optimum: " + optimumPath);
        Debug.Log("T_D: " + System.Math.Round((decimal)T_VD / (decimal)optimumPath, 2)); 8?*/
    }

    void resetCoordRec(){
        coords_R = new List<List<decimal>>();
        coords_V = new List<List<decimal>>();
    }

    void updateCoordRec(){
        coords_R.Add(gameObject.GetComponent<WorldScript>().getRealCoord());
        coords_V.Add(gameObject.GetComponent<WorldScript>().getVirtualCoord());
    }

    void saveCoordRec() {
        CoordRec cr = new CoordRec {
            trial_id = trial_no,
            coords_R = coords_R,
            coords_V = coords_V  
        };

        coordRecs.Add(cr);
    }

    void writeMetrics()
    {
        MetricWriter writer = new MetricWriter();
        writer.writeToFile(metricList, suite.suite_id, tAcronym);
    }

    void writeCRs()
    {
        CRWriter writer = new CRWriter();
        writer.writeToFile(coordRecs, suite.suite_id, tAcronym);
    }


    int wrapback(int n) {
        if (n < 0) {
            return n + 6;
        } else {
            return n % 6;
        }
    }

    void loadFixedFlagSeq(int trial_no) {
        //Load the trial_ind flag sequence in the test suite
        int trial_ind = trial_no - 1; //Trial id starts at 1, but trial index starts at 0;
        flag_seq = suite.trials[trial_ind].getFlagSeq();
    }
    
    // initTrial: Loads targets, zeros metrics and bot indicies, orient the bot to target
    // setup the prev_pos and prev_location vars, then undo done flag to start the next trial.
    void initTrial() {
        Debug.Log("Trial " + trial_no + " of " + suite.trials.Count); //Help get a sense of progress
        loadFixedFlagSeq(trial_no);
        resetMetrics();
        resetCoordRec();
        ind = 0;
        target = flags[flag_seq[ind]];
        OrientToTarget();
        prev_pos = center.position;
        prev_location = virtualbot.position;
        done = false;
    }

    void Start()
    {
        // Test out our reading function
        readTestSuite();
        metricList = new List<Metric>();
        coordRecs = new List<CoordRec>();
        // Set the starting positions
        virtualStart = virtualbot.position;
        realStart = realbot.position;
        // Initiate the trial
        initTrial();
    }

    void updateVirtual() {
        // Map the real bot to virtual bot 50 frames per second
        List<decimal> coordinates = new List<decimal>();
        coordinates = gameObject.GetComponent<WorldScript>().real2Virtual(realbot);
        if (coordinates != null) {
            virtualbot.position = new Vector3((float)coordinates[0], virtualbot.position.y, (float)coordinates[1]);
        }
    }

    void FixedUpdate()
    {
        if (done) {
            if (trial_no == suite.trials.Count) {
                saveMetrics();
                saveCoordRec();
                writeMetrics();
                Debug.Log("Finished Writing Metrics to File!");
                writeCRs();
                Debug.Log("Finished Writing CRs to File!");
                Debug.Break(); //Stops running at runtime.
                return; // Should not reach this line in theory..  
            } else {
                //Save our metrics
                saveMetrics();
                saveCoordRec();
                // Increment the trial_no
                trial_no += 1;
                //Reset position of virtualbot and realbot...
                virtualbot.position = virtualStart;
                realbot.position = realStart;
                // Init Trial
                initTrial();
            }
        }
        
        // dist = Vector3.Distance(virtualbot.position, flags[flag_i].position);
        dist = Vector3.Distance(virtualbot.position, target.position);
        prev_dist = Vector3.Distance(virtualbot.position, prev_pos);
        speed_scale = 1;
        if ((dist < 0.5f && ind != flag_seq.Count) || dist < 0.01f)
        {
            optimumPath += Vector3.Distance(prev_location, virtualbot.position);
            prev_location = virtualbot.position;
            prev_pos = virtualbot.position;
            ind += 1;
            if (ind < flag_seq.Count) {
                target = flags[flag_seq[ind]];
            }
            else if (ind == flag_seq.Count) {
                // Case where we need to return to center
                target = center;
            }
            else if (ind > flag_seq.Count) {
                //Terminate current trial...
                done = true;
                return;
            }
        }
        // Return to center speed scaling
        if (dist < 0.9f && ind == flag_seq.Count) {
            speed_scale = dist + 0.1f; //Ensures at least 0.1 sf
        }
        // Normal speed scaling
        else if (dist < 1.4f || prev_dist < 0.9f)
        {
            speed_scale = Mathf.Min(dist, prev_dist + 0.5f) - 0.4f;
        }
        

       // Debug.Log(virtualbot.position.x);
        OrientToTarget(); //Orient every single fixed update
        if (!rotating) {
            Move(speed_scale);
            updateVirtual();
        }

        updateMetrics();
        updateCoordRec(); //Whenever we update metrics, update CoordRec
    }

    void Move(float speed_scale)
    {
        realbot.Translate(Vector3.forward * speed * speed_scale * Time.fixedDeltaTime);
    }

    float getRotTarget(float diffX, float diffZ) {
        //This here figures out the absolute rotation target.
        
        float targetAngle = 0;

        if (diffX > 0 && diffZ > 0) {
            targetAngle = Mathf.Atan(diffX / diffZ);
        }
        if (diffX > 0 && diffZ < 0) {
            targetAngle = Mathf.PI - Mathf.Atan(diffX / -diffZ);
        }
        if (diffX < 0 && diffZ < 0) {
            targetAngle = Mathf.PI + Mathf.Atan(diffX/diffZ);
        }
        if (diffX < 0 && diffZ > 0) {
            targetAngle = 2f * Mathf.PI - Mathf.Atan(-diffX/diffZ);
        }
        if (diffZ == 0) {
            if (diffX >= 0) {
                targetAngle = Mathf.PI / 2f;
            } else {
                targetAngle = - Mathf.PI / 2f;
            }
        }
        targetAngle *= (180f / Mathf.PI);
        
        return targetAngle;
    }
    
    void OrientToTarget() {
        
        // float diffX = flags[flag_i].position.x - virtualbot.position.x;
        // float diffZ = flags[flag_i].position.z - virtualbot.position.z;

        float diffX = target.position.x - virtualbot.position.x;
        float diffZ = target.position.z - virtualbot.position.z;
        float maxRot = 5f; //A cap on the rotation per second
        
        float targetAngle = getRotTarget(diffX, diffZ);
        //Debug.Log(targetAngle);

        float angleDiff = targetAngle - virtualbot.rotation.eulerAngles.y;
        if (Mathf.Abs(angleDiff) >= maxRot) {
            rotating = true;
        } else {
            rotating = false; //Can start moving again.
        }

        //Select the most efficient rotation direction
        if (angleDiff > 180) {
            angleDiff -= 360;
        }
        if (angleDiff < -180) {
            angleDiff += 360;
        }

        // virtualbot.LookAt(flags[flag_i].position);

        if (angleDiff < 0) {
            virtualbot.Rotate(Vector3.up, Mathf.Max(angleDiff, -maxRot));
        } else {
            virtualbot.Rotate(Vector3.up, Mathf.Min(angleDiff, maxRot));
        }

        //Debug.Log(virtualbot.rotation.eulerAngles);
        realbot.rotation = virtualbot.rotation;
    }
}
