using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header("Main Data")]
    public static MenuManager Instance {  get; private set; }
    [SerializeField] private float CameraRotationStrength = 2.5f;
    [SerializeField] private float CameraSmoothSpeed = 2.5f;

    [SerializeField] private Camera MenuCamera;
    [SerializeField] private bool IsTitleMoving = false;

    private Quaternion StartingCameraRotation;
    private Animator Animator;
    private bool ButtonsEnabled = true;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private List<TextMeshProUGUI> MenuButtonTexts;

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

        Animator = GetComponent<Animator>();

        if (IsTitleMoving)
        {
            Animator.Play("Title");
            string InitialText = TitleText.text;
            TitleText.SetText($"<pend>{InitialText}</pend>");
        }
    }

    private void Update()
    {
        float MouseX = Mathf.Clamp((Mouse.current.position.x.value / Screen.width - 0.5f) * 2f, -1, 1);
        float MouseY = Mathf.Clamp((Mouse.current.position.y.value / Screen.height - 0.5f) * 2f, -1, 1);

        Quaternion TargetRotation = StartingCameraRotation * Quaternion.Euler(MouseY * CameraRotationStrength, -MouseX * CameraRotationStrength, 0f);
        MenuCamera.transform.rotation = Quaternion.Slerp(MenuCamera.transform.rotation, TargetRotation, Time.deltaTime * CameraSmoothSpeed);
    }

    public void StartGame()
    {
        if (!ButtonsEnabled) return;
        ButtonsEnabled = false;

    }

    public void OpenSettings()
    {
        if (!ButtonsEnabled) return;
        ButtonsEnabled = false;

        Animator.SetTrigger("ToggleSettings");
        print("Opening settings");
    }

    public void SettingsOpened()
    {

    }

    public void SettingsClosed()
    {

    }

    public void Close()
    {
        if (!ButtonsEnabled) return;
        ButtonsEnabled = false;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
    #endif

        Application.Quit();
    }
}
