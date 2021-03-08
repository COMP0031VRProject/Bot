using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public Transform[] flags;
    public Transform virtualbot;
    public Transform realbot;
    public float speed;
    public int numTargets;

    // private int flag_i; //Unused in current situation
    private float speed_scale;
    private float dist;
    private Vector3 prev_pos;
    private float prev_dist;

    private List<int> flag_seq = new List<int>();
    private Transform target;
    private int ind;
    private bool done = true;
    private bool rotating = false;

    int wrapback(int n) {
        if (n < 0) {
            return n + 6;
        } else {
            return n % 6;
        }
    }

    void generateRandomFlagSeq(int n) {
        //n is number of flags in sequence, either 3 or 5.
        System.Random rnd = new System.Random();
        int flag1 = rnd.Next(6);
        flag_seq.Add(flag1);
        int flag2 = (flag1-1) + rnd.Next(2) * 2;
        flag2 = wrapback(flag2);
        flag_seq.Add(flag2);
        int flag3 = (flag2-1) + rnd.Next(2) * 2;
        flag3 = wrapback(flag3);
        flag_seq.Add(flag3);
        if (n == 5) {
            int flag4 = (flag3 - 2) + rnd.Next(2) * 4;
            flag4 = wrapback(flag4);
            flag_seq.Add(flag4);
            int flag5 = (flag4 + 3) % 6;
            flag_seq.Add(flag5);
        }
    }

    // void Shuffle() {
    //     System.Random rnd = new System.Random();
    //     for (int i = flags.Length - 1; i > 0; i--)
    //     {
    //         int rand_i = rnd.Next(i + 1);
    //         if (rand_i == flags.Length - 1) {
    //             //Prevent case where there's two consecutive flags chosen
    //             rand_i = 0;
    //         }
    //         Transform temp = flags[i];
    //         flags[i] = flags[rand_i];
    //         flags[rand_i] = temp;
    //     }
    // }
    
    void Start()
    {
        generateRandomFlagSeq(numTargets);
        // Shuffle();
        //flag_i = 0;
        ind = 0;
        target = flags[flag_seq[ind]];
        OrientToTarget();
        //prev_pos = flags[flag_i].position;
        prev_pos = target.position;
        done = false;
    }

    void FixedUpdate()
    {
        if (done) {
            return;
        }
        // dist = Vector3.Distance(virtualbot.position, flags[flag_i].position);
        dist = Vector3.Distance(virtualbot.position, target.position);
        prev_dist = Vector3.Distance(virtualbot.position, prev_pos);
        speed_scale = 1;
        if (dist < 0.5f)
        {
            prev_pos = virtualbot.position;
            ind += 1;
            if (ind >= flag_seq.Count) {
                //Terminate program
                done = true;
                return;
            }
            target = flags[flag_seq[ind]];
            //IncreaseIndex();
            
        }
        if (dist < 1.4f || prev_dist < 0.9f)
        {
            speed_scale = Mathf.Min(dist, prev_dist + 0.5f) - 0.4f;
        }

       // Debug.Log(virtualbot.position.x);
        OrientToTarget(); //Orient every single fixed update
        if (!rotating) {
            Move(speed_scale);    
        }
    }

    void Move(float speed_scale)
    {
        realbot.Translate(Vector3.forward * speed * speed_scale * Time.deltaTime);
    }

    // void IncreaseIndex()
    // {
    //     flag_i++;
    //     if (flag_i >= flags.Length)
    //     {
    //         Shuffle();
    //         flag_i = 0;
    //     }
    //     // OrientToTarget();
    // }

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
        Debug.Log(targetAngle);

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

        Debug.Log(virtualbot.rotation.eulerAngles);
        realbot.rotation = virtualbot.rotation;
    }
}
