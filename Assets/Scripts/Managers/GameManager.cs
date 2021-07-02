using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public UnityEvent onLanguageChangeEvent;
    public FontAssets fonts;

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
}
