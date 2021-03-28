/*
    This is an object created for writing json format metrics to "metrics.json"
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MetricWriter
{
    private string filePath = Application.dataPath;
    public void writeToFile(List<Metric> metrics, int suite_id, string tAcronym)
    {
        filePath += "/MetricFiles/" + tAcronym + "_TestSuite_" + suite_id.ToString() + "_metrics.json";
        string metricJson = Newtonsoft.Json.JsonConvert.SerializeObject(metrics);
        File.WriteAllText(filePath, metricJson);
    }

}
