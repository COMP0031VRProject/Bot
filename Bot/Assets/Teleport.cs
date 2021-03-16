/* 
    This was written as an experimental script for adding delayed teleportation of unity objects.
    Currently not used in implementation of the bot or metrics. 
*/

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Vector3[] points;
    public int delayInSeconds;

    private int pos_i;

    void Start()
    {
        pos_i = 0;
        transform.position = points[pos_i];

        StartCoroutine(WaitThenTeleport());
    }

    private IEnumerator WaitThenTeleport()
    {
        while (true)
        {
            pos_i += 1;
            if (pos_i >= points.Length)
            {
                pos_i = 0;
            }
            transform.LookAt(points[pos_i]); //Look at next point first
            yield return new WaitForSecondsRealtime(delayInSeconds);
            transform.position = points[pos_i];
        }
    }
}
