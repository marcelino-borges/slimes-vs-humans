using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [ReadOnly]
    [Tooltip("Attribute set in the end of the level to store the amount of stars player has won in this level")]
    public int starsWonInLevel = 0;
    [ReadOnly]
    public bool isGameOver = false;
    [ReadOnly]
    public bool isLevelWon = false;
    [ReadOnly]
    public int slimesLaunched = 0;
    [ReadOnly]
    [Tooltip("After the play this attribute will store to total slimes the player can use in this level")]
    public int maxSlimesOfLevel = 20;

    [ReadOnly]
    public int humansInfected = 0;
    [ReadOnly]
    public int totalHumansInLevel = 0;

    [Space(10)]
    [Header("- REFERENCES TO ASSIGN")]
    [Space(20)]
    public GameObject gameOverPanel;
    public VictoryMenu victoryMenu;
    public GameObject tipCanvas;
    public Cannon cannonInScene;
    public MeshRenderer terrainInLevel;

    [Space(10)]
    [Header("- LEVEL INITIAL SETTINGS")]
    [Space(20)]
    public float delayToShowVictoryPanel = .5f;
    public float delayToShowGameOverPanel = .5f;
    public float timeToCheckGameOver = 3f;
    [Tooltip("Max number of cloned slimes allowed in the level")]
    public int maxClonedSlimesInLevel = 1000;
    [SerializeField]
    [Tooltip("Set the rotation speed in the LevelManager of this scene")]
    public float terrainRotationSpeed = 2f;
    [Header("Slimes available for the player")]
    [Tooltip("The number of collector slimes available in the card for the player. " +
             "0 for making the respective card unavailable")]
    public int quantitySlimeCollector = 0;
    [Tooltip("The number of tactical slimes available in the card for the player. " +
             "0 for making the respective card unavailable")]
    public int quantitySlimeTactical = 0;
    [Tooltip("The number of bomb slimes available in the card for the player. " +
             "0 for making the respective card unavailable")]
    public int quantitySlimeBomb = 0;
    public SlimeType initialSlimeSelected = SlimeType.NONE;
    public UnityEvent OnGameOverEvent;
    public UnityEvent OnVictoryEvent;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (OnVictoryEvent == null)
            OnVictoryEvent = new UnityEvent();

        terrainInLevel = GameObject.FindGameObjectWithTag("Terrain").GetComponent<MeshRenderer>();

#if UNITY_EDITOR

        if(terrainInLevel == null)
        {
            Debug.LogError("Terrain not found in the scene. Have you assigned a 'Terrain' to your terrain?");
        }
