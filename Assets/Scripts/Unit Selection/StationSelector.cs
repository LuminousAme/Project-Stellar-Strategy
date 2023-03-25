using UnityEngine;

public class StationSelector : MonoBehaviour
{
    public GameObject[] planets; // an array of game objects to choose from
    private bool playerPlanetChosen; // flag to check if a planet  has been chosen
    private bool aiPlanetChosen; // flag to check if a planet  has been chosen

    [SerializeField] private GameObject spaceStation;

    [SerializeField] private GameObject selectedPlanet;
    [SerializeField] private GameObject aiSelectedPlanet;

    private int randomIndexPlayer;
    private int randomIndexAi;

    // Start is called before the first frame update
    private void Start()
    {
        playerPlanetChosen = false;
        aiPlanetChosen = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!playerPlanetChosen) // if no planet has been chosen yet for the player
        {
            planets = GameObject.FindGameObjectsWithTag("Planet"); // find all game objects with the "Planet" tag and add them to the array

            randomIndexPlayer = Random.Range(0, planets.Length); // choose a random index in the planets array
            selectedPlanet = planets[randomIndexPlayer]; // get the game object at the random index

            if (spaceStation != null && selectedPlanet != null)
            {
                GameObject station = Instantiate(spaceStation, selectedPlanet.transform.position, Quaternion.identity);
                station.transform.SetParent(selectedPlanet.transform);
                // set the offset based on the size of the selected planet

                Vector3 planetSize = selectedPlanet.transform.localScale;
                Vector3 stationSize = station.transform.localScale;

                Vector3 offset = (planetSize + stationSize) * 1.1f;
                station.transform.position += offset;
            }

            playerPlanetChosen = true; // set the flag to true to prevent choosing another planet
        }

        if (!aiPlanetChosen) // if no planet for ai has been chosen yet
        {
            planets = GameObject.FindGameObjectsWithTag("Planet"); // find all game objects with the "Planet" tag and add them to the array

            randomIndexAi = Random.Range(0, planets.Length); // choose a random index in the planets array
            while (randomIndexAi == randomIndexPlayer) // if ai's chosen planet index is the same as player's chosen planet index
            {
                randomIndexAi = Random.Range(0, planets.Length); // keep generating a new random index for ai
            }
            aiSelectedPlanet = planets[randomIndexAi]; // get the game object at the random index

            if (spaceStation != null && aiSelectedPlanet != null)
            {
                GameObject station = Instantiate(spaceStation, aiSelectedPlanet.transform.position, Quaternion.identity);
                station.transform.SetParent(aiSelectedPlanet.transform);
                // set the offset based on the size of the selected planet

                Vector3 planetSize = aiSelectedPlanet.transform.localScale;
                Vector3 stationSize = station.transform.localScale;

                Vector3 offset = (planetSize + stationSize) * 1.1f;
                station.transform.position += offset;
            }

            aiPlanetChosen = true; // set the flag to true to prevent choosing another planet
        }

        //snaps camera to space station
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            if (playerPlanetChosen)
            {
                Vector3 newPos = new Vector3(selectedPlanet.transform.position.x + 50f, Camera.main.transform.position.y, selectedPlanet.transform.position.z - 10f);
                Camera.main.transform.position = newPos;

                //                Camera.main.transform.LookAt(selectedPlanet.transform);

                //Vector3 lookDirection = selectedPlanet.transform.position - Camera.main.transform.position;
                //Camera.main.transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            }
        }
    }
}