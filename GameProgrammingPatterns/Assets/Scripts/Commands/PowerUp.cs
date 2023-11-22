using System;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public static event Action OnPowerUpCollected;

    private void OnDisable()
    {
        OnPowerUpCollected?.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Make the player jump
            other.GetComponent<Rigidbody>().AddForce(Vector2.up * 10f);

            // Increase the player's scale by 10
            Vector3 newScale = other.transform.localScale * 3f;
            other.transform.localScale = newScale;

            // Destroy the PowerUp
            Destroy(gameObject);
        }
    }
}
