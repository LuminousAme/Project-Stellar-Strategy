using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
	protected StationUnit station;
	protected List<ShipUnit> attackUnits;
	

	public void SetStation(StationUnit station) {
		if (this.station) {

		}
		this.station = station;
	}

	//nearest other player
	

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
