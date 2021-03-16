/*
    This is the prototypical script for the bot metrics implementation.
    Currently unused, but linked to a hidden "metrics" object in the scene.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordMetrics : MonoBehaviour
{
    private float virtualDistance;
    private Vector3 lastPosition;
    public Transform realbot;
    public Transform virtualbot;

    // Metrics 
    private float T_AD;   // Angle error
    private float T_TCT;  // Task completion time
    private float T_RD;   // Real distance travelled
    private float T_VD;   // Virtual distance travelled
    private List<Metric> metricList;
    private Vector3 lastRealPosition;
    private Vector3 lastVirtualPosition;
    private float optimumPath;
    public float T_D;

    public void resetMetrics()
    {
        T_TCT = 0.0f;
        lastRealPosition = realbot.position;
        lastVirtualPosition = virtualbot.position;

    }

    // Update is called once per frame
    void Update()
    {
        T_TCT += Time.deltaTime;

        T_RD += Vector3.Distance(lastRealPosition, realbot.position);
        lastRealPosition = realbot.position;

        T_VD += Vector3.Distance(lastVirtualPosition, virtualbot.position);
        lastVirtualPosition = virtualbot.position;
    }

    public void saveMetrics()
    {
        T_D = (float)System.Math.Round((decimal)T_VD / (decimal)optimumPath, 2);
        Metric metrics = new Metric
        {
            T_TCT = T_TCT,
            T_RD = T_RD,
            T_D = T_D
        };

        metricList.Add(metrics);

        Debug.Log("T_TCT: " + T_TCT);
        Debug.Log("T_RD: " + T_RD);
        Debug.Log("T_VD: " + T_VD);
        Debug.Log("Optimum: " + optimumPath);
        Debug.Log("T_D: " + System.Math.Round((decimal)T_VD / (decimal)optimumPath, 2));
    }

    private void writeMetrics()
    {
        MetricWriter writer = new MetricWriter();
        writer.writeToFile(metricList);
    }

    private void OnDestroy()
    {

    }
}
