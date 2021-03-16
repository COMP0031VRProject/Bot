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

}
