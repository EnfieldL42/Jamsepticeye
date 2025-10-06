using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuUnderlineButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("Mouse entered");
        Underline.gameObject.SetActive(true);
        TargetUnderlineScale = ButtonText.GetRenderedValues(false) / Underline.rect.width;

        Hovering = true;
        ButtonText.SetText($"<pend><b>{InitialText}</b></pend>");
    }

    public void OnPointerExit(PointerEventData eventData)
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

            if (TargetUnderlineScale == Vector2.zero && Vector3.Distance(CurrentScale, TargetUnderlineScale) == 1f)
            {
                Hovering = false;
                Underline.gameObject.SetActive(false);
            }
        }
    }
}
