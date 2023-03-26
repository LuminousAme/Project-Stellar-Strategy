using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationUnit : Unit
{
    [SerializeField] protected Vector3 offset;
    protected CelestialBody attachedPlanet;



    protected override void Update()
    {
        base.Update();
        if(attachedPlanet != null) transform.position = attachedPlanet.transform.position + offset;
        Select();
    }

    public void SetPlanet(CelestialBody planet)
    {
        attachedPlanet = planet;
        if (attachedPlanet != null) transform.position = attachedPlanet.transform.position + offset;
    }

    public CelestialBody GetPlanet() => attachedPlanet;
}
