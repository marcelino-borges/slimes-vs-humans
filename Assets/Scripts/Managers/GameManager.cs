using MilkShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public UnityEvent onLanguageChangeEvent;
    public FontAssets fonts;
    public bool isVibrating;
    public float vibrationCooldownTime = .7f;
    public ShakePreset shakePreset;

    public static string OBSTACLE_TAG = "Obstacle";
    public static string BUILDING_TAG = "Building";
    public static string SLIME_TAG = "Slime";
    public static string HUMAN_TAG = "Human";
    public static string TERRAIN_TAG = "Terrain";

    //FOR THE PROTOTYPE ONLY
    [ReadOnly]
    public int nextLevelIndex = 0;
    public List<string> levels = new List<string>() 
    { 
        "Level001",
        "Level002",
        "Level003",
        "Level004",
        "Level005",
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        if (onLanguageChangeEvent == null)
            onLanguageChangeEvent = new UnityEvent();
    }

    void Start()
    {
        if (!PlayerPersistence.HasFileCreated())
        {
            //First time playing the game
            PlayerPersistence.SavePlayerData(null, true);
            SetDefaultPlayerConfigs();
        }
        else
        {
            //If not the first time playing
            RetrievePlayerConfigsSaved();
        }
    }
    private static void RetrievePlayerConfigsSaved()
    {
        PlayerOptionsConfig playerConfigs = PlayerPersistence.GetPlayerOptionsConfig();

        if (playerConfigs != null)
        {
            MusicManager.instance.Mute(!playerConfigs.musicOn);
            SoundManager.instance.Mute(!playerConfigs.sfxOn);

            Language savedLanguage = PlayerPersistence.GetPlayerLanguage();

            if (savedLanguage != Language.Null)
                LocalisationSystem.language = savedLanguage;
        }
    }

    public void SetDefaultPlayerConfigs()
    {
        MusicManager.instance.SetNewMusicVolume(MusicManager.instance.maxVolume);
        SoundManager.instance.CurrentVolume  = SoundManager.instance.maxVolume;

        PlayerPersistenceData playerData = PlayerPersistence.LoadPlayerData();
        PlayerOptionsConfig playerOptions = playerData.playerOptionsConfig;

        playerOptions.musicOn = !MusicManager.instance.isMuted;
        playerOptions.sfxOn = !SoundManager.instance.isMuted;

        playerData.playerOptionsConfig = playerOptions;

        PlayerPersistence.SavePlayerData(playerData);
    }

    public string GetNextLevel()
    {
        int levelIndexToPlay = nextLevelIndex;
        nextLevelIndex++;

        if (nextLevelIndex >= levels.Count)
        {
            nextLevelIndex = 0;
            ShuffleLevelsArray();
        }

        return levels[levelIndexToPlay];
    }

    public void ShuffleLevelsArray()
    {
        levels.Shuffle();
    }

    public void VibrateAndShake(bool vibrate = true)
    {
        if (!isVibrating)
        {
            if(vibrate)
                Handheld.Vibrate();
            ShakeCamera();
            StartCoroutine(CountVibrateCooldown(vibrationCooldownTime));
        }
    }

    private IEnumerator CountVibrateCooldown(float time)
    {
        isVibrating = true;
        yield return new WaitForSeconds(time);
        isVibrating = false;
    }

    public void ShakeCamera()
    {
        if (shakePreset != null)
            Shaker.ShakeAll(shakePreset);
    }
}
