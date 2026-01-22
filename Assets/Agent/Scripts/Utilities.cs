using UnityEngine;

public static class Utilities
{
    public static float Wrap(float v, float min, float max)
    {
        // ensure min <= max by swapping if needed
        if (min > max)
        {
            float temp = min;
            min = max;
            max = temp;
        }

        // handle case where range is zero
        if (min == max) return min;

        // calculate offset from min using modulo for wrapping
        float range = max - min;
        float offset = (v - min) % range;

        // handle negative offsets (when v < min)
        if (offset < 0) offset += range;

        return min + offset;
    }

    public static Vector3 Wrap(Vector3 v, Vector3 min, Vector3 max)
    {
        v.x = Wrap(v.x, min.x, max.x);
        v.y = Wrap(v.y, min.y, max.y);
        v.z = Wrap(v.z, min.z, max.z);

        return v;
    }
}

