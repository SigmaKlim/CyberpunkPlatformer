using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<PlayerController>().ResetPosition();
    }
}
