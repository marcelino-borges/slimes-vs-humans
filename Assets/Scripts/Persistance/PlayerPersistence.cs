using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    public static readonly string FILE_PATH = Application.persistentDataPath + "/PlayerData.dat";
    public static readonly string LANGUAGE_KEY = "language";

    public static PlayerPersistenceData SavePlayerData(PlayerPersistenceData playerPersistenceData, bool saveLanguage = false)
    {
        PlayerPersistenceData playerDataToSave;

        if (playerPersistenceData == null)
        {
            //Only when when playing for the 1st time
            playerDataToSave = new PlayerPersistenceData();
        }
        else
        {
            playerDataToSave = playerPersistenceData;
        }

        if (playerDataToSave.playerOptionsConfig == null)
        {
            playerDataToSave.playerOptionsConfig = new PlayerOptionsConfig();
        }

        if (saveLanguage)
            playerDataToSave.playerOptionsConfig.language = LocalisationSystem.language;

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(FILE_PATH);
        binaryFormatter.Serialize(file, playerDataToSave);
        file.Close();
        return playerDataToSave;
    }

    public static PlayerPersistenceData LoadPlayerData()
    {
        try
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(FILE_PATH, FileMode.Open);
            PlayerPersistenceData playerPersistenceData = null;
            playerPersistenceData = binaryFormatter.Deserialize(file) as PlayerPersistenceData;
            file.Close();

            return playerPersistenceData;
        }
        catch
        {
            return SavePlayerData(null, true);
        }
    }

    public static void DeleteLocalFiles()
    {
        //Only to windows
        File.Delete(FILE_PATH);
    }

    public static bool HasFileCreated()
    {
        //If theere is no file,
        //user is playing for the first time
        return File.Exists(FILE_PATH);
    }

    public static Language GetPlayerLanguage()
    {
        if (HasFileCreated())
        {
            PlayerPersistenceData playerData = LoadPlayerData();
            if (playerData.playerOptionsConfig != null)
            {
                foreach (Language languageType in EnumUtil.GetValues<Language>())
                {
                    if (languageType == playerData.playerOptionsConfig.language)                    
                        return languageType;                    
                }
            }
        }
        return Language.Null;
    }

    public static void SavePlayerLanguage()
    {
        PlayerPersistenceData playerData = LoadPlayerData();

        if (playerData.playerOptionsConfig == null)
            playerData.playerOptionsConfig = new PlayerOptionsConfig();

        playerData.playerOptionsConfig.language = LocalisationSystem.language;
        SavePlayerData(playerData, true);
    }

    public static int GetIndexOfLastLevelPlayed()
    {
        if (HasFileCreated())
        {
            PlayerPersistenceData playerData = LoadPlayerData();
            if (playerData.levelsPlayed != null && playerData.levelsPlayed.Count > 0)
                return playerData.levelsPlayed[playerData.levelsPlayed.Count - 1].buildIndex;
        }
        return 1;
    }

    public static void TryToSaveCurrentLevel(LevelData level)
    {
        if (HasFileCreated())
        {
            PlayerPersistenceData playerData = LoadPlayerData();

            if (playerData != null)
            {
                if (playerData.levelsPlayed == null)
                    playerData.levelsPlayed = new List<LevelData>();

                if (!playerData.levelsPlayed.Contains(level))
                    playerData.levelsPlayed.Add(level);
                else                
                    playerData.levelsPlayed[playerData.levelsPlayed.IndexOf(level)] = level;

                SavePlayerData(playerData);
            }
        }
    }

    public static LevelData GetLevelPersisted(int levelBuildIndex)
    {
        if (HasFileCreated())
        {
            PlayerPersistenceData playerData = LoadPlayerData();

            if (playerData != null)
            {
                if (playerData.levelsPlayed != null)
                {
                    LevelData level = new LevelData(levelBuildIndex);
                    if (playerData.levelsPlayed.Contains(level))
                        return playerData.levelsPlayed[playerData.levelsPlayed.IndexOf(level)];
                }
            }
        }
        return null;
    }

    public static PlayerOptionsConfig GetPlayerOptionsConfig()
    {
        if (HasFileCreated())
        {
            PlayerPersistenceData playerData = LoadPlayerData();
            if (playerData != null)
            {
                return playerData.playerOptionsConfig;
            }
        }
        return null;
    }

    public static void SavePlayerConfigs(PlayerOptionsConfig playerConfigs)
    {
        if (HasFileCreated())
        {
            PlayerPersistenceData playerData = LoadPlayerData();
            playerData.playerOptionsConfig = playerConfigs;
            SavePlayerData(playerData, true);
        }
    }
}

[Serializable]
public class PlayerPersistenceData
{
    public string playerName;
    public int score;    
    
    [SerializeField]
    public List<LevelData> levelsPlayed;

    [SerializeField]
    public PlayerOptionsConfig playerOptionsConfig;

    public void SetPlayerData(Player player)
    {
        this.score = player.score;
    }
}

[Serializable]
public class LevelData
{
    public int buildIndex;
    public int stars;

    public LevelData(int buildIndex, int stars)
    {
        this.buildIndex = buildIndex;
        this.stars = stars;
    }

    public LevelData(int buildIndex)
    {
        this.buildIndex = buildIndex;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is LevelData))
            return false;

        if (obj == this)
            return true;

        return (obj as LevelData).buildIndex == this.buildIndex;
    }
    public override int GetHashCode()
    {
        return buildIndex.GetHashCode();
    }
}

[Serializable]
public class PlayerOptionsConfig
{
    [SerializeField]
    public Language language;
    public bool sfxOn, musicOn;

    public PlayerOptionsConfig()
    {
        language = Language.Null;
    }
}


