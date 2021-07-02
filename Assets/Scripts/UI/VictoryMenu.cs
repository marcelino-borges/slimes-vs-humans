using System.Collections;
using UnityEngine;

public class VictoryMenu : MonoBehaviour
{
    public StarUI[] starsUI;
    public float delayBetweenEachStarActivation = 1f;
    public AudioClip victorySfx;

    public GameObject WatchAdPopUp;

    private bool hasShownStars = false;

    public void SetStarsFromLevel()
    {
        if (victorySfx != null)
            SoundManager.instance.PlaySound2D(victorySfx);

        StartCoroutine(SetStarsFromLevelCo());
    }

    private IEnumerator SetStarsFromLevelCo()
    {
        int stars = LevelManager.instance.starsWonInLevel;

        // Analytics - Check High Score Achieved
        // if (stars == 3)

        for (int i = 0; i < stars; i++)
        {
            yield return new WaitForSeconds(delayBetweenEachStarActivation);
            starsUI[i].Activate();
        }
        yield return new WaitForSeconds(delayBetweenEachStarActivation);

        hasShownStars = true;
    }

    public void LoadNextLevel()
    {
        if (hasShownStars)
        {
            CommonUI.LoadNextLevel();
            //UnityAds.instance.ShowVideo();
        }
    }
    public void LoadMainMenu()
    {
        if (hasShownStars)
            CommonUI.LoadMainMenu();
    }

    public void ReloadLevel()
    {
        if (hasShownStars)
            CommonUI.ReloadLevel();
    }
}
