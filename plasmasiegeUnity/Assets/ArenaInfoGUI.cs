using UnityEngine;

public class ArenaInfoGUI : MonoBehaviour
{
    [Header("Arena Information")]
    public bool showArenaInfo = true;
    public KeyCode toggleInfoKey = KeyCode.I;
    
    private ArenaGenerator arenaGenerator;
    private FlyingShipFixed shipController;
    private Rect infoRect = new Rect(Screen.width - 320, Screen.height - 250, 300, 200);
    
    void Start()
    {
        arenaGenerator = FindObjectOfType<ArenaGenerator>();
        shipController = FindObjectOfType<FlyingShipFixed>();
        
        Debug.Log("Arena Info GUI initialized");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(toggleInfoKey))
        {
            showArenaInfo = !showArenaInfo;
            Debug.Log($"Arena info GUI toggled: {showArenaInfo}");
        }
    }
    
    void OnGUI()
    {
        if (!showArenaInfo) return;
        
        infoRect = GUI.Window(1, infoRect, DrawArenaInfo, "Arena Information");
    }
    
    void DrawArenaInfo(int windowID)
    {
        GUILayout.BeginVertical();
        
        if (arenaGenerator != null)
        {
            GUILayout.Label("Arena Status:");
            GUILayout.Label($"Size: {arenaGenerator.arenaSize}");
            GUILayout.Label($"Asteroids: {arenaGenerator.asteroidCount}");
            
            GUILayout.Space(5);
            if (GUILayout.Button("Regenerate Arena (R)"))
            {
                arenaGenerator.RegenerateArena();
            }
        }
        else
        {
            GUILayout.Label("No Arena Generator found!");
        }
        
        // Show ship position relative to arena
        GameObject ship = GameObject.Find("FlyingShip");
        if (ship != null)
        {
            Vector3 pos = ship.transform.position;
            GUILayout.Space(10);
            GUILayout.Label("Ship Status:");
            GUILayout.Label($"Position: ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})");
            
            // Show distance from center
            float distanceFromCenter = Vector3.Distance(pos, Vector3.zero);
            GUILayout.Label($"Distance from center: {distanceFromCenter:F1}");
            
            // Show which quadrant
            string quadrant = GetQuadrant(pos);
            GUILayout.Label($"Quadrant: {quadrant}");
            
            // Distance to boundaries
            if (arenaGenerator != null)
            {
                Vector3 arenaSize = arenaGenerator.arenaSize;
                float distToEdge = Mathf.Min(
                    arenaSize.x / 2f - Mathf.Abs(pos.x),
                    arenaSize.y / 2f - Mathf.Abs(pos.y),
                    arenaSize.z / 2f - Mathf.Abs(pos.z)
                );
                GUILayout.Label($"Distance to boundary: {distToEdge:F1}");
                
                if (distToEdge < 10f)
                {
                    GUI.color = Color.red;
                    GUILayout.Label("WARNING: Near boundary!");
                    GUI.color = Color.white;
                }
            }
        }
        else
        {
            GUILayout.Label("No FlyingShip found!");
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Controls:");
        GUILayout.Label($"Press {toggleInfoKey} to toggle this info");
        GUILayout.Label("R - Regenerate arena");
        
        GUILayout.EndVertical();
        GUI.DragWindow();
    }
    
    string GetQuadrant(Vector3 position)
    {
        string vertical = position.y > 0 ? "Upper" : "Lower";
        string horizontal = "";
        
        if (position.x > 0 && position.z > 0) horizontal = "NorthEast";
        else if (position.x < 0 && position.z > 0) horizontal = "NorthWest";
        else if (position.x < 0 && position.z < 0) horizontal = "SouthWest";
        else if (position.x > 0 && position.z < 0) horizontal = "SouthEast";
        else if (position.x == 0 && position.z > 0) horizontal = "North";
        else if (position.x == 0 && position.z < 0) horizontal = "South";
        else if (position.z == 0 && position.x > 0) horizontal = "East";
        else if (position.z == 0 && position.x < 0) horizontal = "West";
        else horizontal = "Center";
        
        return $"{vertical} {horizontal}";
    }
} 