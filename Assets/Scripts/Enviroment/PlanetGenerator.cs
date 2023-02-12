using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct AtmosphereData
    {
        public Gradient gradient;
        [Range(0f, 0.05f)]
        public float thickness;
        [Range(1f, 25f)]
        public float power;
    }

    private int seed;
    public int Seed
    {
        get { return seed; }
        set { seed = value; }
    }

    [Header("Settings")]
    public int minNumOfPlanets;
    public int maxNumOfPlanets;
    [Range(2, 128)]
    public int resolution = 64;
    public GameObject planetPrefab;

    [Space]
    [Header("Orbit")]
    public CelestialBody sun;
    public float minDistanceFromSun, minDistanceBetweenPlanets, maxDistanceBetweenPlanets;
    public float minRadius, maxRadius;
    public float minOrbitTime, maxOrbitTime;

    [Space]
    [Header("Planet Generation")]
    public List<PlanetShapeSettings> shapeSettings = new List<PlanetShapeSettings>();
    public List<PlanetColorSettings> colorSettings = new List<PlanetColorSettings>();

    [Space]
    [Header("Atmosphere")]
    public List<AtmosphereData> atmosphereSettings = new List<AtmosphereData>();
    private float planetAtmoshpereRatio = 1.25f;

    List<GameObject> planets = new List<GameObject>();

    public void Run()
    {
        //destroy any existing planets
        DestroyPlantes();

        //if we don't have a prefab don't bother
        if (!planetPrefab) return;

        //set the seed
        Random.InitState(seed);

        int numOfPlants = Random.Range(minNumOfPlanets, maxNumOfPlanets+1);
        for(int i = 0; i < numOfPlants; ++i)
        {
            //generate the distance from the sun, based on the distance from the previous planets
            float distanceFromTheSun = 0.0f;
            if (i == 0) distanceFromTheSun = minDistanceFromSun;
            else 
            {
                float startingDistance = Vector3.Distance(planets[i - 1].transform.position, sun.transform.position);
                distanceFromTheSun = startingDistance + Random.Range(minDistanceBetweenPlanets, maxDistanceBetweenPlanets);
            }

            //set the position (random point on a unit circle * distance from the sun)
            float angle = Random.Range(0.0f, 2.0f * Mathf.PI);
            Vector3 position = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle)).normalized * distanceFromTheSun;

            //create the game object
            GameObject go = Instantiate(planetPrefab, transform);
            
            //set it's transform details
            go.transform.position = position;

            //set the planet generation settings
            Planet planet = go.GetComponent<Planet>();
            planet.resolution = resolution;
            int shapeIndex = Random.Range(0, shapeSettings.Count);
            planet.shapeSettings = shapeSettings[shapeIndex];
            int colorIndex = Random.Range(0, (colorSettings.Count <= atmosphereSettings.Count) ? 
                colorSettings.Count : atmosphereSettings.Count);
            planet.colorSettings = colorSettings[colorIndex];
            planet.GeneratePlanet();

            //set the celestial body settings
            float maxPossibleDistance = minDistanceFromSun + maxDistanceBetweenPlanets * numOfPlants;
            float orbitT = MathUtils.InverseLerp(minDistanceFromSun, maxPossibleDistance, distanceFromTheSun);
            float orbitTime = MathUtils.LerpClamped(minOrbitTime, maxOrbitTime, orbitT);
            CelestialBody celestialBody = go.GetComponent<CelestialBody>();
            celestialBody.orbitTime = orbitTime;
            celestialBody.rotatingAround = sun;

            //set the atmosphere settings
            AtmosphereData atmosphereData = atmosphereSettings[colorIndex];
            Atmosphere atmosphere = go.GetComponentInChildren<Atmosphere>();
            atmosphere.gradient = atmosphereData.gradient;
            atmosphere.thickness = atmosphereData.thickness;
            atmosphere.power = atmosphereData.power;
            atmosphere.radius = planet.shapeSettings.radius * planetAtmoshpereRatio;
            atmosphere.sun = sun;

            //add it to the list of gameobjects
            planets.Add(go);
        }

        Random.InitState((int)System.DateTime.Now.Ticks);
    }

    void DestroyPlantes()
    {
        for (int i = 0; i < planets.Count; ++i)
        {
            Destroy(planets[i]);        
        }
        planets.Clear();
    }
}
