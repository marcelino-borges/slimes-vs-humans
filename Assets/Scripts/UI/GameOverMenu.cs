using UnityEngine;
using Facebook.Unity;

public class GameOverMenu : MonoBehaviour
{
    public void LoadMainMenu()
    {
        CommonUI.LoadMainMenu();
    }

    public void ReloadLevel()
    {
        CommonUI.ReloadLevel();
    }
}
