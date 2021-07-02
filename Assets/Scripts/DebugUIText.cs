using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUIText : MonoBehaviour
{
    public static DebugUIText instance;
    public TextMeshProUGUI textElement;
    public bool canShowText = true;
    private bool isCollectingGarbage = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        textElement.text = "";
    }

    void Start()
    {
        if(textElement == null)
            textElement = GetComponent<TextMeshProUGUI>();

        ShowText("teste");
    }

    public void ShowText(string text)
    {
        if (!canShowText) return;

        textElement.text += textElement.text.Length > 0 ? "\n" + text : text;
        if(!isCollectingGarbage)
            StartCoroutine(HideText());
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(3f);

        if (textElement.text.Length > 0)
        {
            isCollectingGarbage = true;

            string finalText = "";
            string[] lines = textElement.text.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                finalText += lines[i] + "\n";
            }

            textElement.text = finalText;

            if (textElement.text.Length > 0)
                StartCoroutine(HideText());
            else
                isCollectingGarbage = false;
        }

    }
}
