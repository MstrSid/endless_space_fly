using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// BEGIN 3d_gamemanager
public class GameManager : Singleton<GameManager> {

    // The prefab to use for the ship, the place it starts from, 
    // and the current ship object
    public Material mat1;
    public Material mat2;
    public Material mat3;
    public Material mat4;
    public GameObject shipPrefab;
	public Transform shipStartPosition;
	public GameObject currentShip {get; private set;}

	// The prefab to use for the space station, the place it starts from, 
	// and the current ship object
	public GameObject spaceStationPrefab;
	public Transform spaceStationStartPosition;
	public GameObject currentSpaceStation {get; private set;}

	// The follow script on the main camera
	public SmoothFollow cameraFollow;

	// BEGIN 3d_gamemanager_timer
	// The game timer
	public Timer timer;
	// END 3d_gamemanager_timer

	// BEGIN 3d_gamemanager_boundary
	// The game's boundary
	public Boundary boundary;
	// END 3d_gamemanager_boundary

	// The containers for the various bits of UI
	public GameObject inGameUI;
	public GameObject pausedUI;
	public GameObject gameOverUI;
    public GameObject victoryUI;
    public GameObject mainMenuUI;
    public Text hpLabel;
	// BEGIN 3d_gamemanager_boundary
	// The warning UI that appears when we approach
	// the boundary
	public GameObject warningUI;
	// END 3d_gamemanager_boundary

	// Is the game currently playing?
	public bool gameIsPlaying {get; private set;}
    
	// The game's Asteroid Spawner
	public AsteroidSpawner asteroidSpawner;
    public EnemySpawner enemySpawner;

    // Keeps track of whether the game is paused or not.
    public bool paused;

    // BEGIN 3d_gamemanager_start
	// Show the main menu when the game starts
	void Start() {
		ShowMainMenu();
	}
    // END 3d_gamemanager_start
    
    // BEGIN 3d_gamemanager_showui
	// Shows a UI container, and hides all others.
	void ShowUI(GameObject newUI) {

		// Create a list of all UI containers.
		GameObject[] allUI = {inGameUI, pausedUI, gameOverUI, mainMenuUI};

		// Hide them all.
		foreach (GameObject UIToHide in allUI) {
			UIToHide.SetActive(false);
		}

		// And then show the provided UI container.
		newUI.SetActive(true);
	}
    // END 3d_gamemanager_showui
    
    // BEGIN 3d_gamemanager_showmain
	public void ShowMainMenu() {
		ShowUI(mainMenuUI);

		// We aren't playing yet when the game starts
		gameIsPlaying = false;

		// Don't spawn asteroids either
		asteroidSpawner.spawnAsteroids = false;
        enemySpawner.spawnEnemy = false;
	}
    // END 3d_gamemanager_showmain

    // BEGIN 3d_gamemanager_startgame
	// Called by the New Game button being tapped
    public void ChangeSkyBox()
    {
        Dropdown uiDropdown = GameObject.Find("GalaxyChange").GetComponent<Dropdown>();
        if (uiDropdown.value == 0)
        {
            RenderSettings.skybox = mat1;
                }
        if (uiDropdown.value == 1)
        {
            RenderSettings.skybox = mat2;
        }
        if (uiDropdown.value == 2)
        {
            RenderSettings.skybox = mat3;
        }
        if (uiDropdown.value == 3)
        {
            RenderSettings.skybox = mat4;
        }
    }

 

    public void StartGame() {
        Dropdown uiDropdown = GameObject.Find("Difficulty").GetComponent<Dropdown>();
        // Show the in-game UI
        ShowUI(inGameUI);
        victoryUI.active = false;

		// We're now playing
		gameIsPlaying = true;

		// If we happen to have a ship, destroy it
		if (currentShip != null) {
			Destroy(currentShip);
		}

		// Likewise for the station
		if (currentSpaceStation != null) {
			Destroy(currentSpaceStation);
		}

		// Create a new ship, and place it at the start position
		currentShip = Instantiate(shipPrefab);
		currentShip.transform.position = shipStartPosition.position;
		currentShip.transform.rotation = shipStartPosition.rotation;

		// And likewise for the station
		currentSpaceStation = Instantiate(spaceStationPrefab);
        
		currentSpaceStation.transform.position = 
            spaceStationStartPosition.position;
        
		currentSpaceStation.transform.rotation = 
            spaceStationStartPosition.rotation;

		// Make the follow script track the new ship
		cameraFollow.target = currentShip.transform;

		// Start spawning asteroids
		asteroidSpawner.spawnAsteroids = true;
        enemySpawner.spawnEnemy = true;

        // And aim the the spawner at the new space station
        asteroidSpawner.target = currentSpaceStation.transform;
        enemySpawner.target = currentShip.transform;
        // BEGIN 3d_gamemanager_timer
        timer.StartClock();
        // END 3d_gamemanager_timer

       if (uiDropdown.value == 0)
        {
            enemySpawner.spawnEnemy = false;
            asteroidSpawner.target = currentShip.transform;
        }
        if (uiDropdown.value == 1)
        {
            enemySpawner.spawnEnemy = false;
            asteroidSpawner.target = currentSpaceStation.transform;
        }
        if (uiDropdown.value == 2)
        {
            enemySpawner.spawnEnemy = true;
            asteroidSpawner.target = currentSpaceStation.transform;
        }

    }
    // END 3d_gamemanager_startgame

