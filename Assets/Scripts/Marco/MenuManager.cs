using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance {  get; private set; }
    [SerializeField] private float CameraRotationStrength = 5f;

    [SerializeField] private float CameraSmoothSpeed = 5f;

    [SerializeField] private Camera MenuCamera;
    private Quaternion StartingCameraRotation;

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

        StartingCameraRotation = MenuCamera.transform.rotation;
    }

    private void Update()
    {
        float MouseX = Mathf.Clamp((Mouse.current.position.x.value / Screen.width - 0.5f) * 2f, -1, 1);
        float MouseY = Mathf.Clamp((Mouse.current.position.y.value / Screen.height - 0.5f) * 2f, -1, 1);

        Quaternion TargetRotation = StartingCameraRotation * Quaternion.Euler(MouseY * CameraRotationStrength, -MouseX * CameraRotationStrength, 0f);
        MenuCamera.transform.rotation = Quaternion.Slerp(MenuCamera.transform.rotation, TargetRotation, Time.deltaTime * CameraSmoothSpeed);
    }
}
