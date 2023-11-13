using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    private bool _needsRestart = false;
    private Rigidbody2D _rbody;
    private TurnOnOffDevice _switch;
    [SerializeField] private float _velocityY = 3.0f;
    private void Start()
    {
        _rbody = GetComponent<Transform>().parent.GetComponent<Rigidbody2D>();
        _switch = GetComponent<Transform>().root.GetComponent<TurnOnOffDevice>();
    }
    private void Update()
    {
        if (_switch._isOn)
            _rbody.velocity = new Vector2(0.0f, _velocityY);
        else
            _rbody.velocity = new Vector2(0.0f, 0.0f);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        IdentificationTag colliderTag = collision.gameObject.GetComponent<IdentificationTag>();
        if (colliderTag != null && colliderTag.HasTag("Rails"))
            _needsRestart = true;
    }

    public bool NeedsRestart()
    {
        return _needsRestart; 
    }
    public void DisableRestart()
    {
        _needsRestart = false;
    }
    public void InvertVelocity()
    {
        _velocityY *= -1.0f;
    }
}
