using UnityEngine;

public static class Kinematics2D
{
    ///p(t) = p0 + v0*t + 1/2*a*t^2
    public static Vector2 Position(Vector2 p0, Vector2 v0, Vector2 a, float t)
        => p0 + v0 * t + 0.5f * a * t * t;

    ///v(t) = v0 + a*t
    public static Vector2 Velocity(Vector2 v0, Vector2 a, float t)
        => v0 + a * t;

    ///Solve v0 to hit displacement d in time T with constant acceleration a:
    /// v0 = (d - 1/2*a*T^2) / T
    public static Vector2 SolveV0GivenTime(Vector2 d, Vector2 a, float T)
        => (d - 0.5f * a * T * T) / Mathf.Max(0.0001f, T);


    ///Solve smallest positive t where y(t) = yTarget for p0,v0,a (quadratic). Returns null if no real positive root
    public static float? SolveTimeToY(float p0y, float v0y, float ay, float yTarget)
    {
        // 0.5*ay*t^2 + v0y*t + (p0y - yTarget) = 0
        float A = 0.5f * ay;
        float B = v0y;
        float C = p0y - yTarget;

        // Handle nearly-zero acceleration (straight line in y)
        if (Mathf.Abs(A) < 1e-6f)
        {
            if (Mathf.Abs(B) < 1e-6f) return null;           // no motion in y
            float t = -C / B;
            return t > 0f ? t : (float?)null;
        }

        float disc = B * B - 4f * A * C;
        if (disc < 0f) return null;                          // no real roots

        float sqrt = Mathf.Sqrt(disc);
        float t1 = (-B - sqrt) / (2f * A);
        float t2 = (-B + sqrt) / (2f * A);

        float tMin = float.PositiveInfinity;
        if (t1 > 0f) tMin = Mathf.Min(tMin, t1);
        if (t2 > 0f) tMin = Mathf.Min(tMin, t2);

        return float.IsInfinity(tMin) ? (float?)null : tMin;
    }

    ///position at time t along X only (ax=0 assumed).
    public static float PositionX(float p0x, float v0x, float t) => p0x + v0x * t;
}
