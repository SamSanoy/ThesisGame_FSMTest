using UnityEngine;

public class PlayerHitCounter : MonoBehaviour
{
    [Header("Player Hit Settings")]
    [SerializeField] private int activationThreshold = 4;

    private int playerHitCount = 0;
    private bool countingEnabled = true;

    public int PlayerHitCount => playerHitCount;

    public bool IsReady =>
        playerHitCount >= activationThreshold;

    public void RegisterPlayerHit()
    {
        if (!countingEnabled || IsReady)
            return;

        playerHitCount++;

        /*Debug.Log(
            "Player Hit Count: " +
            playerHitCount + "/" +
            activationThreshold
        );*/

        if (IsReady)
        {
            //Debug.Log("AOE Attack Ready!");
        }
    }

    public void OnAOEExecuted()
    {
        playerHitCount = 0;
        countingEnabled = false;

        //Debug.Log("Player-Hit Counter paused.");
    }

    public void OnAOERecoveryComplete()
    {
        countingEnabled = true;

        //Debug.Log("Player-Hit Counter active again.");
    }
}