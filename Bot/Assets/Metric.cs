/*
    This object is created to store all the metrics in a single object standard format.
    Helps storing as a single element within a list. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metric
{
    public float T_SF { get; set; }
    public float T_TCT { get; set; }
    public float T_RD { get; set; }
    public float T_D { get; set; }
}
