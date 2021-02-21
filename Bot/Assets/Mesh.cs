using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh
{
    public object verts;
    public object tInd;
    public Mesh(object verts, object tInd)
    {
        this.verts = verts;
        this.tInd = tInd;
    }


}
