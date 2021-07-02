using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionUI : MonoBehaviour
{
    public int latestLevelPlayed = 1;
    public static LevelSelectionUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        latestLevelPlayed = PlayerPersistence.GetIndexOfLastLevelPlayed();
    }

    private void Start()
    {
        PlayerPersistence.LoadPlayerData();
    }

    public void LoadMainMenu()
    {
        CommonUI.LoadMainMenu();
    }
}
