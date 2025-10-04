using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private float MasterVolume;
    private bool ChangingMasterVolume = false;

    [Header("Main UI Elements")]
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private List<TextMeshProUGUI> MenuButtonTexts;

    [Space, Header("Settings UI Elements")]
    [SerializeField] private TextMeshProUGUI MasterVolumePercentage;
    [SerializeField] private Slider MasterVolumeSlider;

    private IEnumerator TeleportToNewScene()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadSceneAsync(1);
    }

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

        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = MasterVolume;
        StartingCameraRotation = MenuCamera.transform.rotation;

        MasterVolumeSlider.value = MasterVolume;
        MasterVolumePercentage.SetText("{0}%", Mathf.Round(MasterVolumeSlider.value * 100f));
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

        print(LoadingScreenManager.Instance == null);

        LoadingScreenManager.Instance.StartLoading();
        StartCoroutine(TeleportToNewScene());
    }

    public void OpenSettings()
    {
        if (!ButtonsEnabled) return;
        ButtonsEnabled = false;

        Animator.SetBool("OpenSettings", true);
    }

    public void SettingsToggled()
    {
        ButtonsEnabled = true;
    }

    public void ChangeMasterVolume()
    {
        MasterVolumePercentage.SetText("{0}%", Mathf.Round(MasterVolumeSlider.value * 100f));
        AudioListener.volume = MasterVolumeSlider.value;

        if (!ChangingMasterVolume)
        {
            ChangingMasterVolume = true;
        }
    }

    public void SubmitMasterVolume()
    {
        if (!ChangingMasterVolume) return;
        ChangingMasterVolume = false;

        AudioListener.volume = MasterVolumeSlider.value;
        PlayerPrefs.SetFloat("MasterVolume", MasterVolumeSlider.value);
        PlayerPrefs.Save();
    }

    public void CloseSettings()
    {
        if (!ButtonsEnabled) return;
        ButtonsEnabled = false;

        Animator.SetBool("OpenSettings", false);
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
