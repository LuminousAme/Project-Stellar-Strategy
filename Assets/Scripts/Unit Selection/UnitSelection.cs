using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public LayerMask unitLayer; // Layer mask for units
    public GameObject selectionBox; // Selection box UI element

    private Vector2 startPos; // Starting position of selection box
    private Vector2 currentPos; // curretn  position of selection box

    public List<GameObject> selectedUnits = new List<GameObject>(); // List of selected units

    //replace this with a better way of indentifiying selected units soon
    public Material highlightMaterial; // Material for highlighting selected units

    Plane plane = new Plane(Vector3.down, 0f);

    private void Start()
    {
        selectionBox.SetActive(false);
    }


    private void Update()
    {
        // Check for left mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            selectionBox.SetActive(true);
        }

        // Update selection box position and size
        if (Input.GetMouseButton(0))
        {
            currentPos = Input.mousePosition;
            Vector2 boxStart = Vector2.Min(startPos, currentPos);
            Vector2 boxEnd = Vector2.Max(startPos, currentPos);
            Vector2 boxSize = boxEnd - boxStart;
            selectionBox.transform.position = boxStart + boxSize / 2;
            selectionBox.GetComponent<RectTransform>().sizeDelta = boxSize;
        }

        // Check for left mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            selectionBox.SetActive(false);
            SelectUnits();
        }

        //right click to move 
        if (Input.GetMouseButtonDown(1))
        {
            foreach (GameObject selected in selectedUnits)
            {
                Unit unit = selected.GetComponent<Unit>();

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float dist = 0.0f;
                
                // If the camera is pointing somewhere on the floor
                if (plane.Raycast(ray, out dist))
                {
                    Vector3 position = ray.GetPoint(dist);
                    position.y = 0;
                    unit.SetSeekTarget(position);
                }
            }
        }
    }

    private void SelectUnits()
    {
        // Get the corners of the selection box in screen space
        Vector2 boxStart = Vector2.Min(startPos, currentPos);
        Vector2 boxEnd = Vector2.Max(startPos, currentPos);

        // Cast a ray from each corner of the selection box to get all units within the box
        selectedUnits.Clear();
        for (float x = boxStart.x; x < boxEnd.x; x += Mathf.Clamp(boxEnd.x/2, 5,10))
        {
            for (float y = boxStart.y; y < boxEnd.y; y += Mathf.Clamp(boxEnd.y / 2, 5, 10))
            {
                Vector2 screenPos = new Vector2(x, y);
                Ray ray = Camera.main.ScreenPointToRay(screenPos);
                RaycastHit[] hits = Physics.RaycastAll(ray, Camera.main.farClipPlane, unitLayer);

                // Filter hits to only include game objects with collider and "Unit" tag
                foreach (RaycastHit hit in hits)
                {
                    //look at ray in editor

                    Debug.DrawRay(Camera.main.ScreenPointToRay(screenPos).origin,Camera.main.ScreenPointToRay(screenPos).direction * hit.distance, Color.yellow,
                        10);

                    Collider collider = hit.collider;
                    if (collider != null && collider.CompareTag("Unit"))
                    {
                        selectedUnits.Add(collider.gameObject);
                        HighlightUnit(collider.gameObject);
                    }
                }
            }
        }
    }

    private void HighlightUnit(GameObject unit)
    {
        // Add highlight material to unit's renderer
        Renderer renderer = unit.GetComponent<Renderer>();
        Material[] materials = renderer.materials;
        Material[] newMaterials = new Material[materials.Length + 1];
        materials.CopyTo(newMaterials, 0);
        newMaterials[materials.Length] = highlightMaterial;
        renderer.materials = newMaterials;
    }

}