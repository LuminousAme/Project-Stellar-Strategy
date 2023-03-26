using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
	protected StationUnit station;
	protected List<ShipUnit> attackUnits = new List<ShipUnit>();
	public float minDecisionTime = 1f;
	public float maxDecisionTime = 5f;
	public int defenseUnits = 2;
	public int minAttackUnits = 7;

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
		//defend self
		/*for (int i = 0; i < Mathf.Min(defenseUnits, attackUnits.Count); ++i) {
			//send ships to incoming attackers
			
		}*/
	}

	IEnumerator Decision() {
		while (alive) {
			yield return new WaitForSeconds(Random.Range(minDecisionTime, maxDecisionTime));

			if (attackUnits.Count - defenseUnits >= minAttackUnits) {
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
					for (int i = defenseUnits; i < attackUnits.Count; ++i) {
						attackUnits[i].SetFollowTarget(closest.GetPlanet());
					}
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