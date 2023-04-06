using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour {
	enum PurchaseWant {
		WAIT,
		DESTROYER,
		EXTRACTOR,
	}

	[SerializeField]
	private bool alive = true;
	[SerializeField]
	private float alignment = 0f;
	[SerializeField]
	private float alignmentIncreaseOverTime = 0f;
	[SerializeField]
	private float alignmentDecreasePerUnit = 0f;
	[SerializeField]
	private AnimationCurve alignmentRandomFlipCurve; //X is alignment, Y is chance to flip
	[SerializeField]
	private float minDecisionTime = 1f;
	[SerializeField]
	private float maxDecisionTime = 5f;
	[SerializeField]
	private AnimationCurve decisionDifficultyCurve; //X is minute time, Y is number to divide decisionTime by
	[SerializeField]
	private AnimationCurve distanceResourceThresholdCurve; //X is distance from station to planet, Y is resource threshold needed to send extractor
	[SerializeField]
	private int maxStationExtractorUnits = 2;
	[SerializeField]
	private int maxPlanetExtractorUnits = 4;
	[SerializeField]
	private AnimationCurve defenseAttackCurve; //X is amt of allied destroyer units that is orbiting allied station planet, Y is amt to send for attack
	[SerializeField]
	private AnimationCurve alignmentAttackCurve; //X is alignment, Y is threshold of desiredShipUnitsToAttack/enemyShipUnitsInDefense. desiredShipUnitsToAttack is just defenseAttackCurve(X), enemyShipUnitsInDefense is just amt of enemy destroyer units orbiting enemy station

	protected StationUnit station;
	protected List<ShipUnit> shipUnits = new List<ShipUnit>();
	protected List<ExtractorUnit> extractorUnits = new List<ExtractorUnit>();

	private float aliveTime = 0f;
	private float aliveTimePrevious = 0f;
	private float startTime = 0f;
	private PurchaseWant currentPurchaseWant = PurchaseWant.WAIT;

	private static int maxPurchaseWant = 0;
	private static int amtOfPlanets = 0;

	public void SetStation(StationUnit station) {
		if (this.station) {
			this.station.onReceivedUnit -= AddUnit;
			this.station.OnUnitDestroyed -= StationDestroyed;
		}
		this.station = station;
		this.station.onReceivedUnit += AddUnit;
		this.station.OnUnitDestroyed += StationDestroyed;
	}

	//has a deecision timer
	private void Start() {
		startTime = Time.time;
		aliveTime = Time.time - startTime;
		aliveTimePrevious = aliveTime;

		if (maxPurchaseWant == 0)
			maxPurchaseWant = PurchaseWant.GetValues(typeof(PurchaseWant)).Cast<int>().Last() + 1;

		if (amtOfPlanets == 0)
			amtOfPlanets = GameObject.FindGameObjectsWithTag("Planet").Length - MatchManager.instance.stations.Count();
			

		StartCoroutine(Decision());
		//defend self
		/*for (int i = 0; i < Mathf.Min(defenseUnits, attackUnits.Count); ++i) {
			//send ships to incoming attackers
			
		}*/
	}

	IEnumerator Decision() {
		yield return new WaitForSeconds(maxDecisionTime);

		while (alive) {
			aliveTimePrevious = aliveTime;
			aliveTime = Time.time - startTime;
			alignment = Mathf.Clamp(alignment + alignmentIncreaseOverTime * (aliveTime - aliveTimePrevious) / 60f, -1f, 1f);

			if (Random.value <= alignmentRandomFlipCurve.Evaluate(alignment))
				alignment *= -1f;

			PurchaseLogic();

			AttackLogic();

			ExtractorLogic();

			float decisionDifficulty = decisionDifficultyCurve.Evaluate(aliveTime / 60f);
			yield return new WaitForSeconds(Random.Range(minDecisionTime / decisionDifficulty, maxDecisionTime / decisionDifficulty));
		}
	}

	void PurchaseLogic() {
		switch (currentPurchaseWant) {
			default:
				ChangePurchaseCurrentWant();
				return;
			case PurchaseWant.DESTROYER:
				if (station.TrySpendResources(2000f, typeof(ShipUnit))) {
					ChangePurchaseCurrentWant();
					return;
				}
				break;
			case PurchaseWant.EXTRACTOR:
				if (station.TrySpendResources(1000f, typeof(ExtractorUnit))) {
					ChangePurchaseCurrentWant();
					return;
				}
				break;
		}

		ChangePurchaseCurrentWant(true);
	}

	void AttackLogic() {
		int attackUnitsToSend = Mathf.CeilToInt(defenseAttackCurve.Evaluate((float)station.GetOrbitingUnitCount<ShipUnit>()));
		if (attackUnitsToSend == 0)
			return;

		//Edit FindClosestPlanet for include/exlude enum flag, enum flag is station and destroyer orbit
		StationUnit closestStation = FindClosestStation();
		if (!closestStation || closestStation.GetOrbitingUnitCount<ShipUnit>() > 0 && (float)attackUnitsToSend / closestStation.GetOrbitingUnitCount<ShipUnit>() < alignmentAttackCurve.Evaluate(alignment))
			return;

		//make all attackers target this one
		for (int i = shipUnits.Count - attackUnitsToSend; i < shipUnits.Count; ++i) {
			shipUnits[i].SetFollowTarget(closestStation.GetPlanet());
		}
	}

	void ExtractorLogic() {
		//Extractors zoom off if ShipUnits (destroyers) orbit the planet they are grabbing resources on
		foreach (KeyValuePair<Faction, StationUnit> curStation in MatchManager.instance.stations) {
			if (curStation.Value == station)
				continue;

			foreach (ExtractorUnit curUnit in extractorUnits) {
				if (curUnit.GetFollowTarget() != station.GetPlanet() && curStation.Value.GetOrbitingUnitCount<ShipUnit>(curUnit.GetFollowTarget()) > 0)
					curUnit.SetFollowTarget(station.GetPlanet());
			}
		}

		if (extractorUnits.Count <= maxStationExtractorUnits)
			return;

		float minSearchDistance = 0f;
        for (int i = 0; i < amtOfPlanets; ++i) {
			PlanetData closest = FindClosestPlanet(true, true, minSearchDistance);
			if (!closest)
				break;

			//spread gatherers
			for (int index = maxStationExtractorUnits; index < extractorUnits.Count; ++index) {
				if (station.GetOrbitingUnitCount<ExtractorUnit>(closest.GetComponent<CelestialBody>(), 2000f) >= maxPlanetExtractorUnits) {
					minSearchDistance = Vector3.Distance(station.transform.position, closest.transform.position) + 10f;
					break;
				}

				if (extractorUnits[index].GetResources() < extractorUnits[index].GetRate() && extractorUnits[index].Unloading()) {
					extractorUnits[index].SetFollowTarget(closest.GetComponent<CelestialBody>());
				}
			}
		}
	}

	PlanetData FindClosestPlanet(bool skipStations, bool resourceThreshold, float minDistance) {
		//find nearest uninhabited planet
		PlanetData[] bodies = FindObjectsOfType<PlanetData>();

		//look for those without stations
		PlanetData closest = null;
		float dist = float.PositiveInfinity;

		foreach (PlanetData bod in bodies) {
			if (bod.HasStation() && skipStations) continue;

			float newDist = Vector3.Distance(station.transform.position, bod.transform.position);
			if (minDistance <= newDist && newDist < dist && (bod.GetResources() >= distanceResourceThresholdCurve.Evaluate(newDist) || !resourceThreshold)) {
				dist = newDist;
				closest = bod;
			}
		}

		return closest;
	}

	StationUnit FindClosestStation() {
		StationUnit closest = null;
		float dist = float.PositiveInfinity;

		foreach (var pair in MatchManager.instance.stations) {
			if (pair.Value == station) continue;

			//closest check
			float newDist = Vector3.Distance(pair.Value.transform.position, station.transform.position);
			if (newDist < dist) {
				dist = newDist;
				closest = pair.Value;
			}
		}

		return closest;
	}

	void ChangePurchaseCurrentWant(bool failedPurchase = false) {
		//Higher the alignement, more chance for wanting extractor
		if (extractorUnits.Count() < maxStationExtractorUnits + maxPlanetExtractorUnits * amtOfPlanets)
			currentPurchaseWant = (PurchaseWant)Random.Range(0, maxPurchaseWant);
		else
			currentPurchaseWant = (PurchaseWant)Random.Range(0, maxPurchaseWant - 1);
	}

	public void StationDestroyed(Unit unit) {
		alive = false;
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
		shipUnits.Add(unit);
		unit.OnUnitDestroyed += ship => { shipUnits.Remove((ShipUnit)ship); alignment = Mathf.Clamp(alignment - alignmentDecreasePerUnit, -1f, 1f); };
	}

	public void AddExtractorUnit(ExtractorUnit unit) {
		extractorUnits.Add(unit);
		unit.OnUnitDestroyed += ship => { extractorUnits.Remove((ExtractorUnit)ship); alignment = Mathf.Clamp(alignment - alignmentDecreasePerUnit, -1f, 1f); };
	}
}
