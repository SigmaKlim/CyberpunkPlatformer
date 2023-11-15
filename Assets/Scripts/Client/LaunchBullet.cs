using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBullet : MonoBehaviour
{
    public GameObject _bullet;
    private Animator _bulletAnimator;
    void Start()
    {
        _bulletAnimator = _bullet.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
