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

    public static int WrapIndex(int index, int maxExclusive)
    {
        if (index < 0)
        {
            index *= -1;
            return maxExclusive - index;
        }

        if (index > maxExclusive - 1)
        {
            index -= maxExclusive;
        }

        return index;
    }

	public static bool AABB(Vector2 lb, Vector2 ub, Vector2 point) {
		return (lb.x < point.x) && (ub.x > point.x) &&
				(lb.y < point.y) && (ub.y > point.y);
	}
}
