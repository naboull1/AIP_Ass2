using UnityEngine;
using UnityEngine.InputSystem;

public class CannonControl : MonoBehaviour
{
    //An Enum to define the firing mode one for click on point firing and the other for angle and strength firing
    public enum FireMode { ClickToTarget = 1, AngleStrength = 2 }
    //An Enum for rotating the muzzle 
    public enum MuzzleAxis2D { Right, Up, Custom }

    //Assigning spawn point for projectiles
    [SerializeField] private Transform muzzle;   // where shots spawn (barrel tip)

    //Assigning prefabs for projectiles,target points and pathlines
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject targetPointPrefab;
    [SerializeField] private PathLine pathLine;  // drag your single PathLine here

    //Gravity float variable
    [SerializeField] private float gravity = 9.81f;

    //Variable for mode 1 click to target
    [SerializeField] private float flightTimeClick = 1.0f;

    //Variables for mode 2 angle and strength
    [SerializeField] private float launchSpeed = 12f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 60f;
    [SerializeField] private float speedAdjustPerSecond = 20f;
    [SerializeField] private float rotationSpeed = 100f;


    [SerializeField] private float predictionTime = 2.0f;

    //Muzzle orientation for mode 2
    [SerializeField] private MuzzleAxis2D muzzleAxis = MuzzleAxis2D.Up;
    [SerializeField] private Vector2 customLocalAxis = new Vector2(1f, 0f);

    //Input actions for mouse and keyboard
    [SerializeField] private InputActionAsset inputActions;
    public InputAction keyAction;       // spacebar 
    private InputAction clickAction;    // Left Button
    private InputAction positionAction; // MousePosition

    public float Gravity => gravity;
    public float FlightTimeClick => flightTimeClick;

    private FireMode mode = FireMode.ClickToTarget;



    private void OnEnable() //Function to enable input actions and configure path line
    {
        if (keyAction != null) { keyAction.Enable(); }

        clickAction = inputActions.FindAction("Left Button");
        positionAction = inputActions.FindAction("MousePosition");

        clickAction?.Enable();
        positionAction?.Enable();
        if (clickAction != null) clickAction.performed += OnMouseClick;

        ConfigurePathLineForCurrentMode();
        //EnsureOnlyThisPathLineDraws();
    }

    private void OnDisable() //Function to disable input actions
    {
        if (keyAction != null) keyAction.Disable();
        if (clickAction != null) { clickAction.performed -= OnMouseClick; clickAction.Disable(); }
        positionAction?.Disable();
    }

    private void OnMouseClick(InputAction.CallbackContext ctx) //Function to handle mouse click events and fire projectiles
    {
        if (muzzle == null) { Debug.LogWarning("You forgot to add a prefab to muzzle slot"); return; }
        Vector2 origin = muzzle.position;

        if (mode == FireMode.ClickToTarget)// the fedault mode is click to target which is option 1
        {
            if (positionAction == null) return;

            //gets the mouse position in world space using mouse x, mouse y and camera z distance to 0
            Vector2 mouseScreen = positionAction.ReadValue<Vector2>();
            float zDist = 0f - Camera.main.transform.position.z;
            Vector3 world = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, zDist));
            world.z = 0f;
            Vector2 target = world;

            if (targetPointPrefab) Instantiate(targetPointPrefab, world, Quaternion.identity);

