using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct FontAssets
{
    public TMP_FontAsset chinese;
    public TMP_FontAsset japonese;
    public TMP_FontAsset korean;
    public TMP_FontAsset thai;
    public TMP_FontAsset latin;
}

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocaliserUI : MonoBehaviour
{
    public enum ConcatenateType
    {
        ReplaceOriginal,
        BeforeOriginal,
        AfterOriginal
    }

    public ConcatenateType concatenateType = ConcatenateType.ReplaceOriginal;

    TextMeshProUGUI textField;
    
    public LocalisedString localisedString;

    private void Start() {
        textField = GetComponent<TextMeshProUGUI>();
        if (GameManager.instance.onLanguageChangeEvent != null)
            GameManager.instance.onLanguageChangeEvent.AddListener(UpdateLanguage);
        UpdateLanguage();
    }

    public void UpdateLanguage() {
        switch (concatenateType) {
            case ConcatenateType.ReplaceOriginal:
                textField.text = localisedString.value;
                break;
            case ConcatenateType.BeforeOriginal:
                textField.text = localisedString.value + textField.text;
                break;
            case ConcatenateType.AfterOriginal:
                textField.text = textField.text + localisedString.value;
                break;
        }
        SetFontFromLanguage();
    }



    public void SetFontFromLanguage()
    {
        switch (LocalisationSystem.language)
        {
            case Language.Chinese:
                textField.font = GameManager.instance.fonts.chinese;
                break;
            case Language.Japonese:
                textField.font = GameManager.instance.fonts.japonese;
                break;
            case Language.Thai:
                textField.font = GameManager.instance.fonts.thai;
                break;
            case Language.Korean:
                textField.font = GameManager.instance.fonts.korean;
                break;
            default:
                textField.font = GameManager.instance.fonts.latin;
                break;
        }
    }
}
