using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static float InverseLerp(float a, float b, float v)
    {
        return (v - a) / (b - a);
    }

    public static float LerpClamped(float a, float b, float t)
    {
        t = Mathf.Clamp(t, 0f, 1f);
        return Mathf.Lerp(a, b, t);
    }

    public static float ReMap(float oldMin, float oldMax, float newMin, float newMax, float v)
    {
        float t = InverseLerp(oldMin, oldMax, v);
        return Mathf.Lerp(newMin, newMax, v);
    }
}
