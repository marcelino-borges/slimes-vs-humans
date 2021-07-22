using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SoundButton soundButton;
    public string nextLevelName = "LevelSelection";

    private void Start()
    {
        SetMuteIconVisible(MusicManager.instance.isMuted);
    }

    public void Play()
    {
        CommonUI.PlayButtonClickSfx();
        //SceneManager.LoadScene(nextLevelName);
        CommonUI.LoadNextLevel();
    }

    public void LoadSettingsMenu()
    {
        CommonUI.PlayButtonClickSfx();
        CommonUI.LoadSettingsMenu();
    }

    public void SetMuteIconVisible(bool visible)
    {
        soundButton.SetMuteIconVisible(visible);
    }
}
