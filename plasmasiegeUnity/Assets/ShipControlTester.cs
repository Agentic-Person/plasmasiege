using UnityEngine;

public class ShipControlTester : MonoBehaviour
{
    public bool showGUI = true;
    public KeyCode toggleGUIKey = KeyCode.G;

    private FlyingShipFixed shipController;
    private Rect guiRect = new Rect(10, 10, 300, 600);

    void Start()
    {
        Debug.Log("CREATING SHIP CONTROL GUI");
        
        // Find the existing ship controller instead of creating a new ship
        shipController = FindObjectOfType<FlyingShipFixed>();
        if (shipController == null)
        {
            Debug.LogError("No FlyingShipFixed found in scene!");
            return;
        }
        
        // Load saved values from previous session
        LoadSettings();
    }
    
    void OnDestroy()
    {
        // Save current values when destroyed (game stops)
        SaveSettings();
    }
    
    void SaveSettings()
    {
        if (shipController == null) return;
        
        PlayerPrefs.SetFloat("Ship_ThrustPower", shipController.thrustPower);
        PlayerPrefs.SetFloat("Ship_MaxSpeed", shipController.maxSpeed);
        PlayerPrefs.SetFloat("Ship_RotationSpeed", shipController.rotationSpeed);
        PlayerPrefs.SetInt("Ship_InvertPitch", shipController.invertPitch ? 1 : 0);
        PlayerPrefs.SetInt("Ship_InvertYaw", shipController.invertYaw ? 1 : 0);
        PlayerPrefs.SetFloat("Ship_MaxRollAngle", shipController.maxRollAngle);
        PlayerPrefs.SetFloat("Ship_MouseConstraintRadius", shipController.mouseConstraintRadius);
        PlayerPrefs.SetFloat("Ship_CameraDistance", shipController.cameraDistance);
        PlayerPrefs.SetFloat("Ship_CameraHeight", shipController.cameraHeight);
        PlayerPrefs.SetFloat("Ship_CameraSmooth", shipController.cameraSmooth);
        PlayerPrefs.Save();
        
        Debug.Log("Ship settings saved!");
    }
    
    void LoadSettings()
    {
        if (shipController == null) return;
        
        // Only load if values exist (otherwise keep defaults)
        if (PlayerPrefs.HasKey("Ship_ThrustPower"))
        {
            shipController.thrustPower = PlayerPrefs.GetFloat("Ship_ThrustPower");
            shipController.maxSpeed = PlayerPrefs.GetFloat("Ship_MaxSpeed");
            shipController.rotationSpeed = PlayerPrefs.GetFloat("Ship_RotationSpeed");
            shipController.invertPitch = PlayerPrefs.GetInt("Ship_InvertPitch") == 1;
            shipController.invertYaw = PlayerPrefs.GetInt("Ship_InvertYaw") == 1;
            shipController.maxRollAngle = PlayerPrefs.GetFloat("Ship_MaxRollAngle");
            shipController.mouseConstraintRadius = PlayerPrefs.GetFloat("Ship_MouseConstraintRadius");
            shipController.cameraDistance = PlayerPrefs.GetFloat("Ship_CameraDistance");
            shipController.cameraHeight = PlayerPrefs.GetFloat("Ship_CameraHeight");
            shipController.cameraSmooth = PlayerPrefs.GetFloat("Ship_CameraSmooth");
            
            Debug.Log("Ship settings loaded from previous session!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleGUIKey))
        {
            showGUI = !showGUI;
        }
    }

    void OnGUI()
    {
        if (!showGUI) return;

        guiRect = GUI.Window(0, guiRect, DrawGUI, "Ship Control Tester");
    }

    void DrawGUI(int windowID)
    {
        if (shipController == null) 
        {
            GUILayout.Label("No ship controller found!");
            GUI.DragWindow();
            return;
        }

        GUILayout.BeginVertical();

        GUILayout.Label("Movement:");
        shipController.thrustPower = GUILayout.HorizontalSlider(shipController.thrustPower, 0f, 50f);
        GUILayout.Label($"Thrust Power: {shipController.thrustPower:F1}");

        shipController.maxSpeed = GUILayout.HorizontalSlider(shipController.maxSpeed, 1f, 100f);
        GUILayout.Label($"Max Speed: {shipController.maxSpeed:F1}");

        GUILayout.Space(10);
        GUILayout.Label("Rotation:");
        
        shipController.rotationSpeed = GUILayout.HorizontalSlider(shipController.rotationSpeed, 0.1f, 10f);
        GUILayout.Label($"Rotation Speed: {shipController.rotationSpeed:F1}");
        
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        shipController.invertPitch = GUILayout.Toggle(shipController.invertPitch, "Invert Pitch");
        GUILayout.FlexibleSpace();
        shipController.invertYaw = GUILayout.Toggle(shipController.invertYaw, "Invert Yaw");
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        shipController.maxRollAngle = GUILayout.HorizontalSlider(shipController.maxRollAngle, 0f, 90f);
        GUILayout.Label($"Max Roll Angle: {shipController.maxRollAngle:F0}°");
        
        shipController.mouseConstraintRadius = GUILayout.HorizontalSlider(shipController.mouseConstraintRadius, 50f, 500f);
        GUILayout.Label($"Mouse Constraint: {shipController.mouseConstraintRadius:F0}px");

        GUILayout.Space(10);
        GUILayout.Label("Camera:");
        
        shipController.cameraDistance = GUILayout.HorizontalSlider(shipController.cameraDistance, 1f, 30f);
        GUILayout.Label($"Camera Distance: {shipController.cameraDistance:F1}");

        shipController.cameraHeight = GUILayout.HorizontalSlider(shipController.cameraHeight, -5f, 20f);
        GUILayout.Label($"Camera Height: {shipController.cameraHeight:F1}");

        shipController.cameraSmooth = GUILayout.HorizontalSlider(shipController.cameraSmooth, 0.1f, 10f);
        GUILayout.Label($"Camera Smooth: {shipController.cameraSmooth:F1}");

        GUILayout.Space(10);
        GUILayout.Label("Status:");
        // Access rigidbody through the ship GameObject instead
        GameObject ship = GameObject.Find("FlyingShip");
        if (ship != null)
        {
            Rigidbody rb = ship.GetComponent<Rigidbody>();
            if (rb != null)
            {
                GUILayout.Label($"Velocity: {rb.linearVelocity.magnitude:F1}");
                GUILayout.Label($"Angular Vel: {rb.angularVelocity.magnitude:F1}");
                GUILayout.Label($"Position: {ship.transform.position}");
            }
        }
        
        GUILayout.Space(10);
        GUILayout.Label($"Press {toggleGUIKey} to toggle GUI");
        GUILayout.Label("W/S: Forward/Back, Mouse: Look");
        GUILayout.Label("A/D: Roll Left/Right");
        GUILayout.Label("Space/Shift: Up/Down");
        GUILayout.Label("Tab: Debug info");
        
        GUILayout.Space(5);
        if (GUILayout.Button("Save Settings"))
        {
            SaveSettings();
        }

        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}