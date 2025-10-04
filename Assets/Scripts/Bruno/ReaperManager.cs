using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperManager : MonoBehaviour
{
    public static ReaperManager Instance;

    [SerializeField] DialogueInteract dialogueInteract;
    Collider col;

    [SerializeField] Material material;
    [SerializeField] List<Light> lights = new List<Light>();
    [SerializeField, ColorUsage(true, true)] private Color lanternNormalColor;
    [SerializeField, ColorUsage(true, true)] private Color lanternCallingColor;

    [SerializeField] List<DialogueObject> startingDialoguesByID = new List<DialogueObject>();
    [SerializeField] List<DialogueObject> endingDialogues = new List<DialogueObject>();
    private int dialogueID = 0;

    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> callingBells = new List<AudioClip>();

    [SerializeField] bool reaperIsCalling = false;
    private Coroutine bellRoutine;

    [Header("Debug")]
    [SerializeField] bool goNextDialogue = false;
    [SerializeField] bool goEndingDialogue = false;
    [SerializeField] bool callReaper = false;

    private void Start()
    {
        ChangeLanternColor(lanternNormalColor);

        col = GetComponent<Collider>();
        dialogueInteract = GetComponent<DialogueInteract>();
        audioSource = GetComponent<AudioSource>();

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
        if (goNextDialogue)
        {
            goNextDialogue = false;
            GoNextDialogue();
        }
        if (goEndingDialogue)
        {
            goEndingDialogue = false;
            ChooseEnding(1);
        }
        if (callReaper)
        {
            callReaper = false;
            ReaperCallEnvent();
        }

    }

    public void GoNextDialogue()
    {
        dialogueID++;
        dialogueInteract.StartingDialogue = startingDialoguesByID[dialogueID];
    }


    public void ChooseEnding(int endingID)
    {
        if (endingID == 1)
        {
            dialogueInteract.StartingDialogue = endingDialogues[0];
            //play ending sequence
        }
        else if (endingID == 2)
        {
            dialogueInteract.StartingDialogue = endingDialogues[1];
            //play ending sequence
        }
    }

    public void HideCollider()
    {
        col.enabled = false;
    }

    public void ReaperCallEnvent()
    {
        reaperIsCalling = true;
        StartCalling();
        ChangeLanternColor(lanternCallingColor);
        col.enabled = true;
    }

    public void StartCalling()
    {
        if (bellRoutine == null) // prevent duplicates
            bellRoutine = StartCoroutine(PlayCallingBell());
    }

    public void StopCalling()
    {
        if (bellRoutine != null)
        {
            StopCoroutine(bellRoutine);
            bellRoutine = null;
            ChangeLanternColor(lanternNormalColor);
        }
    }

    private IEnumerator PlayCallingBell()
    {
        while (reaperIsCalling)
        {
            AudioClip clip = callingBells[Random.Range(0, callingBells.Count)];
            audioSource.PlayOneShot(clip);

            // Wait for the clip length before looping
            yield return new WaitForSeconds(clip.length);
        }

        bellRoutine = null;
    }


    private void ChangeLanternColor(Color emissionColor)
    {
        // Change emission color
        material.SetColor("_EmissionColor", emissionColor);

        // Make sure emission is actually enabled in the shader
        material.EnableKeyword("_EMISSION");

        foreach (Light light in lights)
        {
            light.color = emissionColor;
        }
    }

}
