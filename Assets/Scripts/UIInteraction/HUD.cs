using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public List<HUDElement> elements = new List<HUDElement>();
    public GameObject unitList, unitButtonPrefab;
    public TMP_Text resourcesText;
    public Button buildDestroyerButton, buildExtractorButton;
    public UnitSelection unitselector;
    private StationUnit playerStation;
    private Dictionary<Unit, GameObject> unitButtonMap = new Dictionary<Unit, GameObject>();
	[SerializeField]	UnityEngine.InputSystem.InputAction selectLockOn;

    private void Start()
    {
        for (int i = 0; i < elements.Count; i++) elements[i].Start();
        StartCoroutine(FirstFrame());
    }

	private void OnEnable() {
		selectLockOn.Enable();
	}

	private void OnDisable() {
		selectLockOn.Disable();
	}

    private void Update()
    {
        for (int i = 0; i < elements.Count; i++) elements[i].Update();

        if(playerStation)
        {
            if (playerStation.GetResources() >= 1000.0f)
            {
                buildExtractorButton.enabled = true;
                buildExtractorButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(22, 241, 22, 255);
            }
            else
            {
                buildExtractorButton.enabled = false;
				buildExtractorButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 41, 22, 255);
            }
            if (playerStation.GetResources() >= 2000.0f) {
                buildDestroyerButton.enabled = true;
                buildDestroyerButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(22, 241, 22, 255);

            }
            else
            {
                buildDestroyerButton.enabled = false;
                buildDestroyerButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(255, 41, 22, 255);
            }

            resourcesText.text = "Resources: " + Mathf.RoundToInt(playerStation.GetResources()).ToString();
            // Update the color of the button images based on unit health while station is still alive
            foreach (KeyValuePair<Unit, GameObject> pair in unitButtonMap)
            {
                UpdateUnitButtonHealth(pair.Key, playerStation != pair.Key);
            }
        }

      
    }

    public void BuildNewDestroyer()
    {
        if(playerStation.TrySpendResources(2000)) MatchManager.instance.SpawnNewDestroyer(-1);
    }

    public void BuildNewExtractor()
    {
        if (playerStation.TrySpendResources(1000)) MatchManager.instance.SpawnNewExtractor(-1);
    }

    public void Hide(int index)
    {
        if (index < 0 || index > elements.Count) return;
        elements[index].ChangeHide();
    }

    IEnumerator FirstFrame()
    {
        yield return null;

        playerStation = MatchManager.instance.stations[MatchManager.instance.playerFaction];
        AddUnit(playerStation);
        playerStation.onReceivedUnit += AddUnit;
    }

    Dictionary<System.Type, string> typeDictionary = new Dictionary<System.Type, string>() {
        {typeof(ExtractorUnit),  "Mining Probe"},
        {typeof(ShipUnit),  "Destroyer"},
        {typeof(StationUnit),  "Station"}
        };

    void AddUnit(Unit unit)
    {
        unit.OnUnitDestroyed += RemoveUnit;
        GameObject go = Instantiate(unitButtonPrefab, unitList.transform);
        TMP_Text text = go.transform.GetChild(0).GetComponent<TMP_Text>();
        text.text = typeDictionary[unit.GetType()];
        Button button = go.GetComponent<Button>();

        button.onClick.AddListener(() =>
		{	
			unitselector.ToggleSelect(unit);
			if (selectLockOn.IsPressed())
				Camera.main.GetComponent<CamController>().LockOnUnit(unit);
        });

        unitButtonMap.Add(unit, go);

    }

    void RemoveUnit(Unit unit)
    {
        GameObject go = unitButtonMap[unit];
        unitButtonMap.Remove(unit);
        Destroy(go);
    }

    void UpdateUnitButtonHealth(Unit unit, bool isUnit)
    {
        GameObject go = unitButtonMap[unit];
        Image image = go.GetComponent<Image>();
		if (isUnit && unit.isSelected) {
			image.color = new Color(0.5f, 0.5f, 0f);
		}
		else {
        	float healthRatio = unit.GetHealthRatio();
        	image.color = Color.Lerp(Color.red, Color.blue, healthRatio);
		}
    }


}

[System.Serializable]
public class HUDElement
{
    public RectTransform trans;
    public Vector3 shownPosition;
    public Vector3 hidePosition;
    public bool hidden = false;
    public float time = 1f;
    private float elapsed = 0f;

    public void Start()
    {
        elapsed = time;
    }

    public void ChangeHide()
    {
        hidden = !hidden;
        elapsed = 0.0f;
    }

    public void Update()
    {
        float t = Mathf.Clamp01(elapsed / time);
        Vector3 position = Vector3.zero;
        if (hidden) position = Vector3.Lerp(shownPosition, hidePosition, t);
        else position = Vector3.Lerp(hidePosition, shownPosition, t);
        trans.anchoredPosition = position;
        elapsed += Time.deltaTime;
    }
}
