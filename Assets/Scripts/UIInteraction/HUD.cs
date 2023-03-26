using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public List<HUDElement> elements = new List<HUDElement>();
    public GameObject unitList, unitButtonPrefab;
    private StationUnit playerStation;
    private Dictionary<Unit, GameObject> unitButtonMap = new Dictionary<Unit, GameObject>();

    private void Start()
    {
        for (int i = 0; i < elements.Count; i++) elements[i].Start();
        StartCoroutine(FirstFrame());
    }

    private void Update()
    {
        for (int i = 0; i < elements.Count; i++) elements[i].Update();
    }

    public void BuildNewDestroyer()
    {
        MatchManager.instance.SpawnNewDestroyer(-1);
    }

    public void Hide(int index)
    {
        if (index < 0 || index > elements.Count) return;
        elements[index].ChangeHide();
    }

    IEnumerator FirstFrame()
    {
        yield return null;
        //yield return null;

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
            SelectAndLockOn(unit);
        });

        unitButtonMap.Add(unit, go);
    }

    void RemoveUnit(Unit unit)
    {
        GameObject go = unitButtonMap[unit];
        unitButtonMap.Remove(unit);
        Destroy(go);
    }

    void SelectAndLockOn(Unit unit)
    {
        Camera.main.GetComponent<CamController>().LockOnUnit(unit);
        //TODO: select unit
    }

}

[System.Serializable]
public class HUDElement
{
    public Transform trans;
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
        trans.localPosition = position;
        elapsed += Time.deltaTime;
    }
}
