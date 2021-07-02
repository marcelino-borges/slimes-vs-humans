using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD instance;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public GameObject pausePanel;
    public GameObject damagePanel;

    private void Awake()
    { 
        instance = this;
    }

    private void Start()
    {
    }

    public void SetHealth(int health)
    {
        healthText.text = health < 10 ? "0" + health : health.ToString();
    }

    public void SetScore(int score)
    {
        scoreText.text = score < 10 ? "0" + score : score.ToString();
    }

    public void ShowPauseMenu()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void BlinkDamagePanel()
    {
        StartCoroutine(BlinkDamagePanelCo());
    }

    private IEnumerator BlinkDamagePanelCo()
    {
        damagePanel.SetActive(true);
        yield return new WaitForSeconds(.1f);
        damagePanel.SetActive(false);
    }
}
