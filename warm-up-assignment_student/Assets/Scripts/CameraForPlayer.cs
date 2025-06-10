using UnityEngine;

public class CameraForPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;           // Drag your player object here
    public Vector3 offset = new Vector3(0, 10, 0);  // Camera offset from player
    public float followSpeed = 5f;     // How fast the camera follows

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate target position
        Vector3 targetPosition = player.position + offset;

        // Smoothly move camera to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Keep camera looking down at the player
        transform.LookAt(player.position);
    }
}
