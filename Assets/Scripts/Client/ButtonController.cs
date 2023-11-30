using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public bool _isPressed = false;
    public void Pressed()
    {
        _isPressed = true;  
    }
    public void Released()
    {
        _isPressed = false;
    }
}
