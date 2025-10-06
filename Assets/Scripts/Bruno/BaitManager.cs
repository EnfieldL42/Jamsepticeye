using UnityEngine;

public class BaitManager : MonoBehaviour
{
    [Header("Settings")]
    public float minTime = 2f;       // minimum time until fish bites
    public float maxTime = 5f;       // maximum time until fish bites

    int WaterLayer;
    private Rigidbody BaitRigidbody;
    public bool FishBiting = false;

    [Header("References")]
    public GameObject biteUI;        // UI element to show when fish bites
    [SerializeField] RodManager throwInput; // set to true when bait hits water

    [SerializeField] float biteTimer;
    [SerializeField] bool biteReady = false;
    public bool timerStarted = false; // track if timer has been initialized

    [Header("Soul Spawn Position")]
    public Transform soulSpawnPoint;

    [Header("Testing")]
    public bool test = false;

    private void OnEnable()
    {
        if (biteUI != null)
            biteUI.SetActive(false);  // make sure UI is off at start

        WaterLayer = LayerMask.NameToLayer("Water");
        BaitRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (test)
        {
            test = false;
            InstantiateSoulAtBait();
        }


        if (RodManager.Instance.isThrowing && transform.position.y < GameManager.Instance.Water.position.y - 5f)
        {
            RodManager.Instance.FishingRodAnimator.Play("Rod Return");
            RodManager.Instance.isThrowing = false;
            return;
        }

        if (!throwInput.onWater)
        {
            timerStarted = false; // reset if not on water
            return;
        }

        // Start timer once when bait hits water
        if (!timerStarted)
        {
            StartBiteTimer();
        }

        // Countdown
        biteTimer -= Time.deltaTime;

        if (biteTimer <= 0f && !FishBiting)
        {
            //biteReady = true;
            if (biteUI != null)
                biteUI.SetActive(true);


            Debug.Log("Fish is biting!");

            RodManager.Instance.FishingRodAnimator.Play("Fish Caught");
            FishBiting = true;
        }
    }

    /// Call this when the bait lands on water
    public void StartBiteTimer()
    {
        timerStarted = true;
        biteReady = false;
        biteTimer = Random.Range(minTime, maxTime);
    }

    /// Optional: Call this if fish is caught or bait is pulled back
    public void ResetBite()
    {
        FishBiting = false;
        biteReady = false;
        timerStarted = false;
        if (biteUI != null)
            biteUI.SetActive(false);
    }

    public void InstantiateSoulAtBait()
    {
        if (GameManager.Instance != null)
        {
            GameObject Soul = GameManager.Instance.SpawnNextSoulPrefab(soulSpawnPoint);
            RodManager.Instance.CurrentSoulInteract = Soul.GetComponent<SoulInteract>();
        }
    }

    private void OnTriggerEnter(Collider Collider)
    {
        if (Collider.gameObject.layer == WaterLayer && RodManager.Instance.isThrowing)
        {
            RodManager.Instance.OnBaitTouchedWater();
        }
    }
}
