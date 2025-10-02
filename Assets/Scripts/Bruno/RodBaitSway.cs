using UnityEngine;

public class BaitReturn : MonoBehaviour
{
    [Header("References")]
    public Transform bait;        // bait is child of rod tip

    [Header("Settings")]
    public Vector3 initialLocalPos;  // set this manually in inspector
    public float smoothTime = 0.2f;  // damping time for return

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (bait == null) return;

        // Smoothly return bait to manually set local position
        bait.localPosition = Vector3.SmoothDamp(bait.localPosition, initialLocalPos, ref velocity, smoothTime);
    }
}
