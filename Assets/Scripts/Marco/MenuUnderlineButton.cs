using TMPro;
using UnityEngine;
using VHierarchy.Libs;

public class MenuUnderlineButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ButtonText;
    [SerializeField] private RectTransform Underline;

    private Vector2 TargetUnderlineScale;
    private bool Hovering = false;
    string InitialText;

    private void Awake()
    {
        InitialText = ButtonText.text;
        Underline.localScale = new Vector3(0f, Underline.localScale.y);
    }

    public void OnHoverEnter()
    {
        Underline.gameObject.SetActive(true);
        TargetUnderlineScale = ButtonText.GetRenderedValues(false) / Underline.rect.width;

        Hovering = true;
        ButtonText.SetText($"<pend><b>{InitialText}</b></pend>");
    }

    public void OnHoverExit()
    {
        TargetUnderlineScale = Vector2.zero;
        ButtonText.SetText($"</b>{InitialText}");
    }

    private void Update()
    {
        if (Hovering)
        {
            Vector3 CurrentScale = Underline.localScale;
            CurrentScale.x = Mathf.Lerp(CurrentScale.x, TargetUnderlineScale.x, 20f * Time.deltaTime);
            Underline.localScale = CurrentScale;

            if (TargetUnderlineScale == Vector2.zero && CurrentScale.DistanceTo(TargetUnderlineScale) == 1f)
            {
                Hovering = false;
                Underline.gameObject.SetActive(false);
            }
        }
    }
}
