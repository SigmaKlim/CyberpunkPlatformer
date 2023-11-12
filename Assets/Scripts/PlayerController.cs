using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EMovementMode
{
    Idle = 0,
    Running = 1,
    StartingJump = 2,
    FlyingUp = 3,
    FlyingDown = 4,
    Climbing = 5
}

public class PlayerController : MonoBehaviour
{
    private Vector2             _startingPosition;
    private float               _startingGravityScale;
    private Transform           _transform;
    private Rigidbody2D         _rbody;
    private SpriteRenderer      _sprite;
    private Animator            _animator;
    private CapsuleCollider2D   _collider;
    private EMovementMode       _eMovementMode;
    private const float         JUMP_VELOCITY = 13.0f;
    private const float         RUN_VELOCITY = 7.0f;
    private const float         CLIMB_VELOCITY = 3.0f;
    private const int           MAX_JUMPS_IN_ROW = 1;
    private int                 _jumpCount;
    private bool                _isClimbingEnabled;
    private float               _climbX;
    private LayerMask           _startingLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        _startingGravityScale = GetComponent<Rigidbody2D>().gravityScale;
        _startingPosition = GetComponent<Transform>().position;
        _eMovementMode = EMovementMode.Idle;
        _transform = GetComponent<Transform>();
        _rbody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = _sprite.GetComponent<Animator>();
        _jumpCount = 0;
        _isClimbingEnabled = false;
        _startingLayerMask = _collider.includeLayers;
    }

    // Update is called once per frame
    void Update()
    {
        LayerMask terrainLayerMask = 1 << LayerMask.NameToLayer("Terrain");
        float dirY = Input.GetAxisRaw("Vertical");
        float dirX = Input.GetAxisRaw("Horizontal");
        bool isJumpActivated = Input.GetButtonDown("Jump");

        if (isJumpActivated == true && _jumpCount < MAX_JUMPS_IN_ROW)
            _eMovementMode = EMovementMode.StartingJump;
        else if (dirY != 0 && _isClimbingEnabled == true)
            _eMovementMode = EMovementMode.Climbing;
        else if (_rbody.velocity.y > 0.01)
            _eMovementMode = EMovementMode.FlyingUp;
        else if (_rbody.velocity.y < -0.01)
            _eMovementMode = EMovementMode.FlyingDown;
        else if (dirX != 0)
            _eMovementMode = EMovementMode.Running;
        else
            _eMovementMode = EMovementMode.Idle;

        _animator.SetInteger("movementMode", (int)_eMovementMode);
        switch (_eMovementMode)
        {
            case EMovementMode.Idle:
                _rbody.velocity = Vector3.zero;
                _jumpCount = 0;
                break;
            case EMovementMode.Running:
                _rbody.velocity = new Vector2(RUN_VELOCITY * dirX, 0.0f);
                _sprite.flipX = (dirX < 0);
                _jumpCount = 0;
                break;
            case EMovementMode.Climbing:
                _transform.position = new Vector2(_climbX, _transform.position.y);
                _rbody.velocity = new Vector2(0.0f, CLIMB_VELOCITY * dirY);
                _collider.excludeLayers = terrainLayerMask;
                _sprite.flipX = false;
                _jumpCount = 0;
                break;
            case EMovementMode.FlyingUp:
                _rbody.velocity = new Vector2(RUN_VELOCITY * dirX, _rbody.velocity.y);
                if (dirX != 0)
                    _sprite.flipX = (dirX < 0);
                break;
            case EMovementMode.FlyingDown:
                _rbody.velocity = new Vector2(RUN_VELOCITY * dirX, _rbody.velocity.y);
                if (dirX != 0)
                    _sprite.flipX = (dirX < 0);
                break;
            case EMovementMode.StartingJump:
                _rbody.velocity = new Vector2(_rbody.velocity.x, JUMP_VELOCITY);
                _jumpCount++;
            break;
        }
        if (_eMovementMode != EMovementMode.Climbing)
            _collider.excludeLayers = 0;
    }

    public void ResetPosition()
    {
        GetComponent<Transform>().position = _startingPosition;
    }

    public void EnableClimbing()
    {
        _isClimbingEnabled = true;
    }

    public void DisableClimbing()
    {
        _isClimbingEnabled = false;
    }

    public void SetClimbX(float climbX)
    {
        _climbX = climbX;
    }
}
