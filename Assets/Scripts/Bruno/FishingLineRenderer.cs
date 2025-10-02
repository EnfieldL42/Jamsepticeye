using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLineRenderer : MonoBehaviour
{
    public Transform rodTip;
    public Transform bait;
    public int lineSegments = 20;        // more = smoother line
    public float sagAmount = 0.5f;       // maximum sag at the middle

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = lineSegments;
    }

    private void LateUpdate()
    {
        if (rodTip == null || bait == null) return;

        for (int i = 0; i < lineSegments; i++)
        {
            float t = i / (float)(lineSegments - 1); // 0 -> 1
            // Linear interpolation between rodTip and bait
            Vector3 pos = Vector3.Lerp(rodTip.position, bait.position, t);

            // Add sag: maximum at middle
            float sag = Mathf.Sin(t * Mathf.PI) * sagAmount;
            pos.y -= sag; // subtract to sag downward

            line.SetPosition(i, pos);
        }
    }
}
