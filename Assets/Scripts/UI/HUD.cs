using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [Header("UI TEXTS")]
    [SerializeField]
    private TextMeshProUGUI slimesLeft;
    [SerializeField]
    private TextMeshProUGUI slimesTotal;
    [SerializeField]
    private TextMeshProUGUI humansHit;
    [SerializeField]
    private TextMeshProUGUI humansTotal;
    [Space(20)]
    [Header("UI PANELS")]
    [SerializeField]
    private GameObject pausePanel;
    [SerializeField]
    private GameObject damagePanel;
    [Space(20)]
    [Header("SLIMES")]
    [SerializeField]
    private GameObject slimeTaticPrefab;
    [SerializeField]
    private GameObject slimeBombPrefab;
    [SerializeField]
    private GameObject slimeCollectorPrefab;
    public GameObject selectedSlime;
    public Slider launchProgressBar;

    private void Awake()
    { 
        instance = this;
    }

    private void Start()
    {
        SelectSlimeTatic();
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

    public void SelectSlimeTatic()
    {
        SelectSlime(slimeTaticPrefab);
    }

    public void SelectSlimeCollector()
    {
        SelectSlime(slimeCollectorPrefab);
    }

    public void SelectSlimeBomb()
    {
        SelectSlime(slimeBombPrefab);
    }

    private void SelectSlime(GameObject slimePrefab)
    {
        if (slimePrefab != null)
        {
            selectedSlime = slimePrefab;
        }
    }

    public void ClearSelectedSlime()
    {
        selectedSlime = null;
    }

    public void SetLaunchBarVisible(bool visible)
    {
        if(launchProgressBar.gameObject.activeSelf != visible)
            launchProgressBar.gameObject.SetActive(visible);
    }

    public void SetLaunchBarValue(float value)
    {
        launchProgressBar.value = value;
    }

    public void SetLaunchBarLimits(Vector2 limits)
    {
        launchProgressBar.minValue = limits.x;
        launchProgressBar.maxValue = limits.y;
    }

    public bool HasSlimeSelected()
    {
        return selectedSlime != null;
    }
}
