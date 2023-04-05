using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
	public bool alive = true;
	public float alignment = 0f;
	public float minDecisionTime = 1f;
	public float maxDecisionTime = 5f;
	public AnimationCurve decisionDifficultyCurve;
	public int maxLocalGatherUnits = 2;
	public int defenseUnits = 2;
	public int minAttackUnits = 7;

	protected StationUnit station;
	protected List<ShipUnit> attackUnits = new List<ShipUnit>();
	protected List<ExtractorUnit> extractorUnits = new List<ExtractorUnit>();

	private float aliveTime = 0f;
	private float startTime = 0f;

	enum Want {
		WAIT,
		DESTROYER,
		EXTRACTOR,
	}

	Want currentWant = Want.WAIT;

	public void SetStation(StationUnit station) {
		if (this.station) {
			this.station.onReceivedUnit -= AddUnit;
		}
		this.station = station;
		this.station.onReceivedUnit += AddUnit;
	}

	//has a deecision timer
	private void Start() {
		StartCoroutine(Decision());
		startTime = Time.time;
		//Debug.Log(Time.timeSinceLevelLoad);
		//defend self
		/*for (int i = 0; i < Mathf.Min(defenseUnits, attackUnits.Count); ++i) {
			//send ships to incoming attackers
			
		}*/
	}

	IEnumerator Decision() {
		while (alive) {
			aliveTime = Time.time - startTime;
			yield return new WaitForSeconds(Random.Range(minDecisionTime / decisionDifficultyCurve.Evaluate(aliveTime), maxDecisionTime / decisionDifficultyCurve.Evaluate(aliveTime)));

			SpendResources();

			if (attackUnits.Count - defenseUnits >= minAttackUnits) {
				Attack();
			}

			//spread gatherers
			if (extractorUnits.Count > maxLocalGatherUnits) {
				for (int i = maxLocalGatherUnits; i < extractorUnits.Count; ++i) {
					if (extractorUnits[i].GetResources() < extractorUnits[i].GetRate() && extractorUnits[i].Unloading()) {
						SendGatherer(extractorUnits[i]);
					}
				}
			}
		}
	}

	void SpendResources() {
		//Higher the alignement, more chance for wanting extractor

		switch (currentWant) {
			default:
				currentWant = (Want)Random.Range(0, 3);
				return;
			case Want.DESTROYER:
				if (station.TrySpendResources(2000f)) {
					MatchManager.instance.SpawnNewDestroyer(station.GetFaction());
					currentWant = (Want)Random.Range(0, 3);
				}
				return;
			case Want.EXTRACTOR:
				if (station.TrySpendResources(1000f)) {
					MatchManager.instance.SpawnNewExtractor(station.GetFaction());
					currentWant = (Want)Random.Range(0, 3);
				}
				return;
		}
	}

	void Attack() {
		StationUnit closest = null;
		float dist = float.PositiveInfinity;
		//do decision
		foreach (var pair in MatchManager.instance.stations) {
			if (pair.Value == station)	continue;

			//closest check
			float newDist = Vector3.Distance(pair.Value.transform.position, station.transform.position);
			if (newDist < dist) {
				dist = newDist;
				closest = pair.Value;
			}
		}

		if (closest) {
			//make all attackers target this one
			for (int i = defenseUnits; i < attackUnits.Count; ++i) {
				attackUnits[i].SetFollowTarget(closest.GetPlanet());
			}
		}
	}

	void SendGatherer(ExtractorUnit unit) {
		//find nearest uninhabited planet
		PlanetData[] bodies = FindObjectsOfType<PlanetData>();

		//look for those without stations
		PlanetData closest = null;
		float dist = float.PositiveInfinity;

		foreach (PlanetData bod in bodies) {
			if (bod.HasStation())	continue;
			
			float newDist = Vector3.Distance(station.transform.position, bod.transform.position);
			if (newDist < dist) {
				dist = newDist;
				closest = bod;
			}
		}

		if (closest) {
			unit.SetFollowTarget(closest.GetComponent<CelestialBody>());
		}
	}

	Dictionary<System.Type, System.Action<Unit, AIPlayer>> typeDictionary = new Dictionary<System.Type, System.Action<Unit, AIPlayer>>() {
		{typeof(ShipUnit), (Unit unit, AIPlayer ai) => { ai.AddAttackUnit((ShipUnit)unit); }},
		{typeof(ExtractorUnit), (Unit unit, AIPlayer ai) => { ai.AddExtractorUnit((ExtractorUnit)unit); }}
	};

	public void AddUnit(Unit unit) {
		if (typeDictionary.ContainsKey(unit.GetType()))
			typeDictionary[unit.GetType()](unit, this);
	}

	public void AddAttackUnit(ShipUnit unit) {
		attackUnits.Add(unit);
		unit.OnUnitDestroyed += ship => attackUnits.Remove((ShipUnit)ship);
	}

	public void AddExtractorUnit(ExtractorUnit unit) {
		extractorUnits.Add(unit);
		unit.OnUnitDestroyed += ship => extractorUnits.Remove((ExtractorUnit)ship);
	}
}
