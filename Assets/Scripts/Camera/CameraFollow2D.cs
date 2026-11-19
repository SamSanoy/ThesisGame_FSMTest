using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;

    [Header("Camera Settings")]
    [SerializeField] float smoothSpeed = 5f;
    [SerializeField] Vector3 offset = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition =
            target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
