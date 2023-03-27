using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetData : MonoBehaviour
{
	[SerializeField]
	float resources = 0f;
	[SerializeField]
	float maxResources = 1000000f;
	[SerializeField]
	float extractionRate = 100f;
	[SerializeField]
	float generationRate = 100f;

	List<ExtractorUnit> extractors = new List<ExtractorUnit>();
	//add them to the list
	public void GrabResources(ExtractorUnit unit) {
		extractors.Add(unit);
	}
	//add them to the list
	public void StopGrabResources(ExtractorUnit unit) {
		extractors.Remove(unit);
	}

	public bool CheckExtractor(ExtractorUnit unit) {
		return extractors.Contains(unit);
	}

	private void Update() {
		resources = Mathf.Min(resources + generationRate * Time.deltaTime, maxResources);

		if (extractors.Count == 0)	return;

		//give resources to all in the list, limited by how many resources there are
		float rate = Mathf.Min(extractionRate * extractors.Count, resources) / extractors.Count;
		foreach (ExtractorUnit unit in extractors) {
			unit.GiveResources(rate);
			resources -= rate;
		}
	}

	public bool HasStation() {
		foreach (var station in MatchManager.instance.stations) {
			if (station.Value.GetPlanet().transform == transform) {
				return true;
			}
		}

		return false;
	}
}
