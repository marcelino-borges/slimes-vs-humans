using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class CardSlimeUI : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI humansDescription;
    [SerializeField]
    protected TextMeshProUGUI quantityLeft;
    [SerializeField]
    protected GameObject quantityLeftPlaceholder;
    [SerializeField]
    protected SlimeType slimeType;
    [SerializeField]
    protected Button button;
    [SerializeField]
    protected GameObject disableMask;
    [SerializeField]
    protected GameObject selectMask;
    protected Animator animator;

    private int quantityLeftCounter;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        switch(slimeType)
        {
            case SlimeType.COLLECTOR:
                if (LevelManager.instance.quantitySlimeCollector > 0)
                    SetQuantityLeft(LevelManager.instance.quantitySlimeCollector);
                else
                    DisableCard();
                break;
            case SlimeType.TACTICAL:
                if (LevelManager.instance.quantitySlimeTactical > 0)
                    SetQuantityLeft(LevelManager.instance.quantitySlimeTactical);
                else
                    DisableCard();
                break;
            case SlimeType.BOMB:
                if (LevelManager.instance.quantitySlimeBomb > 0)
                    SetQuantityLeft(LevelManager.instance.quantitySlimeBomb);
                else
                    DisableCard();
                break;
        }
    }

    public void DecrementQuantityLeft()
    {
        if (quantityLeftCounter <= 0) return;

        quantityLeftCounter--;
        SetQuantityLeft(quantityLeftCounter);

        if(quantityLeftCounter <= 0)
        {
            SetQuantityLeft(0);
            DisableCard();
        }
    }

    private void DisableCard()
    {
        button.interactable = false;
        disableMask.SetActive(true);
        Deselect();
    }

    public void SetQuantityLeft(int quantity) 
    {
        quantityLeftCounter = quantity;
        quantityLeft.text = quantityLeftCounter.ToString();
    }

    public void Select()
    {
        selectMask.SetActive(true);
        animator.Play("Select");
    }

    public void Deselect()
    {
        selectMask.SetActive(false);
        animator.Play("Deselect");
    }
}
