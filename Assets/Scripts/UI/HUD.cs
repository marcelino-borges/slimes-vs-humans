using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [Header("UI TEXTS")]
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
    [ReadOnly]
    public GameObject selectedSlime;
    public Slider launchProgressBar;
    public CardSlimeUI collectorCard;
    public CardSlimeUI tacticalCard;
    public CardSlimeUI bombCard;
    public CardSlimeUI cardSelected;

    private void Awake()
    { 
        if(instance == null)
            instance = this;
    }

    public void ShowPauseMenu()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void SelectInitialCard(SlimeType type)
    {
        switch(type)
        {
            case SlimeType.COLLECTOR:
                SelectCard(collectorCard);
                SelectSlime(slimeCollectorPrefab);
                break;
            case SlimeType.TACTICAL:
                SelectCard(tacticalCard);
                SelectSlime(slimeTaticPrefab);
                break;
            case SlimeType.BOMB:
                SelectCard(bombCard);
                SelectSlime(slimeBombPrefab);
                break;
        }
        
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

    public void SelectSlimeTatic(CardSlimeUI card)
    {
        if (LevelManager.instance.quantitySlimeTactical <= 0 || !LevelManager.instance.IsGameActive()) return;
        ClearSelectedSlime();
        SelectCard(card);
        SelectSlime(slimeTaticPrefab);
    }

    public void SelectSlimeCollector(CardSlimeUI card)
    {
        if (LevelManager.instance.quantitySlimeCollector <= 0 || !LevelManager.instance.IsGameActive()) return;
        ClearSelectedSlime();
        SelectCard(card);
        SelectSlime(slimeCollectorPrefab);
    }

    public void SelectSlimeBomb(CardSlimeUI card)
    {
        if (LevelManager.instance.quantitySlimeBomb <= 0 || !LevelManager.instance.IsGameActive()) return;
        ClearSelectedSlime();
        SelectCard(card);
        SelectSlime(slimeBombPrefab);
    }

    private void SelectCard(CardSlimeUI card)
    {
        if (card != null)
        {
            card.Select();
            cardSelected = card;
        }
    }

    public void SelectSlime(GameObject slimePrefab)
    {
        if (slimePrefab != null)
        {
            selectedSlime = slimePrefab;
            LevelManager.instance.cannonInScene.InstantiateSlime(selectedSlime);
            LevelManager.instance.cannonInScene.SetCrossMarkVisibleInInitialPosition();
        }
    }

    public void ClearSelectedSlime()
    {
        selectedSlime = null;
        if (cardSelected != null)
        {
            cardSelected.Deselect();
            cardSelected = null;
        }

        if(LevelManager.instance.cannonInScene.slimeInstantiated != null)
        {
            Destroy(LevelManager.instance.cannonInScene.slimeInstantiated.gameObject);
        }
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

    public void SetTotalHumans(int total)
    {
        humansTotal.text = total < 10 ? "0" + total : total.ToString();
    }

    public void SetMaxSlimesOfLevel(int total)
    {
        //slimesTotal.text = total < 10 ? "0" + total : total.ToString();
    }

    public void SetCurrentHumansInfected(int number)
    {
        humansHit.text = number < 10 ? "0" + number : number.ToString();
    }

    public void SetSlimesLaunched(int number)
    {
        //slimesLeft.text = number < 10 ? "0" + number : number.ToString();
    }

    public void ReloadLevel()
    {
        CommonUI.ReloadLevel();
    }
}

