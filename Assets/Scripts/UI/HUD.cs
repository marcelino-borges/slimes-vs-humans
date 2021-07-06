using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [SerializeField]
    private TextMeshProUGUI slimesLeft;
    [SerializeField]
    private TextMeshProUGUI slimesTotal;
    [SerializeField]
    private TextMeshProUGUI humansHit;
    [SerializeField]
    private TextMeshProUGUI humansTotal;
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject damagePanel;
    [SerializeField]
    private GameObject slimeTaticPrefab;
    [SerializeField]
    private GameObject slimeBombPrefab;
    [SerializeField]
    private GameObject slimeCollectorPrefab;
    public GameObject selectedSlime;

    private void Awake()
    { 
        instance = this;
    }

    private void Start()
    {
        SelectSlime1();
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

    public void SelectSlime1()
    {
        selectedSlime = slimeTaticPrefab;
    }

    public void SelectSlime2()
    {
        selectedSlime = slimeCollectorPrefab;
    }

    public void SelectSlime3()
    {
        selectedSlime = slimeBombPrefab;
    }
}
