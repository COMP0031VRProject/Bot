using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

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

}
