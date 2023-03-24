using UnityEngine;
using UnityEngine.UI;

public class StationSelector : MonoBehaviour
{
    public GameObject[] planets; // an array of game objects to choose from
    private bool playerPlanetChosen; // flag to check if a planet  has been chosen

    [SerializeField] private GameObject spaceStation;


    [SerializeField] private GameObject selectedPlanet;


    // Start is called before the first frame update
    private void Start()
    {
        playerPlanetChosen = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!playerPlanetChosen) // if no planet has been chosen yet
        {
            planets = GameObject.FindGameObjectsWithTag("Planet"); // find all game objects with the "Planet" tag and add them to the array

            int randomIndex = Random.Range(0, planets.Length); // choose a random index in the planets array
            selectedPlanet = planets[randomIndex]; // get the game object at the random index

            // TODO: Add UI or feedback to indicate which planet has been chosen
            // Add UI or feedback to indicate which planet has been chosen
            if (spaceStation != null && selectedPlanet != null)
            {
                GameObject station = Instantiate(spaceStation, selectedPlanet.transform.position, Quaternion.identity);
                station.transform.SetParent(selectedPlanet.transform);
                // set the offset based on the size of the selected planet

                Vector3 planetSize = selectedPlanet.transform.localScale;
                Vector3 stationSize = station.transform.localScale;

                Vector3 offset = (planetSize+stationSize)*1.1f;
                station.transform.position += offset;
            }

            playerPlanetChosen = true; // set the flag to true to prevent choosing another planet
            
        }

        //snaps camera to space station 
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            if (playerPlanetChosen)
            {
                Vector3 newPos = new Vector3(selectedPlanet.transform.position.x + 50f, Camera.main.transform.position.y, selectedPlanet.transform.position.z-50f);
                Camera.main.transform.position = newPos;

//                Camera.main.transform.LookAt(selectedPlanet.transform);

                //Vector3 lookDirection = selectedPlanet.transform.position - Camera.main.transform.position;
                //Camera.main.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }

        }



    }
}