using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLadder : MonoBehaviour
{
    public int Side = 1;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Mathf.Sign(collision.gameObject.GetComponent<PlayerController>().GetDirY()) == Mathf.Sign(Side))
                collision.gameObject.GetComponent<PlayerController>().DisableClimbing();
        }
    }
}
