using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;
    public Toggle musicToggle;
    public Toggle sfxToggle;
    private PlayerOptionsConfig playerConfigs;

    private void Start()
    {
        playerConfigs = PlayerPersistence.GetPlayerOptionsConfig();

        languageDropdown.ClearOptions();

        List<string> languages = EnumUtil.GetValues<Language>().ToList().Select(lang => lang.ToString()).ToList();
        languages.Remove("Null");

        languageDropdown.AddOptions(languages);
        languageDropdown.value = languages.IndexOf(LocalisationSystem.language.ToString());
        languageDropdown.RefreshShownValue();

        musicToggle.isOn = playerConfigs.musicOn;
        sfxToggle.isOn = playerConfigs.sfxOn;

        if(musicToggle != null)
            musicToggle.onValueChanged.AddListener(value => MuteMusic(value));
        if (sfxToggle != null)
            sfxToggle.onValueChanged.AddListener(value => MuteSfx(value));        
    }

    public void HandleLanguageDropdown()
    {
        CommonUI.PlayButtonClickSfx();
        Language language = Language.Null;
        Enum.TryParse(languageDropdown.options[languageDropdown.value].text, out language);

        if (language != Language.Null)
        {
            LocalisationSystem.language = language;
            GameManager.instance.onLanguageChangeEvent.Invoke();
            PlayerPersistence.SavePlayerLanguage();
        }
    }

    public void LoadMainMenu()
    {
        CommonUI.PlayButtonClickSfx();
        CommonUI.LoadMainMenu();        
    }

    public void CloseSettings()
    {
        CommonUI.PlayButtonClickSfx();
        gameObject.SetActive(false);
    }

    public void MuteMusic(bool isOn)
    {
        CommonUI.PlayButtonClickSfx();
        MusicManager.instance.Mute(!isOn);
        playerConfigs.musicOn = isOn;
    }

    public void MuteSfx(bool isOn)
    {
        CommonUI.PlayButtonClickSfx();
        SoundManager.instance.Mute(!isOn);
        playerConfigs.sfxOn = isOn;
    }
}
