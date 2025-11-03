using UnityEngine;
using UnityEngine.InputSystem;

public class CannonHUD : MonoBehaviour
{
    [SerializeField] private CannonControl cannon;  // reference to the cannon scriipt to get control data
    [SerializeField] private float groundY = 0f;
    [SerializeField] private bool showMouseAsTargetInMode1 = true;

    // UI layout
    private Rect panel = new Rect(12, 12, 360, 260);
    private GUIStyle titleStyle, labelStyle;
    private bool stylesReady;

    private void OnGUI()
    {
        if (cannon == null) return;

        
        if (!stylesReady)// one-time setup of GUI styles
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold };
            labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 13 };
            stylesReady = true;
        }

        panel = GUI.Window(42, panel, DrawPanel, "Cannon HUD");
    }

    private void DrawPanel(int id)
    {
        GUILayout.Space(6);
        GUILayout.Label("Ballistic Controls", titleStyle);

        // Mode buttons for 1) Click to Target, 2) Angle + Strength
        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(cannon.CurrentMode == CannonControl.FireMode.ClickToTarget, "1) Click → Target", "Button"))
            cannon.SetMode(CannonControl.FireMode.ClickToTarget);
        if (GUILayout.Toggle(cannon.CurrentMode == CannonControl.FireMode.AngleStrength, "2) Angle + Strength", "Button"))
            cannon.SetMode(CannonControl.FireMode.AngleStrength);
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        // Gets the current angle of the cannon muzzle in degrees from the cannon script
        Vector2 dir = cannon.GetMuzzleDir2D();
        float angleDeg = Mathf.Round(Vector2.SignedAngle(Vector2.right, dir));
        GUILayout.Label($"Angle: {angleDeg:0}°    Axis: {cannon.MuzzleAxis}", labelStyle);

        // strength slider only in mode 2
        if (cannon.CurrentMode == CannonControl.FireMode.AngleStrength)
        {
            float s = cannon.LaunchSpeed;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Strength:", GUILayout.Width(70));
            s = GUILayout.HorizontalSlider(s, cannon.MinSpeed, cannon.MaxSpeed, GUILayout.Width(200));
            s = Mathf.Round(s * 10f) / 10f;
            GUILayout.Label($"{s:0.0}", GUILayout.Width(50));
            GUILayout.EndHorizontal();
            if (!Mathf.Approximately(s, cannon.LaunchSpeed)) cannon.LaunchSpeed = s;
        }
        else// if not in mode 2 then text for click-to-target mode
        {
            GUILayout.Label("Click anywhere to set the target point.", labelStyle);
        }

        GUILayout.Space(8);

        // these arent 100% accurate but close enough for demo purposes, gets  the information from cannon script and kinematics script
        Vector2 p0 = cannon.Muzzle.position;
        Vector2 a = new Vector2(0f, -Mathf.Abs(cannon.Gravity));
        Vector2 v0;
        float? tTarget = null;
        Vector2 targetPoint = Vector2.zero;

        if (cannon.CurrentMode == CannonControl.FireMode.ClickToTarget)
        {

            float T = Mathf.Max(0.01f, cannon.FlightTimeClick);
            Vector2 mouse = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
            float zDist = 0f - Camera.main.transform.position.z;
            Vector3 target3 = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, zDist));
            target3.z = 0f;
            targetPoint = target3;

            Vector2 d = (Vector2)target3 - p0;
            v0 = Kinematics2D.SolveV0GivenTime(d, a, T);
            tTarget = T;
        }
        else
        {
            v0 = cannon.GetMuzzleDir2D() * cannon.LaunchSpeed;
        }

        float? tImpact = Kinematics2D.SolveTimeToY(p0.y, v0.y, a.y, groundY);
        Vector2 impactPoint = Vector2.zero;
        if (tImpact.HasValue)
        {
            float t = tImpact.Value;
            float x = Kinematics2D.PositionX(p0.x, v0.x, t);
            impactPoint = new Vector2(x, groundY);
        }

        GUILayout.Label("Predictions", titleStyle);
        if (cannon.CurrentMode == CannonControl.FireMode.ClickToTarget && showMouseAsTargetInMode1)
            GUILayout.Label($"Target (mouse) @ T = {tTarget:F2}s  →  ({targetPoint.x:F2}, {targetPoint.y:F2})", labelStyle);

        GUILayout.Label(tImpact.HasValue
            ? $"Ground impact in {tImpact.Value:F2}s  →  ({impactPoint.x:F2}, {impactPoint.y:F2})"
            : "Ground impact: (no real solution at current angle/strength)",
            labelStyle);

        GUILayout.Space(6);
        GUILayout.Label("Shortcuts: 1/2 switch modes • W/S rotate • A/D strength • LMB fire", labelStyle);

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}
