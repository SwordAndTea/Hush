using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCharacterControl : MonoBehaviour
{
    [Tooltip("The horizontal walk movement speed."), Range(0f, 1000f)]
    public float walSpeed = 100f;

    [Tooltip("The run walk movement speed."), Range(0f, 1000f)]
    public float runSpeed = 150f;

    private Animator _animator;
    private Rigidbody2D _rb;
    private SpriteRenderer _sprintRenderer;

    private InputAction _moveAction;
    private Vector2 _moveInputValue;
    private InputAction _sprintAction;
    private AudioSource _runAudio;

    private bool _isRunning;

    

    #region Hurt and Dead Variables

    private bool _canHurt = true;

    public int health = 100;

    #endregion


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _sprintRenderer = GetComponent<SpriteRenderer>();
        _runAudio = GetComponent<AudioSource>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    // Update is called once per frame
    private void Update()
    {
        _moveInputValue = _moveAction.ReadValue<Vector2>();
        if (_moveInputValue.magnitude > 0)
        {
            _animator.SetBool("IsMoving", true);
            JudgeRunning();
        }
        else
        {
            _animator.SetBool("IsMoving", false);
            _animator.SetBool("IsRunning", false);
            _isRunning = false;
        }

        UpdateRunAudio();

        if (_moveInputValue.x > 0)
        {
            _sprintRenderer.flipX = false;
        }
        else if (_moveInputValue.x < 0)
        {
            _sprintRenderer.flipX = true;
        }
    }

    // only when is moving, we check is running
    private void JudgeRunning()
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

    private void UpdateRunAudio()
    {
        if (_runAudio == null)
            return;

        bool shouldPlay = _isRunning && _moveInputValue.sqrMagnitude > 0f;
        if (shouldPlay)
        {
            if (!_runAudio.isPlaying)
                _runAudio.Play();
        }
        else
            _runAudio.Stop();
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime);
    }

    private void Move(float deltaTime)
    {
        
        var moveSpeed = walSpeed;
        if (_isRunning)
        {
            moveSpeed = runSpeed;
        }
        
        var targetVelocity = _moveInputValue * (moveSpeed * deltaTime);
        _rb.linearVelocity = targetVelocity;
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