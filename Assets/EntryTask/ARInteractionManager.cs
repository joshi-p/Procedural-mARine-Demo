using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleARSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject spherePrefab;

    public Button cubeButton;
    public Button sphereButton;
    public Button deleteButton;

    public Material glowMaterial;

    public Color normalColor = Color.white;
    public Color activeColor = Color.green;

    // Hover settings
    public float hoverHeight = 0.02f;
    public float hoverSpeed = 1.5f;
    public float rotateSpeed = 20f;

    private ARRaycastManager raycastManager;

    private GameObject selectedObject;
    private GameObject glowObject;
    private GameObject currentPrefab;

    private bool canSpawn = false;

    // Track objects for hover
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Dictionary<GameObject, Vector3> startPositions = new Dictionary<GameObject, Vector3>();

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        cubeButton.image.color = normalColor;
        sphereButton.image.color = normalColor;

        deleteButton.gameObject.SetActive(false);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            HandleTap(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            HandleTap(Input.GetTouch(0).position);
        }
#endif

        // Glow pulse
        if (glowObject != null)
        {
            float scale = 1.1f + Mathf.Sin(Time.time * 5f) * 0.02f;
            glowObject.transform.localScale = Vector3.one * scale;
        }

        // Hover + rotation
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj == null) continue;

            Vector3 startPos = startPositions[obj];

            float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

            obj.transform.position = new Vector3(startPos.x, newY, startPos.z);

            obj.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    void HandleTap(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);

        // Selection
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Spawned"))
            {
                SelectObject(hit.collider.gameObject);
                return;
            }
        }

        // Spawn
        if (!canSpawn || currentPrefab == null) return;

        if (raycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;

            GameObject obj = Instantiate(currentPrefab);
            obj.tag = "Spawned";

            obj.transform.localScale = Vector3.one * 0.2f;

            Renderer rend = obj.GetComponentInChildren<Renderer>();
            float halfHeight = 0.1f;

            if (rend != null)
                halfHeight = rend.bounds.size.y / 2f;

            Vector3 finalPos = pose.position + Vector3.up * halfHeight;

            obj.transform.position = finalPos;
            obj.transform.rotation = pose.rotation;

            // Register for hover
            spawnedObjects.Add(obj);
            startPositions[obj] = finalPos;
        }
    }

    // BUTTONS

    public void ToggleCube()
    {
        if (currentPrefab == cubePrefab && canSpawn)
        {
            canSpawn = false;
            currentPrefab = null;
            cubeButton.image.color = normalColor;
        }
        else
        {
            canSpawn = true;
            currentPrefab = cubePrefab;

            cubeButton.image.color = activeColor;
            sphereButton.image.color = normalColor;
        }
    }

    public void ToggleSphere()
    {
        if (currentPrefab == spherePrefab && canSpawn)
        {
            canSpawn = false;
            currentPrefab = null;
            sphereButton.image.color = normalColor;
        }
        else
        {
            canSpawn = true;
            currentPrefab = spherePrefab;

            sphereButton.image.color = activeColor;
            cubeButton.image.color = normalColor;
        }
    }

    // SELECTION + GLOW

    void SelectObject(GameObject obj)
    {
        if (glowObject != null)
            Destroy(glowObject);

        selectedObject = obj;

        MeshFilter mf = obj.GetComponentInChildren<MeshFilter>();
        if (mf == null) return;

        glowObject = new GameObject("Glow");
        glowObject.transform.SetParent(obj.transform);
        glowObject.transform.localPosition = Vector3.zero;
        glowObject.transform.localRotation = Quaternion.identity;
        glowObject.transform.localScale = Vector3.one * 1.1f;

        MeshFilter glowMF = glowObject.AddComponent<MeshFilter>();
        glowMF.mesh = mf.mesh;

        MeshRenderer glowMR = glowObject.AddComponent<MeshRenderer>();
        glowMR.material = glowMaterial;

        deleteButton.gameObject.SetActive(true);
    }

    // DELETE

    public void DeleteSelected()
    {
        if (selectedObject != null)
        {
            spawnedObjects.Remove(selectedObject);
            startPositions.Remove(selectedObject);

            Destroy(selectedObject);
            selectedObject = null;

            if (glowObject != null)
                Destroy(glowObject);

            deleteButton.gameObject.SetActive(false);
        }
    }
}