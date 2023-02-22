using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinSound;
    
    bool wasCollected = false;

    void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "Player" && !wasCollected) {
            wasCollected = true;
            AudioSource.PlayClipAtPoint(coinSound, Camera.main.transform.position);
            FindObjectOfType<GameSession>().AddToScore(1);
            Destroy(gameObject);
        }
    }
}
