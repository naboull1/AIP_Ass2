using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))] // Ensure there is a LineRenderer component
public class PathLine : MonoBehaviour
{
    public enum MuzzleAxis2D { Right, Up, Custom }

    //public fields for the inspector, to setup startpointpojt, mode, flight time, gravity, etc.
    public Transform startPoint;
    public bool aimAtMouse = true;
    public float flightTime = 1.0f;
    public float gravity = 9.81f;

    //Muzzle direction,speed and initial strength settings
    public MuzzleAxis2D muzzleAxis = MuzzleAxis2D.Right;
    public Vector2 customLocalAxis = new Vector2(1f, 0f);
    public bool useManualVelocity = false;       // when true, use manualInitialVelocity
    public Vector3 manualInitialVelocity = new Vector3(10f, 10f, 0f);
    public float previewSpeed = 12f;             // used only if useManualVelocity == false

    // LineRenderer settings
    [Min(2)] public int resolution = 200;
    public float predictionTime = 2.0f;
    private LineRenderer lr;

    void Awake() // Initialize and configure LineRendererc component
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        if (lr.widthMultiplier == 0f) lr.widthMultiplier = 0.04f;
        lr.enabled = true;

        if (startPoint == null) startPoint = transform;


        if (lr.sharedMaterial == null)
        {
            var shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Unlit"); // URP fallback
            }
            if (shader != null)
            {
                lr.sharedMaterial = new Material(shader);
            }
        }

    }

    void Update()
    {
        if (!lr.enabled) return;
        if (startPoint == null || Camera.main == null) return;

        // gets Initial position and acceleration into variables
        Vector2 p0 = startPoint.position;
        Vector2 a = new Vector2(0f, -Mathf.Abs(gravity));

        // Decide v0 for current mode
        Vector2 v0;
        if (aimAtMouse)
        {
            // If in point-and-click mode, compute v0 to hit mouse position in flightTime seconds
            Vector2 mouse = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
            float zDist = 0f - Camera.main.transform.position.z;
            Vector3 target3 = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, zDist));
            target3.z = 0f;

            float T = Mathf.Max(0.01f, flightTime);
            Vector2 d = (Vector2)target3 - p0;
            v0 = Kinematics2D.SolveV0GivenTime(d, a, T);
        }
        else // if not in point-and-click mode then must be in angle and strength mode, compute velocity and speed with direction
        {
            if (useManualVelocity)
            {
                v0 = new Vector2(manualInitialVelocity.x, manualInitialVelocity.y);
            }
            else
            {
                Vector2 dir = GetMuzzleDir2D();
                v0 = dir * Mathf.Max(0f, previewSpeed);
            }
        }
        //always updating the value from the formula
        // Draw p(t) = p0 + v0 t + 1/2 a t^2
        int n = Mathf.Max(2, resolution);
        lr.positionCount = n;

        float total = Mathf.Max(0.01f, predictionTime);
        float dt = total / (n - 1);

        for (int i = 0; i < n; i++)
        {
            float t = dt * i;
            Vector2 p = Kinematics2D.Position(p0, v0, a, t);
            lr.SetPosition(i, new Vector3(p.x, p.y, 0f));
        }
    }

    public void SetDrawingEnabled(bool enabled) // Enable the LineRenderer
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        lr.enabled = enabled;
    }

    private Vector2 GetMuzzleDir2D() // Function to get the direction of the muzzle in 2D space
    {
        Vector2 localAxis;
        switch (muzzleAxis)
        {
            case MuzzleAxis2D.Up: localAxis = Vector2.up; break;
            case MuzzleAxis2D.Custom: localAxis = (customLocalAxis.sqrMagnitude > 0.0001f) ? customLocalAxis.normalized : Vector2.right; break;
            default: localAxis = Vector2.right; break;
        }
        Vector3 world = startPoint.rotation * new Vector3(localAxis.x, localAxis.y, 0f);
        world.z = 0f;
        return ((Vector2)world).normalized;
    }


}
