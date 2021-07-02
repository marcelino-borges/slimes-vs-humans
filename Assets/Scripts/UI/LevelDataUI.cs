using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDataUI : MonoBehaviour
{
    public int buildIndex;
    public TextMeshProUGUI levelNameTxt;
    public GameObject[] stars;
    [HideInInspector]
    public Button button;
    public GameObject padlockImage;
    public Sprite cookieNew;
    public Sprite cookieBite;

    public Image image;


    public static int latestLevelPersisted = 0;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        levelNameTxt.text = buildIndex < 10 ? "0" + buildIndex : buildIndex.ToString();

        PlayerPersistenceData playerData = PlayerPersistence.LoadPlayerData();

        if(playerData != null)
        {
            LevelData levelFound = PlayerPersistence.GetLevelPersisted(buildIndex);

            if (levelFound != null)
            {
                int stars = levelFound.stars;

                image.sprite = cookieBite;

                SetStars(stars);

                MakeInteractable(true);
            }
            else
            {
                if (playerData.levelsPlayed != null)
                {
                    if (buildIndex == playerData.levelsPlayed.Count + 1)
                        MakeInteractable(true);
                }
                else
                {
                    if (buildIndex == 1)
                        MakeInteractable(true);
                }
            }
        }
    }

    private void SetStars(int starsInLevel)
    {
        for (int i = 0; i < starsInLevel; i++)
        {
            stars[i].SetActive(true);
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(buildIndex);    
    }

    public void MakeInteractable(bool interactable)
    {
        button.interactable = interactable;
        levelNameTxt.gameObject.SetActive(interactable);
        padlockImage.SetActive(!interactable);        
    }
}
