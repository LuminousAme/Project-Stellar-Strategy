using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractorUnit : ShipUnit
{
	[SerializeField]
	int resourcesHeld;
	[SerializeField]
	int maxResourcesHeld = 10000;

	PlanetData orbitingPlanet;
	protected override void Update()
	{
		base.Update();

		//check what's being followed
		PlanetData data = followTarget?.GetComponent<PlanetData>();

		//if in range, try grabbing resources
		if (data) {
			if (orbitingPlanet == null) {
				orbitingPlanet = data;
				orbitingPlanet.GrabResources(this);
			}
		}
		else if (orbitingPlanet) {
			orbitingPlanet.StopGrabResources(this);
			orbitingPlanet = null;
		}
	}

	public void GiveResources(int amt) {
		resourcesHeld = Mathf.Min(resourcesHeld + amt, maxResourcesHeld);

		if (resourcesHeld == maxResourcesHeld) {
			//do something
		}
	}
}
