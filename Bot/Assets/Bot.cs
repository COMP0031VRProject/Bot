using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public Transform[] flags;
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
            Transform temp = flags[i];
            flags[i] = flags[rand_i];
            flags[rand_i] = temp;
        }
    }
    
    void Start()
    {
        Shuffle();
        flag_i = 0;
        transform.LookAt(flags[flag_i].position);
        prev_pos = flags[flag_i].position;
    }

    void Update()
    {
        dist = Vector3.Distance(transform.position, flags[flag_i].position);
        prev_dist = Vector3.Distance(transform.position, prev_pos);
        speed_scale = 1;
        if (dist < 0.5f)
        {
            prev_pos = flags[flag_i].position;
            IncreaseIndex();
        }
        if (dist < 1.4f || prev_dist < 1.4f)
        {
            speed_scale = Mathf.Min(dist, prev_dist) - 0.4f;
        }
        
        Move(speed_scale);
    }

    void Move(float speed_scale)
    {
        transform.Translate(Vector3.forward * speed * speed_scale * Time.deltaTime);
    }

    void IncreaseIndex()
    {
        flag_i++;
        if (flag_i >= flags.Length)
        {
            Shuffle();
            flag_i = 0;
        }
        transform.LookAt(flags[flag_i].position);
    }
}
