using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float panSpeed = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    
    [Header("Boundaries")]
    public bool useBoundaries = true;
    public float boundaryPadding = 1f;
    public Vector2 mapSize = new Vector2(10f, 10f);
    
    private Camera cam;
    private Vector3 lastMousePosition;
    private bool isPanning = false;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        // Set initial position and zoom
        transform.position = new Vector3(0f, 0f, -10f);
        cam.orthographicSize = 5f;
    }
    
    void Update()
    {
        HandleInput();
        ClampPosition();
    }
    
    void HandleInput()
    {
        // Mouse panning
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            isPanning = false;
        }
        
        if (isPanning)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * panSpeed * Time.deltaTime;
            transform.Translate(move);
            lastMousePosition = Input.mousePosition;
        }
        
        // Keyboard panning
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (horizontal != 0f || vertical != 0f)
        {
            Vector3 move = new Vector3(horizontal, vertical, 0f) * panSpeed * Time.deltaTime;
            transform.Translate(move);
        }
        
        // Zoom with mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        
        // Zoom with +/- keys
        if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
        {
            float newSize = cam.orthographicSize - zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
        {
            float newSize = cam.orthographicSize + zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        
        // Reset camera position
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCamera();
        }
    }
    
    void ClampPosition()
    {
        if (!useBoundaries) return;
        
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        
        float minX = -mapSize.x / 2f + halfWidth - boundaryPadding;
        float maxX = mapSize.x / 2f - halfWidth + boundaryPadding;
        float minY = -mapSize.y / 2f + halfHeight - boundaryPadding;
        float maxY = mapSize.y / 2f - halfHeight + boundaryPadding;
        
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        
        transform.position = clampedPosition;
    }
    
    public void ResetCamera()
    {
        transform.position = new Vector3(0f, 0f, -10f);
        cam.orthographicSize = 5f;
    }
    
    public void FocusOnTerritory(Territory territory)
    {
        if (territory == null) return;
        
        Vector3 targetPosition = territory.transform.position;
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;
    }
    
    public void SetMapSize(Vector2 size)
    {
        mapSize = size;
    }
    
    void OnDrawGizmos()
    {
        if (useBoundaries)
        {
            // Draw map boundaries
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x, mapSize.y, 0.1f));
        }
    }
}
