using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
	protected StationUnit station;
	protected List<ShipUnit> attackUnits;
	protected float minDecisionTime = 1f;
	protected float maxDecisionTime = 5f;

	public bool alive = true;

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
	}

	IEnumerator Decision() {
		while (alive) {
			yield return new WaitForSeconds(Random.Range(minDecisionTime, maxDecisionTime));

			StationUnit closest = null;
			float dist = float.PositiveInfinity;
			//do decision
			foreach (var pair in MatchManager.instance.stations) {
				if (pair.Value == station)	continue;

				//closest check
				float newDist = Vector3.Distance(pair.Value.transform.position, transform.position);
				if (newDist < dist) {
					dist = newDist;
					closest = pair.Value;
				}
			}

			if (closest) {
				//make all attackers target this one
				foreach (ShipUnit ship in attackUnits) {
					ship.SetFollowTarget(closest.GetPlanet());
				}
			}
		}
	}

	Dictionary<System.Type, System.Action<Unit, AIPlayer>> typeDictionary = new Dictionary<System.Type, System.Action<Unit, AIPlayer>>() {
		{typeof(ShipUnit), (Unit unit, AIPlayer ai) => { ai.AddAttackUnit((ShipUnit)unit); }}
	};

	public void AddUnit(Unit unit) {
		if (typeDictionary.ContainsKey(unit.GetType()))
			typeDictionary[unit.GetType()](unit, this);
	}

	public void AddAttackUnit(ShipUnit unit) {
		attackUnits.Add(unit);
		unit.OnUnitDestroyed += ship => attackUnits.Remove((ShipUnit)ship);
	}
}
