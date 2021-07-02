using UnityEngine;

public class AdWatchPopUp : MonoBehaviour
{
    public void WatchAd()
    {
        //UnityAds.instance.ShowRewardedVideo();
    }

    public void ClosePopUp()
    {
        gameObject.SetActive(false);
    }
}
