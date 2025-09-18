using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;             // Player ship to follow

    [Header("Camera Settings")]
    public float followSpeed = 5f;       // How quickly camera catches up
    public Vector3 offset = new Vector3(0, 0, -10f); // Keeps camera behind

    private void LateUpdate()
    {
        if (target == null) return;

        // Desired position = target + offset
        Vector3 desiredPosition = target.position + offset;

        // Smooth transition to target
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}

