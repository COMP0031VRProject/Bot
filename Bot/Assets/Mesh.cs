using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh
{
    public List<(decimal, decimal)> verts;
    public List<(int, int, int)> tInd;
    public Mesh(List<(decimal, decimal)> verts, List<(int, int, int)> tInd)
    {
        this.verts = verts;
        this.tInd = tInd;
    }
}
