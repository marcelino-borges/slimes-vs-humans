/*using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour, IUnityAdsListener
{
    public static UnityAds instance;

#if UNITY_IOS
    private const string gameId = "3950914";
#elif UNITY_ANDROID || UNITY_EDITOR
    private const string gameId = "3950915";
#endif

    [Header("PLACEMENT TYPES")]
    public string videoPlacement = "video";
    public string interstitialPlacement = "interstitialVideo";
    public string rewardedPlacement = "rewardedVideo";
    public string bannerPlacement = "banner";

    [Header("IS FOR TESTING?")]
#if UNITY_EDITOR
    public bool isTest = true;
#else
    public bool isTest = false;
#endif

    public bool canShowAd = true;

    public float adsCooldownInMinutes = 1f;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        Advertisement.AddListener(this);

        Initialize();
    }

    public void Initialize()
    {
        Advertisement.Initialize(gameId, isTest);
    }

    private IEnumerator ShowAd(string placement)
    {
        if (canShowAd)
        {
            if (!Advertisement.IsReady(placement))
            {
                yield return new WaitForSeconds(.5f);                
            }
            Advertisement.Show(placement);
            StartCoroutine(CountShowAdCooldown());
        }
    }

    private IEnumerator CountShowAdCooldown()
    {
        canShowAd = false;
        yield return new WaitForSeconds(adsCooldownInMinutes * 60);
        canShowAd = true;
    }

    public void ShowVideo()
    {
        StartCoroutine(ShowAd(videoPlacement));
    }

    public void ShowInterstitial()
    {
        StartCoroutine(ShowAd(interstitialPlacement));
    }

    public void ShowRewardedVideo()
    {
        StartCoroutine(ShowAd(rewardedPlacement));
    }

    public void ShowBanner()
    {
        StartCoroutine(ShowAd(bannerPlacement));
    }

    public void OnUnityAdsReady(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidError(string message)
    {
#if UNITY_EDITOR
        Debug.LogError("Erro ao exibir anúncio. Mensagem: " + message);
#endif
    }

    public void OnUnityAdsDidStart(string placementId)
    {
#if UNITY_EDITOR
        Debug.Log("Exibindo anúncio de id: " + placementId);
#endif
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch(showResult)
        {
            case ShowResult.Failed:
#if UNITY_EDITOR
                Debug.LogError("Erro ao exibir anúncio de id: " + placementId);
#endif
                break;
            case ShowResult.Finished:
#if UNITY_EDITOR
                Debug.Log("Finalizou anúncio de id: " + placementId);
#endif

                break;
            case ShowResult.Skipped:
#if UNITY_EDITOR
                Debug.Log("Pulou anúncio de id: " + placementId);
#endif

                break;
        }
    }
}
*/