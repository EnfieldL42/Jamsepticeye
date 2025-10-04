using UnityEngine;

public class FishingRodWorldObjectManager : MonoBehaviour
{
    public static FishingRodWorldObjectManager Instance;

    private Collider col;
    [SerializeField] GameObject fishingRodInHand;


    [Header("Tests")]
    public bool enableCollider = false;
    public bool disableObject = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        col = GetComponent<Collider>();

    }

    private void Update()
    {
        if (enableCollider)
        {
            enableCollider = false;
            EnableCollider();
        }

        if (disableObject)
        {
            disableObject = false;
            PickUpFishingRod();
        }
    }

    public void EnableCollider()
    {
        col.enabled = true;
    }

    public void PickUpFishingRod()
    {
        fishingRodInHand.SetActive(true);
        gameObject.SetActive(false);
    }

}
