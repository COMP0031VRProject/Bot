using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh
{
    public List<(double, double)> verts;
    public List<(int, int, int)> tInd;
    public Mesh(List<(double, double)> verts, List<(int, int, int)> tInd)
    {
        this.verts = verts;
        this.tInd = tInd;
    }
}
