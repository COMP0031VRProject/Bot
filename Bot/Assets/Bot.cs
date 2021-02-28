using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public Transform[] flags;
    public Transform virtualbot;
    public Transform realbot;
    public float speed;

    private int flag_i;
    private float speed_scale;
    private float dist;
    private Vector3 prev_pos;
    private float prev_dist;

    void Shuffle() {
        System.Random rnd = new System.Random();
        for (int i = flags.Length - 1; i > 0; i--)
        {
            int rand_i = rnd.Next(i + 1);
            if (rand_i == flags.Length - 1) {
                //Prevent case where there's two consecutive flags chosen
                rand_i = 0;
            }
            Transform temp = flags[i];
            flags[i] = flags[rand_i];
            flags[rand_i] = temp;
        }
    }
    
    void Start()
    {
        Shuffle();
        flag_i = 0;
        OrientToTarget();
        prev_pos = flags[flag_i].position;
    }

    void FixedUpdate()
    {
        dist = Vector3.Distance(virtualbot.position, flags[flag_i].position);
        prev_dist = Vector3.Distance(virtualbot.position, prev_pos);
        speed_scale = 1;
        if (dist < 0.5f)
        {
            prev_pos = virtualbot.position;
            IncreaseIndex();
            
        }
        if (dist < 1.4f || prev_dist < 0.9f)
        {
            speed_scale = Mathf.Min(dist, prev_dist + 0.5f) - 0.4f;
        }

       // Debug.Log(virtualbot.position.x);

        Move(speed_scale);
    }

    void Move(float speed_scale)
    {
        realbot.Translate(Vector3.forward * speed * speed_scale * Time.deltaTime);
    }

    void IncreaseIndex()
    {
        flag_i++;
        if (flag_i >= flags.Length)
        {
            Shuffle();
            flag_i = 0;
        }
        OrientToTarget();
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
        
        float diffX = flags[flag_i].position.x - virtualbot.position.x;
        float diffZ = flags[flag_i].position.z - virtualbot.position.z;
        
        float targetAngle = getRotTarget(diffX, diffZ);
        Debug.Log(targetAngle);

        float angleDiff = targetAngle - virtualbot.rotation.eulerAngles.y;

        // virtualbot.LookAt(flags[flag_i].position);
        virtualbot.Rotate(Vector3.up, angleDiff);
        Debug.Log(virtualbot.rotation.eulerAngles);
        realbot.rotation = virtualbot.rotation;
    }
}
