using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
	public static MatchManager instance;

    [Header("Gameplay")]
    public CelestialBody sun;
    public Faction playerFaction;
    public List<Faction> aiFactions = new List<Faction>();
    public List<AIPlayer> aiPlayers = new List<AIPlayer>();
    public StationUnit StationPrefab;
    public ShipUnit DestroyerPrefab;
    public ExtractorUnit ExtractorPrefab;
	public AIPlayer AIPrefab;
    public int destroyersAtSpawn = 2;
    public int extractorsAtSpawn = 2;
    public int maxUnits = 50;
	public event System.Action playerLost;
	public event System.Action playerWon;

    [Space]
    [Header("Music")]
    public int intesnityLevelCutoff = 5;
    int numInCombat = 0;
    int desiredIntensity = 0;

    bool inCombat = false;
    bool acutalInCombat = false;
    float timeSinceCombatStatusChanged = 0.0f;


	Dictionary<Faction, StationUnit> m_stations= new Dictionary<Faction, StationUnit>();
	public Dictionary<Faction, StationUnit> stations { get => m_stations; }

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
        MusicManager.instance.FadeTracksIn(1, int.MaxValue, 5f);
        inCombat = false;
        acutalInCombat = false;

		while (aiPlayers.Count < aiFactions.Count) aiPlayers.Add(Instantiate(AIPrefab));

		StartCoroutine(OrderedFrames());
    }

	private void OnDestroy()
	{
		instance = null;
	}

    private void Update()
    {
        UpdateMusic();
    }

	IEnumerator OrderedFrames() {
		yield return null;
		FirstFrame();
		yield return null;
		SecondFrame();
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
        stations.Add(playerFaction, PlaceStation(randomIndex, bodies[randomIndex], playerFaction));
		stations[playerFaction].OnUnitDestroyed += ctx => playerLost?.Invoke();

        for(int i = 0; i < aiFactions.Count; i++)
        {
            do randomIndex = Random.Range(0, maxIndex);
            while (bodies[randomIndex] == sun || bodiesClaimed.Contains(randomIndex));

            bodiesClaimed.Add(randomIndex);
			stations.Add(aiFactions[i], PlaceStation(randomIndex, bodies[randomIndex], aiFactions[i]));
			stations[aiFactions[i]].OnUnitDestroyed += unit => {
				stations.Remove(aiFactions[i]);
				if (stations.Count == 1) {
					playerWon?.Invoke();
				}
			};

			if (!aiPlayers[i]) {
				aiPlayers[i] = Instantiate(AIPrefab);
			}
            aiPlayers[i].SetStation(stations[aiFactions[i]]);
        }

        Camera.main.GetComponent<CamController>().LockOnCelestialBody(bodies[bodiesClaimed[0]]);
    }

    void SecondFrame()
    {
        for(int j = -1; j < aiFactions.Count; j++)
        {
        	for (int i = 0; i < destroyersAtSpawn; i++)
        	{
	            SpawnNewDestroyer(j);
    	    }
        	for (int i = 0; i < extractorsAtSpawn; i++)
        	{
	            SpawnNewExtractor(j);
    	    }
        }

    }

    StationUnit PlaceStation(int index, CelestialBody planet, Faction faction)
    {
        StationUnit station = Instantiate(StationPrefab);
        station.SetPlanet(planet);
        station.SetFaction(faction);

		return station;
    }

    public ShipUnit SpawnNewDestroyer(Faction faction)
    {
		if (!stations.ContainsKey(faction))	return null;
		
		StationUnit station = stations[faction];

        if (station.GetUnitCount() >= maxUnits) return null;

        Vector3 offset = Random.onUnitSphere;
        while (offset == Vector3.up || offset == Vector3.down) offset = Random.onUnitSphere;
        offset.y = 0.0f;
        offset = offset.normalized;
        Vector3 newPos = new Vector3(station.transform.position.x + (offset.x * 5.0f), 0.0f, station.transform.position.z + (offset.z * 5.0f));

        ShipUnit unit = Instantiate(DestroyerPrefab, newPos, Quaternion.identity);

		station.AddUnit(unit);

        unit.SetFaction(faction);
        unit.SetFollowTarget(station.GetPlanet());

        return unit;
    }

    public ShipUnit SpawnNewDestroyer(int factionIndex)
    {
        Faction faction;
        if (factionIndex >= 0 && factionIndex < aiFactions.Count) faction = aiFactions[factionIndex];
        else faction = playerFaction;

        return SpawnNewDestroyer(faction);
    }

    public ExtractorUnit SpawnNewExtractor(Faction faction)
    {
		if (!stations.ContainsKey(faction))	return null;
		
		StationUnit station = stations[faction];

        if (station.GetUnitCount() >= maxUnits) return null;

        Vector3 offset = Random.onUnitSphere;
        while (offset == Vector3.up || offset == Vector3.down) offset = Random.onUnitSphere;
        offset.y = 0.0f;
        offset = offset.normalized;
        Vector3 newPos = new Vector3(station.transform.position.x + (offset.x * 5.0f), 0.0f, station.transform.position.z + (offset.z * 5.0f));

        ExtractorUnit unit = Instantiate(ExtractorPrefab, newPos, Quaternion.identity);

		station.AddUnit(unit);

        unit.SetFaction(faction);
        unit.SetFollowTarget(station.GetPlanet());

        return unit;
    }

    public ExtractorUnit SpawnNewExtractor(int factionIndex)
    {
        Faction faction;
        if (factionIndex >= 0 && factionIndex < aiFactions.Count) faction = aiFactions[factionIndex];
        else faction = playerFaction;

        return SpawnNewExtractor(faction);
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
