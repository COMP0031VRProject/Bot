using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MetricWriter
{
    private string filePath = Application.dataPath;
    public void writeToFile(List<Metric> metrics)
    {
        filePath += "/MetricFiles/metrics.json";
        string metricJson = Newtonsoft.Json.JsonConvert.SerializeObject(metrics);
        File.WriteAllText(filePath, metricJson);
    }

}
