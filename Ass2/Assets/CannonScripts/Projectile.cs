using UnityEngine;

public class Projectile : MonoBehaviour
{
    //variables for kinematics
    public Vector3 initialVelocity;   // vi (updated each frame to vf)
    public Vector3 finalVelocity;     // vf (current velocity)
    public Vector3 acceleration;      // (0, -g, 0)
    public Vector3 displacement;      // frame displacement
    public float TimeStep = 0.1f;     // optional: for external samplers

    // variables for firing and life management
    [SerializeField] private Vector3 position;
    [SerializeField] private float timeToLive = 10f;
    private float lifeTimer;
    private bool fired;


    public void Fire(Vector3 origin, Vector3 v0, Vector3 a)    // function for firing the projectile
    {
        position = origin;
        initialVelocity = v0;
        finalVelocity = v0;
        acceleration = a;

        transform.position = origin;
        fired = true;
        lifeTimer = 0f;
    }

    public void FireSolveForV0(Vector3 origin, Vector3 target, float T, float gravity) // kinematics solver for hitting target in time T
    {
        Vector2 a = new Vector2(0f, -Mathf.Abs(gravity));
        Vector2 d = (Vector2)target - (Vector2)origin;
        Vector2 v0 = Kinematics2D.SolveV0GivenTime(d, a, T);
        Fire(origin, new Vector3(v0.x, v0.y, 0f), new Vector3(a.x, a.y, 0f));
    }


    private void Update()
    {
        if (!fired) return;

        float dt = Time.deltaTime;

        //1st kinematic equation
        // d = vi*dt + 1/2*a*dt^2
        Vector2 d2 = Kinematics2D.Position(Vector2.zero, (Vector2)initialVelocity, (Vector2)acceleration, dt);
        displacement = new Vector3(d2.x, d2.y, 0f);

        position += displacement;
        transform.position = new Vector3(position.x, position.y, 0f);

        //2nd kinematic equation
        // vf = vi + a*dt
        Vector2 vf = Kinematics2D.Velocity((Vector2)initialVelocity, (Vector2)acceleration, dt);
        finalVelocity = new Vector3(vf.x, vf.y, 0f);
        initialVelocity = finalVelocity;

        lifeTimer += dt;
        if (timeToLive > 0f && lifeTimer >= timeToLive)
            Destroy(gameObject);
    }
}
