using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameAnalyticsManager : MonoBehaviour
{
    public static GameAnalyticsManager instance;

    void Awake()
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

        GameAnalytics.Initialize();
    }

    #region LOG PROGRESSION EVENTS
    /// <summary>
    /// To add a progression event call
    /// </summary>
    /// <param name="progressionStatus">Status (enum) of added progression (start, complete, fail).</param>
    /// <param name="score">Score earned in the level</param>
    private void LogEventWithScore(GAProgressionStatus progressionStatus, int score)
    {
        GameAnalytics.NewProgressionEvent(progressionStatus, SceneManager.GetActiveScene().name, score);
    }

    /// <summary>
    /// To add a progression event call
    /// </summary>
    /// <param name="progressionStatus">Status (enum) of added progression (start, complete, fail).</param>
    private void LogEventWithoutScore(GAProgressionStatus progressionStatus)
    {
        GameAnalytics.NewProgressionEvent(progressionStatus, SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Called when level starts
    /// </summary>
    public void LogStartLevelEvent()
    {
        LogEventWithoutScore(GAProgressionStatus.Start);
    }


    /// <summary>
    /// Called when level fails (game over)
    /// </summary>
    public void LogFailLevelEvent()
    {
        LogEventWithoutScore(GAProgressionStatus.Fail);
    }


    /// <summary>
    /// Called when level completes (victory)
    /// </summary>
    public void LogCompleteLevelEvent()
    {
        LogEventWithoutScore(GAProgressionStatus.Complete);
    }
    #endregion

    #region LOG ERRORS
    /// <summary>
    /// To add a custom error event call
    /// </summary>
    /// <param name="severity">Severity of error (critical, debug, error, info, warning).</param>
    /// <param name="message">Error message (Optional, can be null).</param>
    private void LogErrorEvent(GAErrorSeverity severity, string message)
    {
        GameAnalytics.NewErrorEvent(severity, message);
    }

    public void LogCriticalErrorEvent(string message)
    {
        LogErrorEvent(GAErrorSeverity.Critical, message);
    }

    public void LogOrdinaryErrorEvent(string message)
    {
        LogErrorEvent(GAErrorSeverity.Error, message);
    }

    public void LogWarningErrorEvent(string message)
    {
        LogErrorEvent(GAErrorSeverity.Warning, message);
    }

    public void LogDebugErrorEvent(string message)
    {
        LogErrorEvent(GAErrorSeverity.Debug, message);
    }

    public void LogInfoErrorEvent(string message)
    {
        LogErrorEvent(GAErrorSeverity.Info, message);
    }
    #endregion

    #region DESIGN EVENTS

    /// <summary>
    /// The event string can have 1 to 5 parts. 
    /// The parts are separated by ‘:’ with a 
    /// max length of 64 each. e.g. “world1:kill:robot:laser”. 
    /// The parts can be written only with a-zA-Z0-9 characters.
    /// </summary>
    /// <param name="eventName">Name (string) of the event</param>
    /// <param name="eventValue">Number (float) value of the event</param>
    public void LogDesignEvent(string eventName, float eventValue = 0)
    {
        GameAnalytics.NewDesignEvent(eventName, eventValue);
    }
    #endregion
}
