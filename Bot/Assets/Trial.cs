/*
    This class defines the Trial object, which is within a TestSuite.
*/
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class Trial
{
    public int id;
    public List<String> sequence;

    public List<int> getFlagSeq()
    {
        List<int> flag_seq = new List<int>();
        foreach (String s in this.sequence)
        {
            //Convert string to int
            if (s == "A")
            {
                flag_seq.Add(0);
            }
            else if (s == "B")
            {
                flag_seq.Add(1);
            }
            else if (s == "C")
            {
                flag_seq.Add(2);
            }
            else if (s == "D")
            {
                flag_seq.Add(3);
            }
            else if (s == "E")
            {
                flag_seq.Add(4);
            }
            else if (s == "F")
            {
                flag_seq.Add(5);
            }
        }
        return flag_seq;
    }

}
