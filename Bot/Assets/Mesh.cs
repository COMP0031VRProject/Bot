using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh
{
    public List<(float, float)> verts;
    public List<(int, int, int)> tInd;
    public Mesh(List<(float, float)> verts, List<(int, int, int)> tInd)
    {
        this.verts = verts;
        this.tInd = tInd;
    }
}
