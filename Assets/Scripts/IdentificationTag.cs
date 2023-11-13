using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentificationTag : MonoBehaviour
{
    [SerializeField] public string[] _tags;

    public bool HasTag(string theTag)
    {
        foreach(string tag in _tags)
        {
            if (tag == theTag) return true;
        }
        return false;
    }
}
