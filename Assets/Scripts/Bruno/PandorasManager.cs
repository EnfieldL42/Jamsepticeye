using UnityEngine;

public class PandorasManager : MonoBehaviour
{
    private Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();
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
}
