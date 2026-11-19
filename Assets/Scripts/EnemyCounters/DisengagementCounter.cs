using UnityEngine;

public class DisengagementCounter : MonoBehaviour
{
    [Header("Disengagement Settings")]
    [SerializeField] private float engagementRange = 1.25f;
    [SerializeField] private int activationThreshold = 2;

    [Header("Detection Settings")]
    [SerializeField] private float exitBuffer = 0.2f;
    [SerializeField] private float disengagementDelay = 0.3f;

    [Header("References")]
    [SerializeField] private Transform player;

    private int disengagementCount = 0;
    private bool isEngaged = false;
    private float outsideTimer = 0f;

    public int DisengagementCount => disengagementCount;

    public bool IsReady =>
        disengagementCount >= activationThreshold;

    void Start()
    {
        if (player == null)
        {
            enabled = false;
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        isEngaged = distance <= engagementRange;
    }

    void Update()
    {
        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (!isEngaged)
        {
            if (distance <= engagementRange)
            {
                isEngaged = true;
                outsideTimer = 0f;

                //Debug.Log("Player entered engagement range.");
            }

            return;
        }

        if (distance <= engagementRange + exitBuffer)
        {
            outsideTimer = 0f;
            return;
        }

        outsideTimer += Time.deltaTime;

        if (outsideTimer < disengagementDelay)
            return;

        isEngaged = false;
        outsideTimer = 0f;

        if (IsReady)
            return;

        disengagementCount++;

        /*Debug.Log(
            "Disengagement Count: " +
            disengagementCount + "/" +
            activationThreshold
        );*/

        if (IsReady)
        {
            //Debug.Log("Long-Range Slow Attack Ready!");
        }
    }

    public void OnLongRangeExecuted()
    {
        disengagementCount = 0;

        //Debug.Log("Disengagement Counter Reset.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            engagementRange
        );

        /*Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            engagementRange + exitBuffer
        );*/
    }
}