#endif
        maxSlimesOfLevel = quantitySlimeCollector + quantitySlimeTactical + quantitySlimeBomb;
        Slime.maxGlobalClonesCount = maxClonedSlimesInLevel;
    }

    void Start()
    {
        InitializeHUD();

        if (tipCanvas != null)
        {
            TerrainRotation.instance.Stop();
            tipCanvas.SetActive(true);
        }

        if (cannonInScene == null)
            throw new MissingReferenceException("Reference the cannon of the scene");

        // Analytics Start Level
        GameAnalyticsManager.instance.LogStartLevelEvent();
        SelectInitialCard();
    }

    private void InitializeHUD()
    {
        HUD.instance.SetMaxSlimesOfLevel(maxSlimesOfLevel);
        HUD.instance.SetTotalHumans(totalHumansInLevel);
        HUD.instance.SetCurrentHumansInfected(0);
        HUD.instance.SetSlimesLaunched(0);
    }

    public void IncrementHumanTotal()
    {
        totalHumansInLevel++;
        HUD.instance.SetTotalHumans(totalHumansInLevel);
    }

    public void DecrementHumanTotal()
    {
        totalHumansInLevel--;
        HUD.instance.SetTotalHumans(totalHumansInLevel);
    }

    /// <summary>
    /// Gets the min and max X of the terrain
    /// Based in the size of the GameObject/mesh tagged with "Terrain" placed in the scene.
    /// </summary>
    public Vector2 GetLevelXBounds()
    {
        return new Vector2(GetLevelMinX(), GetLevelMaxX());
    }

    /// <summary>
    /// Gets the min and max Z of the terrain
    /// Based in the size of the GameObject/mesh tagged with "Terrain" placed in the scene.
    /// </summary>
    public Vector2 GetLevelZBounds()
    {
        return new Vector2(GetLevelMinZ(), GetLevelMaxZ());
    }

    /// <summary>
    /// Gets a point where both X and Z are the minimum positions of the terrain
    /// Based in the size of the GameObject/mesh tagged with "Terrain" placed in the scene.
    /// </summary>
    public Vector2 GetLevelMinPoint()
    {
        return new Vector2(GetLevelMinX(), GetLevelMinZ());
    }

    /// <summary>
    /// Gets the minimum value in X the terrain occupies
    /// </summary>
    /// <returns>X value</returns>
    public float GetLevelMinX()
    {
        return terrainInLevel.bounds.center.x - (terrainInLevel.bounds.size.x / 2);
    }

    /// <summary>
    /// Gets the maximum value in X the terrain occupies
    /// </summary>
    /// <returns>X value</returns>
    public float GetLevelMaxX()
    {
        return terrainInLevel.bounds.center.x + (terrainInLevel.bounds.size.x / 2);
    }

    /// <summary>
    /// Gets a point where both X and Z are the maximum positions of the terrain
    /// Based in the size of the GameObject/mesh tagged with "Terrain" placed in the scene.
    /// </summary>
    /// <returns>Vector2</returns>
    public Vector2 GetLevelMaxPoint()
    {
        return new Vector2(GetLevelMaxX(), GetLevelMaxZ());
    }

    /// <summary>
    /// Gets the minimum value in Z the terrain occupies
    /// </summary>
    /// <returns>Z value</returns>
    public float GetLevelMinZ()
    {
        return terrainInLevel.bounds.center.z - (terrainInLevel.bounds.size.z / 2);
    }

    /// <summary>
    /// Gets the maximum value in Z the terrain occupies
    /// </summary>
    /// <returns>Z value</returns>
    public float GetLevelMaxZ()
    {
        return terrainInLevel.bounds.center.z + (terrainInLevel.bounds.size.z / 2);
    }

    public void SetGameOver()
    {
        //if (isLevelWon || isGameOver) return;

        if (isLevelWon || isGameOver) return;

        StopCoroutine(WaitAndCheckGameOver());
        StartCoroutine(WaitAndCheckGameOver());
    }

    private IEnumerator WaitAndCheckGameOver()
    {
        yield return new WaitForSeconds(timeToCheckGameOver);
        isGameOver = true;
        yield return new WaitForSeconds(delayToShowGameOverPanel);
        SetVictory();
        //ShowUI(gameOverPanel);
        // Analytics - Level failed
        //GameAnalyticsManager.instance.LogFailLevelEvent();
    }

    private int CalculateStarsWon()
    {
        int stars = 3;

        float percentInfected = 100 * humansInfected / totalHumansInLevel;

        if (percentInfected < 90 && percentInfected > 50)
            stars = 2;
        else if (percentInfected <= 50 && percentInfected > 25)
            stars = 1;
        else
            stars = 0;

        return stars;
    }

    public void SetVictory()
    {
        //starsWonInLevel = CalculateStarsWon(humansScared, initialObjectivesInLevel);
        if (isLevelWon) return;
        OnVictoryEvent.Invoke();
        StartCoroutine(SetVictoryCo());
    }

    private IEnumerator SetVictoryCo()
    {
        starsWonInLevel = CalculateStarsWon();
        isLevelWon = true;
        yield return new WaitForSeconds(delayToShowVictoryPanel);
        ShowUI(victoryMenu.gameObject);
        victoryMenu.SetStarsFromLevel(starsWonInLevel);
        SaveLevelData();
        //Analytics - Level Complete
        GameAnalyticsManager.instance.LogCompleteLevelEvent();
    }

    private void ShowUI(GameObject ui)
    {
        ui.SetActive(true);
    }

    public void IncrementHumansInfected()
    {
        humansInfected++;
        HUD.instance.SetCurrentHumansInfected(humansInfected);

        if (humansInfected >= totalHumansInLevel)
        {
            SetVictory();
        }
    }

    public void IncrementSlimeLaunched()
    {
        //slimesLaunched++;

        //if (slimesLaunched >= maxSlimesOfLevel)
        //    SetGameOver();

        //HUD.instance.SetSlimesLaunched(slimesLaunched);
    }

    public void SaveLevelData()
    {
        LevelData currentLevel = new LevelData(SceneManager.GetActiveScene().buildIndex, starsWonInLevel);
        PlayerPersistence.TryToSaveCurrentLevel(currentLevel);
    }

    public void DecrementSlimeCollector()
    { 
        if(quantitySlimeCollector <= 0) return;

        slimesLaunched++;
        quantitySlimeCollector--;

        if (IsCollectorAndTacticalOver())
            SetGameOver();
    }

    public void DecrementSlimeTactical()
    {
        if (quantitySlimeTactical <= 0) return;

        slimesLaunched++;
        quantitySlimeTactical--;

        if (IsCollectorAndTacticalOver())
            SetGameOver();
    }

    public void DecrementSlimeBomb()
    {
        if (quantitySlimeBomb <= 0) return;

        slimesLaunched++;
        quantitySlimeBomb--;
    }

    private bool IsCollectorAndTacticalOver()
    {
        return quantitySlimeCollector <= 0 && quantitySlimeTactical <= 0;
    }

    public bool IsGameActive()
    {
        return !isGameOver && !isLevelWon;
    }

    private void SelectInitialCard()
    {
        HUD.instance.SelectInitialCard(initialSlimeSelected);
    }

    public void CreateGameOverEvent()
    {
        OnGameOverEvent = new UnityEvent();
        OnGameOverEvent.AddListener(SetGameOver);
    }
}

