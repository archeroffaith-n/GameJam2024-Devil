using System.Runtime.InteropServices;
using UnityEngine;

public class RandomUtils
{
    static public float NextGaussian() 
    {
        float v1, v2, s;
        do 
        {
            v1 = 2.0f * UnityEngine.Random.Range(0.0f, 1.0f) - 1.0f;
            v2 = 2.0f * UnityEngine.Random.Range(0.0f, 1.0f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0.0f);
        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

        return v1 * s;
    }

    static public float TruncatedGaussian(float min, float max, float? center_opt, float curvature=1.0f, float baseline=0.0f)
    {
        if (min == max) {
            return min;
        }
        float center;
        if (!center_opt.HasValue) {
            center = (max + min) / 2.0f;
        } else {
            center = (float)center_opt;
        }

        float left = 2 * (center - min) * curvature / (max - min);
        float right = 2 * (max - center) * curvature / (max - min);
        float res;

        if (UnityEngine.Random.Range(0.0f, 1.0f) <= baseline) {
            res = UnityEngine.Random.Range(-left, right);
        } else {
            res = NextGaussian();
            while ((res < -left) || (res > right)) {
                res = NextGaussian();
                
            }
        }

        res = (res + left) / (left + right) * (max - min) + min;

        return res;
    }
}
