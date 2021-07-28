using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameOverChecking : MonoBehaviour
{
    [SerializeField] private float countDown = 3f;

    private UnityEvent gameOverEvent;

    private void Start()
    {
        if (gameOverEvent == null)
        {
            gameOverEvent = new UnityEvent();
            gameOverEvent.AddListener(CallGameOver);
        }       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && gameOverEvent != null)
        {
            gameOverEvent.Invoke();
        }
    }

    private void CallGameOver()
    {
        StopCoroutine(GameOverCD());
        StartCoroutine(GameOverCD());
    }

    private IEnumerator GameOverCD()
    {
        yield return new WaitForSeconds(countDown);

        //print("Sim, game over!!!");
    }
}
