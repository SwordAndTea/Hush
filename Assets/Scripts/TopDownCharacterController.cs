using AIEnemy.Crawler;
using AIEnemy.Spider;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownCharacterControl : MonoBehaviour
{
    #region Inspector · public

    [Header("Movement")]
    [Tooltip("The horizontal walk movement speed.")]
    [Range(0f, 1000f)]
    public float walSpeed = 100f;

    [Tooltip("The run walk movement speed.")]
    [Range(0f, 1000f)]
    public float runSpeed = 150f;

    [Header("Spider web")]
    [Tooltip("How long movement is blocked after being hit by spider web.")]
    public float webTrapDurationSeconds = 2f;

    [Header("Combat · enemy contact")]
    [Tooltip("HP lost when the player is hit by a spider enemy body (not web projectile).")]
    [Min(1)] public int spiderContactDamage = 1;

    [Tooltip("HP lost when the player is hit by a crawler enemy body.")]
    [Min(1)] public int crawlerContactDamage = 5;

    #endregion

    #region Inspector · serialized (private)

    [Header("Combat")]
    [Range(1, 5)]
    [SerializeField] int health = 3;

    [Header("Spider web · animator")]
    [Tooltip("Full animator state path for the web-trap clip (layer path + state name). Used to restart the clip from the beginning on every hit while IsTrappedByWeb stays true.")]
    [SerializeField] string webTrapAnimatorStatePath = "Base Layer.ShakeSpiderWeb";

    [Tooltip("Animator layer index for webTrapAnimatorStatePath.")]
    [SerializeField] int webTrapAnimatorLayer = 0;

    [Header("Enemies")]
    [Tooltip("Tag on the enemy root (or a parent of the damage collider). Works with trigger hitboxes: overlap is handled in OnTriggerEnter2D. Solid (non-trigger) enemy colliders also call OnCollisionEnter2D.")]
    [SerializeField] string enemyTag = "Enemy";

    #endregion

    #region Private

    private Animator _animator;
    private Rigidbody2D _rb;
    private SpriteRenderer _sprintRenderer;

    private InputAction _moveAction;
    private Vector2 _moveInputValue;
    private InputAction _sprintAction;
    private AudioSource _runAudio;

    private bool _isRunning;

    private bool _isTrappedByWeb;
    private float _webTrapEndTime;

    private bool _canHurt = true;

    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _sprintRenderer = GetComponent<SpriteRenderer>();
        _runAudio = GetComponent<AudioSource>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _sprintAction = InputSystem.actions.FindAction("Sprint");

        if (_animator != null)
            _animator.SetInteger("Health", health);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isTrappedByWeb && Time.time >= _webTrapEndTime)
        {
            _isTrappedByWeb = false;
            if (_animator != null)
                _animator.SetBool("IsTrappedByWeb", false);
        }

        if (!_canHurt || _isTrappedByWeb)
        {
            _moveInputValue = Vector2.zero;
            if (_rb != null)
                _rb.linearVelocity = Vector2.zero;
            if (_animator != null)
            {
                _animator.SetBool("IsMoving", false);
                _animator.SetBool("IsRunning", false);
            }
            _isRunning = false;
            UpdateRunAudio();
            return;
        }

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
        if (!_canHurt || _isTrappedByWeb)
        {
            if (_rb != null)
                _rb.linearVelocity = Vector2.zero;
            return;
        }

        Move(Time.fixedDeltaTime);
    }

    // /// <summary>Enemy damage colliders that are not triggers (solid contacts).</summary>
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.collider == null || !IsGameObjectOrParentTaggedEnemy(collision.collider.gameObject))
    //         return;
    //
    //     TryHurtFromEnemy(collision.collider);
    // }

    /// <summary>Spider web (trigger), enemy hitboxes (trigger), and any other trigger overlaps.</summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
            return;

        var web = other.GetComponent<SpiderWeb>() ?? other.GetComponentInParent<SpiderWeb>();
        if (web != null)
        {
            if (!_canHurt)
            {
                Destroy(web.gameObject);
                return;
            }

            Destroy(web.gameObject);

            _isTrappedByWeb = true;
            _webTrapEndTime = Time.time + Mathf.Max(0f, webTrapDurationSeconds);
            if (_animator != null)
            {
                _animator.SetBool("IsTrappedByWeb", true);
                if (!string.IsNullOrEmpty(webTrapAnimatorStatePath))
                    _animator.Play(webTrapAnimatorStatePath, webTrapAnimatorLayer, 0f);
            }

            return;
        }

        if (IsGameObjectOrParentTaggedEnemy(other.gameObject))
            TryHurtFromEnemy(other);
    }

    void ClearWebTrap()
    {
        _isTrappedByWeb = false;
        _webTrapEndTime = 0f;
        if (_animator != null)
            _animator.SetBool("IsTrappedByWeb", false);
    }

    bool IsGameObjectOrParentTaggedEnemy(GameObject go)
    {
        if (go == null || string.IsNullOrEmpty(enemyTag))
            return false;

        Transform t = go.transform;
        while (t != null)
        {
            if (t.gameObject.CompareTag(enemyTag))
                return true;
            t = t.parent;
        }

        return false;
    }

    void TryHurtFromEnemy(Collider2D source)
    {
        if (!_canHurt)
            return;

        EnterHitStun();

        int contactDamage = GetEnemyContactDamage(source);
        if (!ApplyHealthLoss(contactDamage))
        {
            DestroySpiderEnemyIfPresent(source);
            return;
        }

        if (_animator != null)
            _animator.SetBool("IsHurt", true);

        DestroySpiderEnemyIfPresent(source);
    }

    int GetEnemyContactDamage(Collider2D source)
    {
        if (source == null)
            return spiderContactDamage;

        if (source.GetComponent<SpiderEnemy>() != null || source.GetComponentInParent<SpiderEnemy>() != null)
            return spiderContactDamage;
        if (source.GetComponent<CrawlerEnemy>() != null || source.GetComponentInParent<CrawlerEnemy>() != null)
            return crawlerContactDamage;
        return spiderContactDamage;
    }

    void DestroySpiderEnemyIfPresent(Collider2D source)
    {
        if (source == null)
            return;

        var spider = source.GetComponent<SpiderEnemy>() ?? source.GetComponentInParent<SpiderEnemy>();
        if (spider != null)
            Destroy(spider.gameObject);
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


    public void TakeDamage(int damage)
    {
        if (!_canHurt || health <= 0)
            return;

        EnterHitStun();

        if (!ApplyHealthLoss(damage))
            return;

        if (_animator != null && health > 0)
            _animator.SetBool("IsHurt", true);
    }

    /// <summary>Subtracts damage, updates animator Health, and disables control on death. Returns false if dead.</summary>
    bool ApplyHealthLoss(int damage)
    {
        health = Mathf.Max(0, health - damage);
        if (_animator != null)
            _animator.SetInteger("Health", health);

        if (health > 0)
            return true;

        if (_animator != null)
            _animator.SetBool("IsHurt", false);
        ZeroMovementAndClearWeb();
        enabled = false;
        return false;
    }

    void EnterHitStun()
    {
        _canHurt = false;
        ZeroMovementAndClearWeb();
    }

    void ZeroMovementAndClearWeb()
    {
        _moveInputValue = Vector2.zero;
        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;
        ClearWebTrap();
    }

    private void OnHurtAnimationDone()
    {
        if (_animator != null)
            _animator.SetBool("IsHurt", false);
        _canHurt = true;
    }

    // the animation event of dead animation
    private void OnDeadAnimationDone()
    {
        //TODO: show the Dead UI, not created yet
    }
}