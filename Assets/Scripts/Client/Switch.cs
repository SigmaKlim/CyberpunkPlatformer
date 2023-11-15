using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private List<GameObject> _switchableObjects;
    [SerializeField] private float _resetSeconds;
    private List<TurnOnOffDevice> _devices;
    private TurnOnOffDevice _device;
    private float _secondsSinceTurnedOn;
    // Start is called before the first frame update
    void Start()
    {
        _devices = new List<TurnOnOffDevice>();
        foreach (GameObject so in _switchableObjects)
        {
            _devices.Add(so.GetComponent<TurnOnOffDevice>());
        }
        _device = GetComponent<TurnOnOffDevice>();

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_device._isOn == false)
        {
            if (collision.gameObject.CompareTag("Player"))
                if (collision.gameObject.GetComponent<PlayerController>().GetMovementMode() == 7)
                {
                    SwitchOn();
                }
        }
        else
            if (collision.gameObject.CompareTag("Player"))
                if (collision.gameObject.GetComponent<PlayerController>().GetMovementMode() == 7)
                {
                    SwitchOff();
                }
    }

    private void Update()
    {
        if (_device._isOn == true && _resetSeconds != 0.0f)
            if (_secondsSinceTurnedOn < _resetSeconds)
            {
                _secondsSinceTurnedOn += Time.deltaTime;
            }
            else
            {
                SwitchOff();
            }
    }
    private void SwitchOn()
    {
        _device._isOn = true;
        foreach (TurnOnOffDevice d in _devices)
        {
            d._isOn = true;
        }
        _secondsSinceTurnedOn = 0.0f;
    }
    private void SwitchOff()
    {
        _device._isOn = false;
        foreach (TurnOnOffDevice d in _devices)
        {
            d._isOn = false;
        }
    }
}
