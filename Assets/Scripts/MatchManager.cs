using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Gameplay")]
    public CelestialBody sun;
    public Faction playerFaction;
    public List<Faction> aiFactions = new List<Faction>();
    public GameObject StationPrefab;
    public GameObject DestroyerPrefab;
    public int destroyersAtSpawn = 2;

    [Space]
    [Header("Music")]
    public int intesnityLevelCutoff = 5;
    int numInCombat = 0;
    int desiredIntensity = 0;

    bool inCombat = false;
    bool acutalInCombat = false;
    float timeSinceCombatStatusChanged = 0.0f;
    bool firstFrame = true;
    bool secondFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        MusicManager.instance.FadeTracksIn(1, int.MaxValue, 5f);
        inCombat = false;
        acutalInCombat = false;
        firstFrame = true;
        secondFrame = false;
    }

    private void Update()
    {
        if (secondFrame) SecondFrame();
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
        secondFrame = true;
    }

    void SecondFrame()
    {
        for (int i = 0; i< destroyersAtSpawn; i++)
        {
            SpawnNewDestroyer(playerFaction);
        }

        for(int i = 0; i < aiFactions.Count; i++)
        {
            for (int j = 0; j < destroyersAtSpawn; j++)
            {
                SpawnNewDestroyer(aiFactions[i]);
            }
        }

        secondFrame = false;
    }

    void PlaceStation(int index, CelestialBody planet, Faction faction)
    {
        GameObject go = Instantiate(StationPrefab);
        StationUnit station = go.GetComponent<StationUnit>();
        station.SetPlanet(planet);
        station.SetFaction(faction);
    }

    public ShipUnit SpawnNewDestroyer(Faction faction)
    {
        StationUnit[] stations = FindObjectsOfType<StationUnit>();

        for (int i = 0; i < stations.Length; i++)
        {
            StationUnit station = stations[i];
            if (!station.GetFaction().SameFaction(faction)) continue;

            Vector3 offset = Random.onUnitSphere;
            while (offset == Vector3.up || offset == Vector3.down) offset = Random.onUnitSphere;
            offset.y = 0.0f;
            offset = offset.normalized;
            Vector3 newPos = new Vector3(station.transform.position.x + (offset.x * 5.0f), 0.0f, station.transform.position.z + (offset.z * 5.0f));

            GameObject go = Instantiate(DestroyerPrefab, newPos, Quaternion.identity);

            ShipUnit unit = go.GetComponent<ShipUnit>();
            unit.SetFaction(faction);
            unit.SetFollowTarget(station.GetPlanet());

            return unit;
        }

        return null;
    }

    public ShipUnit SpawnNewDestroyer(int factionIndex)
    {
        Faction faction;
        if (factionIndex >= 0 && factionIndex < aiFactions.Count) faction = aiFactions[factionIndex];
        else faction = playerFaction;

        return SpawnNewDestroyer(faction);
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
