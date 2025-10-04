using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    private Animator Animator;
    [HideInInspector] public bool InLoadingScreen = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Animator = GetComponent<Animator>();
    }

    public void StartLoading()
    {
        if (InLoadingScreen) return;

        Animator.Play("LoadingStart");
        InLoadingScreen = true;
    }

    public void StopLoading()
    {
        if (!InLoadingScreen) return;

        Animator.Play("LoadingEnd");
        InLoadingScreen = false;
    }
}
