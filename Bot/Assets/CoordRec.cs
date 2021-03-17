/*
    Each CoordRec object is created to store all the coords in a single trial
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordRec
{
    public int trial_id { get; set; }
    public List<List<decimal>> coords_R { get; set; } //Each coord is a List<decimal> of size 2
    public List<List<decimal>> coords_V { get; set; }
}