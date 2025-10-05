using UnityEngine;

public class PandorasManager : MonoBehaviour
{
    public static PandorasManager Instance;

    private Collider col;

    [SerializeField] DialogueInteract dialogue;
    [SerializeField] DialogueOptionsObject dialogueWithFreeOption;
    [SerializeField] int whichSoulToFree = 0;
 
    private void Start()
    {
        col = GetComponent<Collider>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void Update()
    {
        if (GameManager.Instance.playerHasSoul)
        {
            col.enabled = true;
        }

        else
        {
            col.enabled = false;
        }
    }

    public void CheckIfGiveFreeSoulOption()
    {
        if (GameManager.Instance.soulIndex == whichSoulToFree)
        {
            dialogue.StartingDialogue = dialogueWithFreeOption;
        }
    }
}
