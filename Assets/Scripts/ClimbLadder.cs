using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClimbLadder : MonoBehaviour
{
    float _climbX = 0.0f;
    private void Start()
    {
    
        Transform[] ts = GetComponentsInChildren<Transform>();
        foreach (Transform t in ts)
            if (t.gameObject.name == "ClimbHelper")
                _climbX = t.position.x;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().EnableClimbing();
            collision.gameObject.GetComponent<PlayerController>().SetClimbX(_climbX);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().DisableClimbing();
        }
    }
}
