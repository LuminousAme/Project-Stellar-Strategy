using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public CelestialBody sun;
    public Faction playerFaction;
    public List<Faction> aiFactions = new List<Faction>();
    public GameObject StationPrefab;
    public int intesnityLevelCutoff = 5;
    int numInCombat = 0;
    int desiredIntensity = 0;

    bool inCombat = false;
    bool acutalInCombat = false;
    float timeSinceCombatStatusChanged = 0.0f;
    bool firstFrame = true;

    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.FadeTracksIn(1, int.MaxValue, 5f);
        inCombat = false;
        acutalInCombat = false;
        firstFrame = true;
    }

    private void Update()
    {
        if (firstFrame) FirstFrame();
        UpdateMusic();
    }

    void FirstFrame()
    {
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        List<int> bodiesClaimed = new List<int>();

        //claim one for the player
        int maxIndex = bodies.Length - 1;
        int randomIndex = Random.Range(0, maxIndex);

        while(bodies[randomIndex] == sun) randomIndex = Random.Range(0, maxIndex);

        bodiesClaimed.Add(randomIndex);
        PlaceStation(randomIndex, bodies[randomIndex], playerFaction);

        for(int i = 0; i < aiFactions.Count; i++)
        {
            do randomIndex = Random.Range(0, maxIndex);
            while (bodies[randomIndex] == sun || bodiesClaimed.Contains(randomIndex));

            bodiesClaimed.Add(randomIndex);
            PlaceStation(randomIndex, bodies[randomIndex], aiFactions[i]);
        }

        Camera.main.GetComponent<CamController>().LockOnCelestialBody(bodies[bodiesClaimed[0]]);

        firstFrame = false;
    }

    void PlaceStation(int index, CelestialBody planet, Faction faction)
    {
        GameObject go = Instantiate(StationPrefab);
        StationUnit station = go.GetComponent<StationUnit>();
        station.SetPlanet(planet);
        station.SetFaction(faction);
    }

    void UpdateMusic()
    {
        numInCombat = 0;

        Unit[] units = FindObjectsOfType<Unit>();
        for (int i = 0; i < units.Length; i++)
        {
            Unit unit = units[i];
            if (!playerFaction.SameFaction(unit.GetFaction())) continue;

            if (unit.GetInCombat()) numInCombat++;
        }

        desiredIntensity = numInCombat / intesnityLevelCutoff;
        
        if(numInCombat > 0 && !inCombat)
        {
            inCombat = true;
            timeSinceCombatStatusChanged = 0.0f;
        }
        if(numInCombat > 0 && inCombat && !acutalInCombat && timeSinceCombatStatusChanged > 0.2f)
        {
            acutalInCombat = true;
            if (MusicManager.instance.CurrentIndex() != 2) MusicManager.instance.FadeTracksIn(2, 0, 5f);
        }

        if(numInCombat == 0 && inCombat)
        {
            inCombat = false;
            timeSinceCombatStatusChanged = 0.0f;
        }
        if (numInCombat == 0 && !inCombat && acutalInCombat && timeSinceCombatStatusChanged > 1.0f)
        {
            acutalInCombat = false;
            if (MusicManager.instance.CurrentIndex() == 2) MusicManager.instance.FadeTracksIn(1, 0, 5f);
        }

        if (MusicManager.instance.CurrentIndex() == 2) MusicManager.instance.SetCurrentIntensity(desiredIntensity);

        timeSinceCombatStatusChanged += Time.deltaTime;
    }
}
