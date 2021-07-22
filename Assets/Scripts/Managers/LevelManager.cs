﻿using System.Collections;
using UnityEngine;
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
    [Tooltip("Max number of cloned slimes allowed in the level")]
    public int maxClonedSlimesInLevel = 1000;
    [SerializeField]
    [Tooltip("Set the rotation speed in the LevelManager of this scene")]
    public float speed = 2f;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        terrainInLevel = GameObject.FindGameObjectWithTag("Terrain").GetComponent<MeshRenderer>();

        if(terrainInLevel == null)
        {
            Debug.LogError("Terrain not found in the scene. Have you assigned a 'Terrain' to your terrain?");
        }

        maxSlimesOfLevel = quantitySlimeCollector + quantitySlimeTactical + quantitySlimeBomb;
        Slime.maxGlobalClonesCount = maxClonedSlimesInLevel;
    }

    void Start()
    {
        InitializeHUD();

        if (tipCanvas != null)
            tipCanvas.SetActive(true);

        if (cannonInScene == null)
            throw new MissingReferenceException("Reference the cannon of the scene");

        // Analytics Start Level
        GameAnalyticsManager.instance.LogStartLevelEvent();
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
        StartCoroutine(SetGameOverCo());
    }

    private IEnumerator SetGameOverCo()
    {
        //player.FreezePlayer();
        isGameOver = true;
        yield return new WaitForSeconds(delayToShowGameOverPanel);
        ShowUI(gameOverPanel);
        // Analytics - Level failed
        GameAnalyticsManager.instance.LogFailLevelEvent();
    }

    public void SetVictory()
    {
        //starsWonInLevel = CalculateStarsWon(humansScared, initialObjectivesInLevel);
        StartCoroutine(SetVictoryCo());
    }

    private int CalculateStarsWon(int objectivesDestroyed, int initialObjectivesInLevel)
    {
        int stars = 3;
        //if (objectivesDestroyed < initialObjectivesInLevel)
        //    stars--;

        //if (collectablesInLevel > 0 && player.collectables < collectablesInLevel)
        //    stars--;

        return stars;
    }

    private IEnumerator SetVictoryCo()
    {
        //player.FreezePlayer();
        starsWonInLevel = CalculateStarsWon(0, 0);
        yield return new WaitForSeconds(delayToShowVictoryPanel);
        ShowUI(victoryMenu.gameObject);
        victoryMenu.SetStarsFromLevel();
        SaveLevelData();
        //Analytics - Level Complete
        GameAnalyticsManager.instance.LogCompleteLevelEvent(starsWonInLevel);
    }

    private void ShowUI(GameObject ui)
    {
        ui.SetActive(true);
    }

    public void IncrementHumansInfected()
    {
        humansInfected++;

        if (humansInfected >= totalHumansInLevel)
        {
            SetVictory();
        }

        HUD.instance.SetCurrentHumansInfected(humansInfected);
    }

    public void IncrementSlimeLaunched()
    {
        slimesLaunched++;

        if (slimesLaunched >= maxSlimesOfLevel)
            SetGameOver();

        HUD.instance.SetSlimesLaunched(slimesLaunched);
    }

    public void SaveLevelData()
    {
        LevelData currentLevel = new LevelData(SceneManager.GetActiveScene().buildIndex, starsWonInLevel);
        PlayerPersistence.TryToSaveCurrentLevel(currentLevel);
    }

    public void DecrementSlimeCollector()
    { 
        if(quantitySlimeCollector <= 0) return;

        quantitySlimeCollector--;
    }

    public void DecrementSlimeTactical()
    {
        if (quantitySlimeTactical <= 0) return;

        quantitySlimeTactical--;
    }

    public void DecrementSlimeBomb()
    {
        if (quantitySlimeBomb <= 0) return;

        quantitySlimeBomb--;
    }
}

