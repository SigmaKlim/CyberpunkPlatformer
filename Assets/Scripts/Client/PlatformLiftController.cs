using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLiftController : MonoBehaviour
{
    [SerializeField] private GameObject _platform;
    private PlatformController _platformController;
    // Start is called before the first frame update
    void Start()
    {
        _platformController = GetComponentInChildren<PlatformController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_platformController.NeedsRestart())
        {
            _platformController.DisableRestart();
            _platformController.InvertVelocity();
        }
    }
}
