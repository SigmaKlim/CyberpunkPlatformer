using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EMovementMode
{
    Idle             = 0,
    Running          = 1,
    StartingJump     = 2,
    FlyingUp         = 3,
    FlyingDown       = 4,
    Climbing         = 5,
    StartingInteract = 6,
    Interacting      = 7,
    ShootingIdle     = 8,
    ShootingRunning  = 9,
    ShootingFlyingUp = 10,
    ShootingFlyingDown = 11
}

public class PlayerController : MonoBehaviour, IPunObservable
{
    //maintained by local player
    private Vector2             _startingPosition;
    private float               _startingGravityScale;
    private Transform           _transform;
    private Rigidbody2D         _rbody;
    private SpriteRenderer      _sprite;
    private Animator            _animator;
    private CapsuleCollider2D   _collider;
    private PhotonView          _photonView;   
    private EMovementMode       _eMovementMode;
    private bool                _isFacingRight;
    private const float         JUMP_VELOCITY = 13.0f;
    private const float         RUN_VELOCITY = 7.0f;
    private const float         CLIMB_VELOCITY = 3.0f;
    private const int           MAX_JUMPS_IN_ROW = 1;
    private int                 _jumpCount;
    private bool                _canClimb;
    private float               _climbX;
    private const float         INTERACT_ANIMATION_DURATION = 0.5f;
    private float               _secondsSinceInteractAnimationStarted;
    public float                _secondsSinceShootAnimationStarted;
    private const float         SHOOTING_ANIMATION_DURATION = 1.0f;
    private bool                _canInteract;
    private LayerMask           _defaultExcludeLayerMask;
    public GameObject           RightHandFly;
    public GameObject           LeftHandFly;
    public GameObject           RightHandRun;
    public GameObject           LeftHandRun;
    public GameObject           RightHandIdle;
    public GameObject           LeftHandIdle;
    private GameObject          _activeHand;
    public GameObject           Gun;
    public GameObject           Shot;
    private SpriteRenderer      _gunSprite;

    public int IntState;
    //consumed from photon view

