using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//based on the procedural planet generation series by Sebastian Lague 
//https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public class MinMaxTracker
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public MinMaxTracker()
    {
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    public void AddValue(float v)
    {
        if (v > Max) Max = v;
        if (v < Min) Min = v;
    }

    public Vector2 ToVec2()
    {
        return new Vector2(Min, Max);
    }
}
