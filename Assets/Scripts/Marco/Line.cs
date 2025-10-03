using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour
{
    [Header("This will only update during Play Mode")]
    [SerializeField] private Transform Attachment0;

    [SerializeField] private Transform Attachment1;
    private LineRenderer LineRenderer;

    private void Awake()
    {
        LineRenderer = GetComponent<LineRenderer>();
        LineRenderer.positionCount = 2;
    }

    private void Update()
    {
        if (Attachment0 != null && Attachment1 != null)
        {
            LineRenderer.SetPosition(0, Attachment0.position);
            LineRenderer.SetPosition(1, Attachment1.position);
        }
    }
}
