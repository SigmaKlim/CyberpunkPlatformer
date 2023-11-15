using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour
{
    private bool _isShooting;
    private bool _isShootingRight;
    public GameObject Bullet;
    private Rigidbody2D _bulletRb;
    //private SpriteRenderer _bulletSprite;
    private SpriteRenderer _shotSprite;
    private Animator _shotAnimator;
    private const float SHOOT_ANIMATION_DURATION = 0.5f;
    private float _secondSinceShootAnimationStarted;
    private const float BULLET_VELOCITY = 9.0f;
    private Vector2 _bulletStartingPosition;
    // Start is called before the first frame update
    void Start()
    {
        //_bulletSprite = Bullet.GetComponent<SpriteRenderer>();
        _bulletRb = Bullet.GetComponent <Rigidbody2D>();
        _shotSprite = GetComponent<SpriteRenderer>();
        _shotAnimator = GetComponent<Animator>();
        _bulletStartingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isShooting == true)
        {
            Bullet.SetActive(true);
            _shotAnimator.SetBool("isShooting", true);
            _shotSprite.enabled = true;
            _secondSinceShootAnimationStarted = 0.0f;
            Bullet.transform.position = _bulletStartingPosition;
            if (_isShootingRight == true)
            {
                _bulletRb.velocity = new Vector2(BULLET_VELOCITY, 0.0f);
                _shotSprite.flipX = false;
            }
            else
            {
                _bulletRb.velocity = new Vector2(-BULLET_VELOCITY, 0.0f);
                _shotSprite.flipX = true;
            }
            _isShooting = false;
        }
        if (_secondSinceShootAnimationStarted < SHOOT_ANIMATION_DURATION)
        {
            _secondSinceShootAnimationStarted += UnityEngine.Time.deltaTime;
        }
        else
        {
            //Bullet.SetActive(false);
            _shotAnimator.SetBool("isShooting", false);
            _shotSprite.enabled = false;
        }

    }

    public void Shoot(bool shootRight)
    {
        _isShooting = true;
        _isShootingRight = shootRight;
    }
}