    float _dirY;
    float _dirX;
    bool _isJumpActivated;
    bool _isUseActivated;
    bool _isShootActivated;
    // Start is called before the first frame update
    private void Start()
    {
        _startingGravityScale = GetComponent<Rigidbody2D>().gravityScale;
        _startingPosition = GetComponent<Transform>().position;
        _eMovementMode = EMovementMode.Idle;
        _transform = GetComponent<Transform>();
        _rbody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = _sprite.GetComponent<Animator>();
        _photonView = GetComponent<PhotonView>();
        _jumpCount = 0;
        _canClimb = false;
        _secondsSinceInteractAnimationStarted = 0.0f;
        _canInteract = false;
        _defaultExcludeLayerMask = 1 << LayerMask.NameToLayer("Player");
        _activeHand = RightHandFly;
        _gunSprite = Gun.GetComponent<SpriteRenderer>();
        _secondsSinceShootAnimationStarted = 0.0f;
        if (_photonView.IsMine == true)
        {
            GetComponentInChildren<Camera>().enabled = true;
        }
        else
        {
            GetComponentInChildren<Camera>().enabled = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //_isShootActivated = false;
        if (_photonView.IsMine == true)
        {
            _dirY = Input.GetAxisRaw("Vertical");
            _dirX = Input.GetAxisRaw("Horizontal");
            _isJumpActivated = Input.GetButtonDown("Jump");
            _isUseActivated = Input.GetButtonDown("Use");
            _isShootActivated = Input.GetButtonDown("Shoot");
        
            if (_isUseActivated == true && (_eMovementMode == EMovementMode.Idle) && _canInteract)
                _eMovementMode = EMovementMode.StartingInteract;
            else if (_eMovementMode == EMovementMode.StartingInteract || _eMovementMode == EMovementMode.Interacting)
                _eMovementMode = EMovementMode.Interacting;
            else if (_isJumpActivated == true && _jumpCount < MAX_JUMPS_IN_ROW)
                _eMovementMode = EMovementMode.StartingJump;
            else if (_dirY != 0 && _canClimb == true)
                _eMovementMode = EMovementMode.Climbing;
            else if (_rbody.velocity.y > 0.01)
            {
                if (_isShootActivated == true || _eMovementMode == EMovementMode.ShootingFlyingUp)
                    _eMovementMode = EMovementMode.ShootingFlyingUp;
                else
                    _eMovementMode = EMovementMode.FlyingUp;
                if (_isShootActivated == true)
                    _secondsSinceShootAnimationStarted = 0.0f;
            }
            else if (_rbody.velocity.y < -0.01)
            {
                if (_isShootActivated == true || _eMovementMode == EMovementMode.ShootingFlyingDown)
                    _eMovementMode = EMovementMode.ShootingFlyingDown;
                else
                    _eMovementMode = EMovementMode.FlyingDown;
            }
            else if (_dirX != 0)
            {
                if (_isShootActivated == true || _eMovementMode == EMovementMode.ShootingRunning)
                    _eMovementMode = EMovementMode.ShootingRunning;
                else
                    _eMovementMode = EMovementMode.Running;
            }
            else
            {
                if (_isShootActivated == true || _eMovementMode == EMovementMode.ShootingIdle)
                    _eMovementMode = EMovementMode.ShootingIdle;
                else
                    _eMovementMode = EMovementMode.Idle;
            }
        }
        _animator.SetInteger("movementMode", (int)_eMovementMode);
        IntState = (int)_eMovementMode;
        if (_dirX != 0)
            _isFacingRight = (_dirX > 0);
        _activeHand.SetActive(false);
        LayerMask terrainLayerMask = 1 << LayerMask.NameToLayer("Terrain");
        switch (_eMovementMode)
        {
            case EMovementMode.Idle:
                _rbody.velocity = Vector3.zero;
                _jumpCount = 0;
                break;
            case EMovementMode.Running:
                _rbody.velocity = new Vector2(RUN_VELOCITY * _dirX, 0.0f);
                _sprite.flipX = !_isFacingRight;
                _jumpCount = 0;
                break;
            case EMovementMode.Climbing:
                _transform.position = new Vector2(_climbX, _transform.position.y);
                _rbody.velocity = new Vector2(0.0f, CLIMB_VELOCITY * _dirY);
                _collider.excludeLayers = terrainLayerMask; //allow climbing throught the floor
                _sprite.flipX = false;
                _jumpCount = 0;
                break;
            case EMovementMode.FlyingUp:
                _rbody.velocity = new Vector2(RUN_VELOCITY * _dirX, _rbody.velocity.y);
                if (_dirX != 0)
                    _sprite.flipX = !_isFacingRight;
                break;
            case EMovementMode.FlyingDown:
                _rbody.velocity = new Vector2(RUN_VELOCITY * _dirX, _rbody.velocity.y);
                if (_dirX != 0)
                    _sprite.flipX = !_isFacingRight;
                break;
            case EMovementMode.StartingJump:
                _rbody.velocity = new Vector2(_rbody.velocity.x, JUMP_VELOCITY);
                _jumpCount++;
                break;
            case EMovementMode.StartingInteract:
                _secondsSinceInteractAnimationStarted = 0.0f;
                break;
            case EMovementMode.Interacting:
                _secondsSinceInteractAnimationStarted += Time.deltaTime;
                if (_secondsSinceInteractAnimationStarted > INTERACT_ANIMATION_DURATION)
                {
                    _eMovementMode = EMovementMode.Idle;
                    _secondsSinceShootAnimationStarted = 0.0f;
                }
                    break;
            case EMovementMode.ShootingFlyingUp:
                _secondsSinceShootAnimationStarted += Time.deltaTime;
                if (_isFacingRight == true) _activeHand = RightHandFly;
                else _activeHand = LeftHandFly;
                _activeHand.SetActive(true);
                if (_secondsSinceShootAnimationStarted > SHOOTING_ANIMATION_DURATION)
                {
                    _eMovementMode = EMovementMode.FlyingUp;
                    _secondsSinceShootAnimationStarted = 0.0f;
                }
                    break;
            case EMovementMode.ShootingFlyingDown:;
                _secondsSinceShootAnimationStarted += Time.deltaTime;
                if (_isFacingRight == true) _activeHand = RightHandFly;
                else _activeHand = LeftHandFly;
                _activeHand.SetActive(true);
                if (_secondsSinceShootAnimationStarted > SHOOTING_ANIMATION_DURATION)
                {
                    _eMovementMode = EMovementMode.FlyingDown;
                    _secondsSinceShootAnimationStarted = 0.0f;
                }
                    break;
            case EMovementMode.ShootingIdle:
                _secondsSinceShootAnimationStarted += Time.deltaTime;
                if (_isFacingRight == true) _activeHand = RightHandIdle;
                else _activeHand = LeftHandIdle;
                _activeHand.SetActive(true);
                if (_secondsSinceShootAnimationStarted > SHOOTING_ANIMATION_DURATION)
                {
                    _eMovementMode = EMovementMode.Idle;
                    _secondsSinceShootAnimationStarted = 0.0f;
                }
                    break;
            case EMovementMode.ShootingRunning:
                _secondsSinceShootAnimationStarted += Time.deltaTime;
                if (_isFacingRight == true) _activeHand = RightHandRun;
                else _activeHand = LeftHandRun;
                _activeHand.SetActive(true);
                _rbody.velocity = new Vector2(RUN_VELOCITY * _dirX, 0.0f);
                if (_secondsSinceShootAnimationStarted > SHOOTING_ANIMATION_DURATION)
                {
                    _eMovementMode = EMovementMode.Running;
                    _secondsSinceShootAnimationStarted = 0.0f;
                }
                break;
        }
        if (_eMovementMode != EMovementMode.Climbing)
            _collider.excludeLayers = _defaultExcludeLayerMask; //disable climbing through the floor
        if (_activeHand.activeInHierarchy)
        {
            Gun.transform.localPosition = _activeHand.transform.localPosition;
            _gunSprite.flipX = !_isFacingRight;
            Gun.SetActive(true);
        }
        else Gun.SetActive(false);
        if (_isShootActivated == true)
        {
            Shot.GetComponent<GunController>().Shoot(_isFacingRight);
        }
    }

    public void ResetPosition()
    {
        GetComponent<Transform>().position = _startingPosition;
    }

    public void EnableClimbing()
    {
        _canClimb = true;
    }

    public void DisableClimbing()
    {
        _canClimb = false;
    }
    public void SetClimbX(float climbX)
    {
        _climbX = climbX;
    }
    public void EnableInteraction()
    {
        _canInteract = true;
    }
    public void DisableInteraction()
    {
        _canInteract = false;
    }
    public int GetMovementMode()
    {
        return (int)_eMovementMode;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_dirY);
            stream.SendNext(_dirX);
            stream.SendNext(_isJumpActivated);
            stream.SendNext(_isUseActivated);
            stream.SendNext(_isShootActivated);
            stream.SendNext(_eMovementMode);
        }
        else
        {
            _dirY = (float)stream.ReceiveNext();
            _dirX = (float)stream.ReceiveNext();
            _isJumpActivated = (bool)stream.ReceiveNext();
            _isUseActivated = (bool)stream.ReceiveNext();
            _isShootActivated = (bool)stream.ReceiveNext();
            _eMovementMode = (EMovementMode)stream.ReceiveNext();
        }
    }
}
