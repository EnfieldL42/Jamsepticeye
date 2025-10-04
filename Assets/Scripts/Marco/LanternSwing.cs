using System.Collections;
using UnityEngine;

public class LanternSwing : MonoBehaviour
{
    private Rigidbody lanternRb;

    [SerializeField] private bool applyingWind = true;

    [Header("Wind Settings")]
    [SerializeField] private float torqueStrength = 1f;     // max torque per push
    [SerializeField] private float pushInterval = 10f;      // how often to apply wind
    [SerializeField] private float verticalTorqueFactor = 0.2f; // reduce torque on Y for more natural swing

    private void Awake()
    {
        lanternRb = GetComponent<Rigidbody>();
        StartCoroutine(ApplyWind());
    }

    private IEnumerator ApplyWind()
    {
        while (applyingWind)
        {
            float t = Time.time;
            Vector3 torque = new Vector3(
                (Mathf.PerlinNoise(t, 0f) - 0.5f) * 2f,  // X
                (Mathf.PerlinNoise(0f, t) - 0.5f) * 0.4f, // Y dampened
                (Mathf.PerlinNoise(t, t) - 0.5f) * 2f   // Z
            ) * torqueStrength;

            lanternRb.AddTorque(torque, ForceMode.Acceleration);


            yield return new WaitForSeconds(pushInterval);
        }
    }
}
