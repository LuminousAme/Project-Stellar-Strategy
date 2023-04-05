using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractorUnit : ShipUnit
{
	[SerializeField]	ParticleSystem attractionBeam;
	[SerializeField]	Transform attractionBeamRotPoint;
	[SerializeField]	ParticleSystem outputBeam;
	[SerializeField]	float resourcesHeld = 0f;
	[SerializeField]	float maxResourcesHeld = 10000f;
	[SerializeField]	float depositRate = 1000f;

	PlanetData orbitingPlanet;

	protected override void Start()
	{
		base.Start();

		OnUnitDestroyed += unit => {
			if (orbitingPlanet) {
				orbitingPlanet.StopGrabResources(this);
			}
		};
	}

	bool connectedToStation = false;
	protected override void Update()
	{
		base.Update();

		if (orbitingPlanet && followTarget && followTarget.transform != orbitingPlanet.transform) {
			orbitingPlanet.StopGrabResources(this);
			orbitingPlanet = null;
		}

		if (attractionBeam.isPlaying) {
			var system = attractionBeam.main;
			if (orbitingPlanet) {
				//if it's an extraction planet, do the beam in, with some slight fixed offset
				attractionBeamRotPoint.LookAt(orbitingPlanet.transform);
				attractionBeam.transform.localPosition = Vector3.forward * (Vector3.Distance(
					attractionBeamRotPoint.position, orbitingPlanet.transform.position
					) - 1f);
				system.startLifetime = attractionBeam.transform.localPosition.z / system.startSpeed.constant;
			}
			else {
				//stop beam
				//system.simulationSpace = ParticleSystemSimulationSpace.World;
				attractionBeam.Stop();
				attractionBeam.Clear();
			}
		}

		//check for station
		if (connectedToStation) {
			if (outputBeam.isStopped) {
				//var system = outputBeam.main;
				//system.simulationSpace = ParticleSystemSimulationSpace.Local;
				outputBeam.Play();
			}
		}
		else if (outputBeam.isPlaying) {
			//var system = outputBeam.main;
			//system.simulationSpace = ParticleSystemSimulationSpace.World;
			outputBeam.Stop();
			outputBeam.Clear();
		}
	}

	protected void OnTriggerStay(Collider other)
	{

		//look for what you're following
		if (followTarget && other.transform == followTarget.transform) {
			PlanetData data = followTarget.GetComponent<PlanetData>();

			if (data) {
				if (!data.CheckExtractor(this)) {
					orbitingPlanet = data;
					orbitingPlanet.GrabResources(this);
					attractionBeam.Play();
					//var system = attractionBeam.main;
					//system.simulationSpace = ParticleSystemSimulationSpace.Local;
				}
			}

			//to update this value

			connectedToStation = false;
			//check if the station is connected/exists
			StationUnit station;
			if (MatchManager.instance.stations.TryGetValue(faction, out station)) {
				if (followTarget == station.GetPlanet()) {
					float change = Mathf.Min(depositRate * Time.deltaTime, resourcesHeld);
					resourcesHeld -= change;
					station.DepositResources(change);
					connectedToStation = true;

					outputBeam.transform.LookAt(station.transform);
					var system = outputBeam.main;
					system.startLifetime = Vector3.Distance(
						outputBeam.transform.position, station.transform.position
						) / system.startSpeed.constant;
				}
			}
		}
	}

	//reset connectedToStation because pain
	public override void SetFollowTarget(CelestialBody followTarget)
	{
		base.SetFollowTarget(followTarget);
		
		connectedToStation = false;
	}

	public override void SetSeekTarget(Vector3 targetPosition)
	{
		base.SetSeekTarget(targetPosition);

		connectedToStation = false;
	}

	protected override void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);

		if (orbitingPlanet && other.transform == orbitingPlanet.transform) {
			orbitingPlanet.StopGrabResources(this);
			orbitingPlanet = null;
			connectedToStation = false;
		}
	}

	public void GiveResources(float amt) {
		resourcesHeld = Mathf.Min(resourcesHeld + amt, maxResourcesHeld);

		if (resourcesHeld == maxResourcesHeld) {
			//do something
			SetFollowTarget(MatchManager.instance.stations.GetValueOrDefault(faction, null)?.GetPlanet());
		}
	}

	public bool Unloading() {
		return orbitingPlanet && followTarget == MatchManager.instance.stations[faction].GetPlanet();
	}

	public float GetResources() => resourcesHeld;
	public float GetRate() => depositRate;
}
