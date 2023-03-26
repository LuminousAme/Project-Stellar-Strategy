using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public MatchManager matchManager;
    public List<HUDElement> elements = new List<HUDElement>();
    public GameObject unitList, unitButtonPrefab;

    private void Start()
    {
        for (int i = 0; i < elements.Count; i++) elements[i].Start();
    }

    private void Update()
    {
        for (int i = 0; i < elements.Count; i++) elements[i].Update();
    }

    public void BuildNewDestroyer()
    {
        matchManager.SpawnNewDestroyer(-1);
    }

    public void Hide(int index)
    {
        if (index < 0 || index > elements.Count) return;
        elements[index].ChangeHide();
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
