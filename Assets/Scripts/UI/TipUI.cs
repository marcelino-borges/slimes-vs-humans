using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipUI : MonoBehaviour
{
    private void OnEnable()
    {
        //LevelManager.instance.player.FreezePlayer();
    }

    public void CloseTip()
    {
        // Analytics - Tutorial Complete

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //LevelManager.instance.player.UnfreezePlayer();
    }

}
