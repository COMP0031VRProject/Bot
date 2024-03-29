﻿/*
    This is an object created for writing json format CRs to "Testsuite_{suite_id}_coods.json"
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CRWriter
{
    private string filePath = Application.dataPath;
    public void writeToFile(List<CoordRec> CRs, int suite_id, string tAcronym, int parts)
    {
        string part = "";
        if (parts > 0)
        {
            part = "_part" + parts.ToString();
        }
        filePath += "/MetricFiles/" + tAcronym + "_TestSuite_" + suite_id.ToString() + "_coords" + part + ".json";
        string coordJson = Newtonsoft.Json.JsonConvert.SerializeObject(CRs);
        File.WriteAllText(filePath, coordJson);
    }

}
