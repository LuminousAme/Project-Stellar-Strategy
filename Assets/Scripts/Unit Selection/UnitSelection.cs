using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelection : MonoBehaviour
{
    public LayerMask unitLayer; // Layer mask for units
    public GameObject selectionBox; // Selection box UI element
    [SerializeField]
    private Faction playerFaction; //the player's faction, faction must match this to be a valid selection choice

    private Vector2 startPos; // Starting position of selection box
    private Vector2 currentPos; // curretn  position of selection box

    public List<GameObject> selectedUnits = new List<GameObject>(); // List of selected units

    [SerializeField]
    private CelestialBody sun;

    [SerializeField]
    private LayerMask planetLayer;

    Plane plane = new Plane(Vector3.down, 0f);

    private void Start()
    {
        selectionBox.SetActive(false);
    }

    private void Update()
    {
        if(selectionBox.activeSelf)
        {
            currentPos = Mouse.current.position.ReadValue();
            Vector2 boxStart = Vector2.Min(startPos, currentPos);
            Vector2 boxEnd = Vector2.Max(startPos, currentPos);
            Vector2 boxSize = boxEnd - boxStart;
            selectionBox.transform.position = boxStart + boxSize / 2;
            selectionBox.GetComponent<RectTransform>().sizeDelta = boxSize;
        }
    }

    public void SelectUnitAction(InputAction.CallbackContext context)
    {
		//to avoid annoying lag
		if (context.started)
		{
            startPos = Mouse.current.position.ReadValue();
		}

        if (context.performed)
        {
            selectionBox.SetActive(true);
        }

		//need to check if the hold went through
        if (context.canceled && selectionBox.activeInHierarchy)
        {
            selectionBox.SetActive(false);
            SelectUnits();
        }
    }

    public void SelectTarget(InputAction.CallbackContext context)
    {
        if (!context.performed)	return;

        //do some raycasts to determine what, if anything, the selected units should be moving towards
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        float dist = 0.0f;
        RaycastHit hit;
        bool planet = false;
        bool floor = false;
        CelestialBody targetCB = null;
        Vector3 position = Vector3.zero;


        //check if we hit a planet (other than the sun)
        if (Physics.Raycast(ray, out hit, float.MaxValue, planetLayer))
        {
            Collider col = hit.collider;
            Transform trans = col.transform;
            CelestialBody thisCB = trans.GetComponent<CelestialBody>();
            CelestialBody parentCB = trans.parent?.GetComponent<CelestialBody>();
            if (thisCB != null && thisCB != sun)
            {
                targetCB = thisCB;
                planet = true;
            }
            else if (parentCB != null && parentCB != sun)
            {
                targetCB = parentCB;
                planet = true;
            }
        }
        // If the camera is pointing somewhere on the floor
        else if (plane.Raycast(ray, out dist))
        {
            position = ray.GetPoint(dist);
            position.y = 0;
            floor = true;
        }

        //iterate over the selected units and updated their targets
        foreach (GameObject selected in selectedUnits)
        {
            ShipUnit unit = selected.GetComponent<ShipUnit>();

            if(planet) unit.SetFollowTarget(targetCB);
            else if (floor) unit.SetSeekTarget(position);
        }
    }

    private void SelectUnits()
    {
        // Get the corners of the selection box in screen space
        Vector2 boxStart = Vector2.Min(startPos, currentPos);
        Vector2 boxEnd = Vector2.Max(startPos, currentPos);

        // Cast a ray from each corner of the selection box to get all units within the box
        DeselectUnits();

		List<Unit> units = MatchManager.instance.stations[MatchManager.instance.playerFaction].factionUnits;

		//world to screen point
		Vector3 direction;
		foreach (Unit unit in units) {
			direction = unit.transform.position - Camera.main.transform.position;

			if (Vector3.Dot(direction, Camera.main.transform.forward) < 0)	continue;

			if (MathUtils.AABB(boxStart, boxEnd,
				Camera.main.WorldToScreenPoint(unit.transform.position)
				))
			{
				SelectUnit(unit);
			}
		}

        /*
		for (float x = boxStart.x; x < boxEnd.x; x += Mathf.Clamp(boxEnd.x / 2, 5, 10))
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

                    Debug.DrawRay(Camera.main.ScreenPointToRay(screenPos).origin, Camera.main.ScreenPointToRay(screenPos).direction * hit.distance, Color.yellow,
                        10);

                    Collider collider = hit.collider;
                    Unit unit = collider.gameObject.GetComponent<Unit>();
                    if (collider != null && !collider.isTrigger && collider.CompareTag("Unit") && unit != null && unit.GetFaction().SameFaction(playerFaction))
                    {
                        SelectUnit(unit);
                    }
                }
            }
        }
		*/
    }

    private void SelectUnit(Unit unit)
    {
		selectedUnits.Add(unit.gameObject);
        unit.Select();
        unit.OnUnitDestroyed += UnitDestroyed;
    }

    private void UnitDestroyed(Unit unit)
    {
        if(selectedUnits.Contains(unit.gameObject))
        {
            unit.OnUnitDestroyed -= UnitDestroyed;
            selectedUnits.Remove(unit.gameObject);
        }
    }

    private void DeselectUnits()
    {
        for(int i = 0; i < selectedUnits.Count; i++)
        {
            Unit unit = selectedUnits[i].GetComponent<Unit>();
            unit.OnUnitDestroyed -= UnitDestroyed;
            unit.Deleselect();
        }
        selectedUnits.Clear();
    }

}