    // BEGIN 3d_gamemanager_gameover
	// Called by objects that end the game when they're destroyed
	public void GameOver() {
		// Show the game over UI
		ShowUI(gameOverUI);
		// We're no longer playing
		gameIsPlaying = false;
        var tt = GameObject.Find("TextTime").GetComponent<Text>();
        tt.text = "Your time: "+(timer.startTime - timer.timeRemaining);
        var sc = GameObject.Find("Score").GetComponent<Text>();
        sc.text = "Your score: " + (Mathf.Round((timer.startTime - timer.timeRemaining)*10));

        // Destroy the ship and the station
        if (currentShip != null)
			Destroy (currentShip);

		if (currentSpaceStation != null)
			Destroy (currentSpaceStation);

		// BEGIN 3d_gamemanager_boundary
		// Stop showing the warning UI, if it was visible
		warningUI.SetActive(false);
		// END 3d_gamemanager_boundary

		// Stop spawning asteroids
		asteroidSpawner.spawnAsteroids = false;
        enemySpawner.spawnEnemy = false;

        // And remove all lingering asteroids from the game
        asteroidSpawner.DestroyAllAsteroids();
        enemySpawner.DestroyAllEnemies();
    }

    public void Victory()
    {
        // Show the game over UI
        ShowUI(victoryUI);

        // We're no longer playing
        gameIsPlaying = false;

        var tt = GameObject.Find("TextTime").GetComponent<Text>();
        tt.text = "Your time: " + (timer.startTime - timer.timeRemaining);
        var sc = GameObject.Find("Score").GetComponent<Text>();
        sc.text = "Your score: " + (Mathf.Round((timer.startTime - timer.timeRemaining) * 10));



        // Destroy the ship and the station
        if (currentShip != null)
            Destroy(currentShip);

        if (currentSpaceStation != null)
            Destroy(currentSpaceStation);

        // BEGIN 3d_gamemanager_boundary
        // Stop showing the warning UI, if it was visible
        warningUI.SetActive(false);
        // END 3d_gamemanager_boundary

        // Stop spawning asteroids
        asteroidSpawner.spawnAsteroids = false;
        enemySpawner.spawnEnemy = false;

        // And remove all lingering asteroids from the game
        asteroidSpawner.DestroyAllAsteroids();
        enemySpawner.DestroyAllEnemies();
    }
    // END 3d_gamemanager_gameover

    // BEGIN 3d_gamemanager_setpaused
    // Called when the Pause or Resume buttons are tapped
    public void SetPaused(bool paused) {

		// Switch between the in-game and paused UI
		inGameUI.SetActive(!paused);
		pausedUI.SetActive(paused);

		// If we're paused..
		if (paused) {
			// Stop time
			Time.timeScale = 0.0f;
            AudioListener.pause = true;
		} else {
			// Resume time
			Time.timeScale = 1.0f;
            AudioListener.pause = false;
        }
	}

    public void SExit()
    {
        Application.Quit();
    }
    // END 3d_gamemanager_setpaused

    // BEGIN 3d_gamemanager_boundary
    public void Update() {
        if (currentSpaceStation != null)
            hpLabel.text = currentSpaceStation.GetComponent<DamageTaking>().hitPoints.ToString();
        

		// If we don't have a ship, bail out
		if (currentShip == null)
			return;

		// If the ship is outside the Boundary's Destroy Radius,
		// game over. If it's within the Destroy Radius, but outside
		// the Warning radius, show the Warning UI. If it's within both,
		// don't show the Warning UI.

		float distance = 
			(currentShip.transform.position 
				- boundary.transform.position).magnitude;

		if (distance > boundary.destroyRadius) {
			// The ship has gone beyond the destroy radius, so it's game over
			GameOver();
		} else if (distance > boundary.warningRadius) {
			// The ship has gone beyond the warning radius, so show the 
			// warning UI
			warningUI.SetActive(true);
		} else {
			// It's within the warning threshold, so don't show the warning UI
			warningUI.SetActive(false);
		}
	}
    
	// END 3d_gamemanager_boundary

}
// END 3d_gamemanager