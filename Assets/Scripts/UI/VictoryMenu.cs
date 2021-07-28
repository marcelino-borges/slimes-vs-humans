using System.Collections;
using UnityEngine;

public class VictoryMenu : MonoBehaviour
{
    public StarUI[] starsUI;
    public float delayBetweenEachStarActivation = 1f;
    public AudioClip victorySfx;

    public GameObject WatchAdPopUp;

    private Animator animator;

    public bool hasShownStars = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetStarsFromLevel(int starsWon)
    {
        animator.Play("Show");

        if (victorySfx != null)
            SoundManager.instance.PlaySound2D(victorySfx);

        hasShownStars = true;
        StartCoroutine(SetStarsFromLevelCo(starsWon));
    }

    private IEnumerator SetStarsFromLevelCo(int starsWon)
    {
        // Analytics - Check High Score Achieved
        // if (stars == 3)

        for (int i = 0; i < starsWon; i++)
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
