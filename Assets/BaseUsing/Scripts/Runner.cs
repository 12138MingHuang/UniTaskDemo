using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Runner
{
    public Transform Target;
    public float Speed;
    public Vector3 StartPos;
    public bool ReachGoal = false;

    public void Reset()
    {
        ReachGoal = false;
        Target.position = StartPos;
    }
}
