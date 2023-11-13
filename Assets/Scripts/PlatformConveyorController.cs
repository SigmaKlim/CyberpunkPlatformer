using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpdatePlatformConveyor : MonoBehaviour
{
    private PlatformController[] _platformControllers;
    private float _newPlatformY = -9.0f;
    // Start is called before the first frame update
    void Start()
    {
        _platformControllers = GetComponentsInChildren<PlatformController>();
    }

    // Update is called once per frame
    void Update()
    {
       foreach (PlatformController pc in _platformControllers) 
        {
            if (pc.NeedsRestart() == true)
            {
                pc.gameObject.GetComponent<Transform>().parent.position = new Vector2(
                    GetComponent<Transform>().position.x,
                    _newPlatformY);
                pc.DisableRestart();
                break;
            }
        }

    }

    
}
