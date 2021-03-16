/*
    This class stores the mesh object, which is directly deserialised in Worldscript.
    Defines a simple operation for getting the max displacement (helper to compute scaling)
*/
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class Mesh
{
    public List<List<decimal>> verts;
    public List<List<int>> tInd;

    public Mesh(List<List<decimal>> verts, List<List<int>> tInd)
    {
        this.verts = verts;
        this.tInd = tInd;
    }
    public decimal getMax()
    {
        decimal max = 0m;
        foreach (List<decimal> arr in this.verts)
        {
            if (arr.Max() > max)
            {
                max = arr.Max();
            }
        }
        return max;
    }

}
