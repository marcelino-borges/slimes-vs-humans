using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    public GameObject muteIcon;

    public void SetMuteIconVisible(bool show)
    {
        if (muteIcon != null)
            muteIcon.SetActive(show);
    }
}
