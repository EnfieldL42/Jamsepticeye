using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Stats UI")]
    public TextMeshProUGUI FPSText;

    [Header("Interact UI")]
    public GameObject InteractionHolder;
    public TextMeshProUGUI InteractionText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowInteractionPrompt(string Text = default)
    {
        InteractionText.SetText(Text);
        InteractionHolder.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        InteractionHolder.SetActive(false);
    }
}
