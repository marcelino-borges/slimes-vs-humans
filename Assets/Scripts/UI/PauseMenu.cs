using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject settingsPanel;

    public void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void ReloadLevel()
    {
        CommonUI.ReloadLevel();
    }

    public void LoadMainMenu()
    {
        CommonUI.LoadMainMenu();
    }

    public void ClosePauseMenu()
    {
        CommonUI.PlayButtonClickSfx();
        gameObject.SetActive(false);
    }
}
