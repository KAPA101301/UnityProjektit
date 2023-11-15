using System;
using UnityEngine;

public class RotateCoin : MonoBehaviour
{
    public GameObject goldCoinPrefab;
    public float hoverHeight = 0.2f;  // The height the coin hovers above its original position.
    public float hoverSpeed = 1.0f;  // The speed at which the coin hovers.
    public float rotateSpeed = 50.0f;  // The speed at which the coin rotates.

    private Vector3 initialPosition;
    public static event Action OnCoinCollected;

    private void OnDisable()
    {
        OnCoinCollected?.Invoke();
    }
    private void Start()
    {
        // Store the initial position of the coin.
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Calculate the new Y position for hovering.
        float newY = initialPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        // Update the coin's position to create the hovering effect.
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotate the coin around its local up axis.
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(goldCoinPrefab);
    }
}
