using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public GameObject gameOverPanel;
    public VictoryMenu victoryMenu;
    public bool isGameOver = false;

    public int initialObjectivesInLevel = 0;
    public int totalObjectivesToDestroyInLevel = 0;
    public int objectivesDestroyed = 0;
    public int collectablesInLevel = 0;
    public float delayToShowUI = .5f;
    public int starsWonInLevel = 0;
    public GameObject tipCanvas;
    public float delayToShowGameOver = .5f;
    public Cannon cannonInScene;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        if (tipCanvas != null)
            tipCanvas.SetActive(true);

        if (cannonInScene == null)
            throw new MissingReferenceException("Reference the cannon of the scene");

        // Analytics Start Level
        GameAnalyticsManager.instance.LogStartLevelEvent();
    }

    public void SetGameOver()
    {
        StartCoroutine(SetGameOverCo());
    }

    private IEnumerator SetGameOverCo()
    {
        //player.FreezePlayer();
        isGameOver = true;
        yield return new WaitForSeconds(delayToShowGameOver);
        ShowUI(gameOverPanel);
        // Analytics - Level failed
        GameAnalyticsManager.instance.LogFailLevelEvent();
    }

    public void SetVictory()
    {
        starsWonInLevel = CalculateStarsWon(objectivesDestroyed, initialObjectivesInLevel);
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
        yield return new WaitForSeconds(delayToShowUI);
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

    public void CountDestroyedObjective()
    {
        objectivesDestroyed++;

        if (objectivesDestroyed >= totalObjectivesToDestroyInLevel)
            SetVictory();
    }

    public void SaveLevelData()
    {
        LevelData currentLevel = new LevelData(SceneManager.GetActiveScene().buildIndex, starsWonInLevel);
        PlayerPersistence.TryToSaveCurrentLevel(currentLevel);
    }

    public void DecreaseTotalObjectivesToDestroy(int amount)
    {
        totalObjectivesToDestroyInLevel--;
    }
}
