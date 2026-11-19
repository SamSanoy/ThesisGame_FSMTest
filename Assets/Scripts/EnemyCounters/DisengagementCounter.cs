using UnityEngine;

public class DisengagementCounter : MonoBehaviour
{
    [Header("Disengagement Settings")]
    [SerializeField] private float engagementRange = 1.25f;
    [SerializeField] private int activationThreshold = 2;

    [Header("Detection Settings")]
    [SerializeField] private float exitBuffer = 0.2f;
    [Tooltip("How long the player must stay beyond engagementRange + exitBuffer before an exit counts. Set to 0 during testing to count immediately on crossing the buffered boundary.")]
    [SerializeField, Min(0f)] private float disengagementDelay = 0.3f;

    [Header("Debug (enable while testing)")]
    [SerializeField] private bool logCounterEvents = false;

    [Header("References")]
    [SerializeField] private Transform player;

    private int disengagementCount = 0;
    private bool isEngaged = false;
    private float outsideTimer = 0f;

    public int DisengagementCount => disengagementCount;
    public int ActivationThreshold => activationThreshold;
    public float EngagementRange => engagementRange;
    public float ExitThreshold => engagementRange + exitBuffer;
    public bool IsEngaged => isEngaged;

    public bool IsReady =>
        disengagementCount >= activationThreshold;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("DisengagementCounter: Player reference missing; counter disabled.", this);
            enabled = false;
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        isEngaged = distance <= engagementRange;
        Log("Start: " + (isEngaged ? "engaged" : "waiting to enter close range") +
            " (distance " + distance.ToString("F2") + ", engagementRange " + engagementRange.ToString("F2") +
            ", exit boundary " + ExitThreshold.ToString("F2") +
            ", threshold " + activationThreshold + ").");
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
                Log("Re-engaged at distance " + distance.ToString("F2") +
                    ". Another disengagement can now be counted.");
            }

            return;
        }

        if (distance <= ExitThreshold)
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
        {
            Log("Exit detected while Long-Range is already ready; count remains " +
                disengagementCount + "/" + activationThreshold + ".");
            return;
        }

        disengagementCount++;
        Log("Disengagement counted: " + disengagementCount + "/" + activationThreshold +
            " (distance " + distance.ToString("F2") + ")." +
            (IsReady ? " LONG-RANGE READY; requires player outside close range to fire." : ""));
    }

    public void OnLongRangeExecuted()
    {
        disengagementCount = 0;
        outsideTimer = 0f;

        Log("Long-Range fired: counter reset to 0/" + activationThreshold +
            ". Re-enter within " + engagementRange.ToString("F2") +
            " before another disengagement can count.");
    }

    private void Log(string message)
    {
        if (logCounterEvents)
            Debug.Log("[DisengagementCounter] " + message, this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            engagementRange
        );

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, ExitThreshold);
    }
}