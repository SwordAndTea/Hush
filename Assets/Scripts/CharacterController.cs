using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    [Tooltip("The horizontal walk movement speed."), Range(0f, 1000f)]
    public float walSpeed = 100f;
    
    [Tooltip("The run walk movement speed."), Range(0f, 1000f)]
    public float runSpeed = 150f;
    
    [Tooltip("The up force applied when jump"), Range(150f, 500f)]
    public float jumpPower = 250f;
    
    private Animator _animator;
    private Rigidbody2D _rb;
    private SpriteRenderer _sprintRenderer;
    
    private InputAction _moveAction;
    private InputAction _sprintAction;
    private InputAction _jumpAction;
    
    private bool _isMovingRight;
    private bool _isMovingLeft;
    private bool _isRunning;
    
    private bool _isGrounded;

    #region Hurt and Dead Variables

    private bool _canHurt = true;

    public int health = 100;
        

    #endregion
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _sprintRenderer = GetComponent<SpriteRenderer>();
        
        _moveAction = InputSystem.actions.FindAction("Move");
        _sprintAction = InputSystem.actions.FindAction("Sprint");
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    private void Update()
    {
        var moveInput = _moveAction.ReadValue<Vector2>();
        switch (moveInput.x)
        {
            case > 0:
                _isMovingRight = true;
                _isMovingLeft = false;
                _sprintRenderer.flipX = false;
                _animator.SetBool("IsMoving", true);
                JudgeRunning();
                break;
            case < 0:
                _isMovingLeft = true;
                _isMovingRight = false;
                _sprintRenderer.flipX = true;
                _animator.SetBool("IsMoving", true);
                JudgeRunning();
                break;
            default: // equals 0
                _isMovingRight = false;
                _isMovingLeft = false;
                _animator.SetBool("IsMoving", false);
                _animator.SetBool("IsRunning", false);
                break;
        }
        _animator.SetFloat("YVelocity", _rb.linearVelocity.y);
    }

    // only when is moving, we check is running
    private void JudgeRunning()
    {
        if (_isGrounded) // update running status only when is grounded
        {
            if (_sprintAction.IsPressed())
            {
                _animator.SetBool("IsRunning", true);
                _isRunning = true;
            }
            else
            {
                _animator.SetBool("IsRunning", false);
                _isRunning = false;
            }
        }
    }

    // the trigger is bound for GroundChecker child GameObject
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            _animator.SetBool("IsGrounded", true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
            _animator.SetBool("IsGrounded", false);
        }
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    private void Move(float deltaTime)
    {
        #region Move And Run

        var targetVelocity = new Vector2(0, _rb.linearVelocityY);
        var moveSpeed = walSpeed;
        if (_isRunning)
        {
            moveSpeed = runSpeed;
        }

        if (_isMovingRight)
        {
            targetVelocity.x = moveSpeed *  deltaTime;
        }
        else if (_isMovingLeft)
        {
            targetVelocity.x = -1 * moveSpeed * deltaTime;
        }

        _rb.linearVelocity = targetVelocity;

        #endregion
        
        
        #region Jump And Fall
        if (_isGrounded && _jumpAction.IsPressed())
        {
            _rb.AddForce(new Vector2(0f, jumpPower));
            _isGrounded = false;
        }
        
        #endregion
    }

    
    // the animation event of hurt animation
    private void OnHurtAnimationDone()
    {
        _canHurt = true;
    }

    // the animation event of dead animation
    private void OnDeadAnimationDone()
    {
        
    }
}