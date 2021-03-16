/*
    This class defines the TestSuite object, which is deserialised in Bot.
*/
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class TestSuite
{
    public int suite_id;
    public int flag_num;
    public float distance;
    public List<Trial> trials;

    // public List<List<decimal>> verts;
    // public List<List<int>> tInd;

    // public Mesh(List<List<decimal>> verts, List<List<int>> tInd)
    // {
    //     this.verts = verts;
    //     this.tInd = tInd;
    // }
    // public decimal getMax()
    // {
    //     decimal max = 0m;
    //     foreach (List<decimal> arr in this.verts)
    //     {
    //         if (arr.Max() > max)
    //         {
    //             max = arr.Max();
    //         }
    //     }
    //     return max;
    // }

}