            //if projectile successful instantiated fire projectile using SolveForV0 method
            var go = Instantiate(projectilePrefab, origin, Quaternion.identity);
            DisableAnyPathLines(go);
            var proj = go.GetComponent<Projectile>() ?? go.AddComponent<Projectile>();
            float T = Mathf.Max(0.01f, flightTimeClick);
            proj.FireSolveForV0(origin, target, T, gravity);
        }
        else // if mode is not is not 'click to target' then it must be angle and strength mode, which is option 2
        {
            //get the muzzle direction and calculate initial velocity and acceleration
            Vector2 dir = GetMuzzleDir2D();
            float speed = Mathf.Clamp(launchSpeed, minSpeed, maxSpeed);
            Vector2 v0 = dir * speed;
            Vector3 a = new Vector3(0f, -Mathf.Abs(gravity), 0f);

            //instantiate projectile and fire using the calculated v0 and acceleration
            var go = Instantiate(projectilePrefab, origin, Quaternion.identity);
            DisableAnyPathLines(go);
            var proj = go.GetComponent<Projectile>() ?? go.AddComponent<Projectile>();
            proj.Fire(origin, new Vector3(v0.x, v0.y, 0f), a);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            //if 1 key pressed switch to click to target mode and configure path line to match mouse position
            mode = FireMode.ClickToTarget;
            ConfigurePathLineForCurrentMode();
            EnsureOnlyThisPathLineDraws();
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            //if 2 key pressed switch to angle and strength mode and configure path line to match muzzle direction and strength
            mode = FireMode.AngleStrength;
            ConfigurePathLineForCurrentMode();
            EnsureOnlyThisPathLineDraws();
        }

        // Rotates the Cannons turret with W/S keys
        float rot = 0f;
        if (Keyboard.current.wKey.isPressed) rot += 1f;
        if (Keyboard.current.sKey.isPressed) rot -= 1f;
        if (rot != 0f) transform.Rotate(Vector3.forward, -rot * rotationSpeed * Time.deltaTime);

        // When in angle and strength mode, A/D keys increase/decrease launch speed
        if (mode == FireMode.AngleStrength)
        {
            float dSpeed = 0f;
            if (Keyboard.current.dKey.isPressed) dSpeed += 1f;
            if (Keyboard.current.aKey.isPressed) dSpeed -= 1f;
            if (dSpeed != 0f)
            launchSpeed = Mathf.Clamp(launchSpeed + dSpeed * speedAdjustPerSecond * Time.deltaTime, minSpeed, maxSpeed);
        }

        UpdatePathLinePreview(); // always keeps the path line updated running that function every frame
    }


    private void ConfigurePathLineForCurrentMode() //function to configure path line based on current mode, it organises the variables and values needed for each mode
    {
        if (pathLine == null) return;
        //setting up variable values for path line
        pathLine.startPoint = muzzle != null ? muzzle : transform;
        pathLine.gravity = gravity;
        pathLine.predictionTime = predictionTime;

        if (mode == FireMode.ClickToTarget)// when in click to target mode, path line aims at mouse and flight time is set
        {
            pathLine.aimAtMouse = true;
            pathLine.flightTime = flightTimeClick;
            pathLine.useManualVelocity = false;
            pathLine.SetDrawingEnabled(true);
        }
        else // when in angle and strength mode, path line uses muzzle direction and manual velocity
        {
            pathLine.aimAtMouse = false;
            pathLine.useManualVelocity = true;
            pathLine.muzzleAxis = (PathLine.MuzzleAxis2D)muzzleAxis;
            pathLine.customLocalAxis = customLocalAxis;
            pathLine.SetDrawingEnabled(true);
        }
    }

    private void UpdatePathLinePreview()// function that is used in update to keep path line updated every frame
    {
        if (pathLine == null || muzzle == null) return;

        if (mode == FireMode.AngleStrength)
        {
            //if in angle and strength mode, update to match muzzle direction and launch speed
            Vector2 dir = GetMuzzleDir2D();
            float speed = Mathf.Clamp(launchSpeed, minSpeed, maxSpeed);
            Vector3 v0 = new Vector3(dir.x * speed, dir.y * speed, 0f);

            pathLine.useManualVelocity = true;
            pathLine.aimAtMouse = false;
            pathLine.manualInitialVelocity = v0;
            pathLine.startPoint = muzzle;
        }
        else // if in click to target mode, update to match mouse position and flight time
        {
            pathLine.useManualVelocity = false;
            pathLine.aimAtMouse = true;
            pathLine.flightTime = flightTimeClick;
            pathLine.startPoint = muzzle;
        }
    }

    private void EnsureOnlyThisPathLineDraws() // sets the path line to be based on the mode and disables any other path lines in the scene
    {
        if (pathLine == null) return;
        var all = FindObjectsOfType<PathLine>(true);
        foreach (var pl in all)
            pl.SetDrawingEnabled(true);
    }

    private void DisableAnyPathLines(GameObject root) //Disables any path lines in the instantiated projectile to avoid nested path lines
    {
        var pls = root.GetComponentsInChildren<PathLine>(true);
        foreach (var pl in pls)
            pl.SetDrawingEnabled(false);
    }

  
    public FireMode CurrentMode => mode; // Public info for sending to HUD script
    public void SetMode(FireMode m) // Public info for sending to HUD script
    {
        mode = m;
        ConfigurePathLineForCurrentMode();
        EnsureOnlyThisPathLineDraws();
    }

    public float LaunchSpeed // Public info for sending to HUD script
    {
        get => launchSpeed;
        set => launchSpeed = Mathf.Clamp(value, minSpeed, maxSpeed);
    }
    public float MinSpeed => minSpeed; // Public info for sending to HUD script
    public float MaxSpeed => maxSpeed; // Public info for sending to HUD script
    public Transform Muzzle => muzzle; // Public info for sending to HUD script
    public MuzzleAxis2D MuzzleAxis => muzzleAxis; // Public info for sending to HUD script



    // Single (public) version used by both internal code and HUD
    public Vector2 GetMuzzleDir2D()
    {
        Vector2 localAxis;
        switch (muzzleAxis)
        {
            case MuzzleAxis2D.Up: localAxis = Vector2.up; break;
            case MuzzleAxis2D.Custom: localAxis = (customLocalAxis.sqrMagnitude > 0.0001f) ? customLocalAxis.normalized : Vector2.right; break;
            default: localAxis = Vector2.right; break;
        }
        Vector3 world = muzzle.rotation * new Vector3(localAxis.x, localAxis.y, 0f);
        world.z = 0f;
        return ((Vector2)world).normalized;
    }
}
