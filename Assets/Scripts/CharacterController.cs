using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    [Tooltip("The horizontal walk movement speed."), Range(0.5f, 10f)]
    public float walSpeed = 2f;
    
    [Tooltip("The run walk movement speed."), Range(5f, 20f)]
    public float runSpeed = 5f;
    
    private Animator _animator;
    private Rigidbody2D _rb;
    private SpriteRenderer _sprintRenderer;
    private InputAction _moveAction;
    private InputAction _sprintAction;
    private bool _isMovingRight;
    private bool _isMovingLeft;
    private bool _isRunning = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _sprintRenderer = GetComponent<SpriteRenderer>();
        _moveAction = InputSystem.actions.FindAction("Move");
        _sprintAction = InputSystem.actions.FindAction("Sprint");
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
    }

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

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        var targetVelocity = new Vector2(0, _rb.linearVelocityY);
        var moveSpeed = walSpeed;
        if (_isRunning)
        {
            moveSpeed = runSpeed;
        }

        if (_isMovingRight)
        {
            targetVelocity.x = 1 * moveSpeed *  Time.deltaTime * 60;
        }
        else if (_isMovingLeft)
        {
            targetVelocity.x = -1 * moveSpeed * Time.deltaTime * 60;
        }

        _rb.linearVelocity = targetVelocity;
    }
}