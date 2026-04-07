using System.Reflection;
using Pathfinding;
using UnityEngine;

namespace AIEnemy.Crawler
{
    public class CrawlerEnemy : MonoBehaviour
    {
        static readonly int IsGrabbingHash = Animator.StringToHash("IsGrabbing");
        static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

        private IAstarAI _pathfinder;
        private Animator _animator;
        private Quaternion _rotationMovingLeft;
        private Quaternion _rotationMovingRight;
        private Patrol _patrol;
        private bool _isGrabbing;
        private bool _grabComplete;
        private TopDownCharacterControl _grabbedPlayer;

        public bool ReachedDestination => _pathfinder.reachedEndOfPath;
        public bool CanPatrol => _patrol != null;
        public bool IsGrabbing => _isGrabbing;

        [Header("Movement")]
        public float runSpeed = 3f;
        public float patrolSpeed = 1f;

        [Space]
        [Header("Grab Attack")]
        [SerializeField] float grabRange = 1f;
        [SerializeField] Transform grabPoint;
        [SerializeField] LayerMask grabTargetLayers;

        [Space]
        [Header("Patrol · random generation (StartPatrol)")]
        [Tooltip("Random patrol points are picked within this radius around the crawler (XY)."), Min(0.01f)]
        [SerializeField] float patrolGenerationRadius = 4f;
        [Tooltip("Minimum distance between the two patrol points (clamped to what fits inside the radius).")]
        [SerializeField] float patrolMinTargetSeparation = 1f;
        [Tooltip("Physics2D overlap radius at each candidate point; must be clear of blocking colliders.")]
        [SerializeField] float patrolColliderCheckRadius = 0.12f;
        [SerializeField] LayerMask patrolObstacleLayers = ~0;
        [SerializeField] int patrolRandomPointAttempts = 32;
        [SerializeField] int patrolPairAttempts = 48;

        private Transform _patrolRuntimeTargetA;
        private Transform _patrolRuntimeTargetB;

        private void Awake()
        {
            _pathfinder = GetComponent<IAstarAI>();
            _animator = GetComponent<Animator>();
            _patrol = GetComponent<Patrol>();
            _rotationMovingLeft = transform.localRotation;
            _rotationMovingRight = _rotationMovingLeft * Quaternion.Euler(0f, 180f, 0f);

            _isGrabbing = false;
            if (_patrol != null)
                _patrol.enabled = false;
            if (_pathfinder != null)
                _pathfinder.isStopped = true;
            if (_animator != null)
            {
                _animator.SetBool(IsRunningHash, false);
                _animator.SetBool(IsGrabbingHash, false);
                _animator.SetBool(IsWalkingHash, false);
            }
        }

        public void SetPathfindingTarget(Vector2 targetPosition)
        {
            _pathfinder.destination = targetPosition;
        }

        public void StartPathfindToTarget()
        {
            _pathfinder.maxSpeed = runSpeed;
            _pathfinder.isStopped = false;
            _animator.SetBool(IsRunningHash, true);
            _animator.SetBool(IsWalkingHash, false);
        }

        public void StopPathfinding()
        {
            _animator.SetBool(IsRunningHash, false);
            _pathfinder.isStopped = true;
        }

        public void StartPatrol()
        {
            if (_patrol == null || _pathfinder == null)
                return;

            if (_animator != null)
            {
                _animator.SetBool(IsRunningHash, false);
                _animator.SetBool(IsWalkingHash, true);
            }

            EnsureRuntimePatrolTargetTransforms();
            Vector2 center = transform.position;
            float z = transform.position.z;
            if (!TryGenerateRandomPatrolPair(center, patrolGenerationRadius, out Vector2 pA, out Vector2 pB))
                GetFallbackPatrolPair(center, patrolGenerationRadius, out pA, out pB);

            _patrolRuntimeTargetA.position = new Vector3(pA.x, pA.y, z);
            _patrolRuntimeTargetB.position = new Vector3(pB.x, pB.y, z);
            _patrol.targets = new[] { _patrolRuntimeTargetA, _patrolRuntimeTargetB };
            ResetPatrolInternalState(_patrol);

            _patrol.enabled = true;
            _pathfinder.maxSpeed = patrolSpeed;
            _pathfinder.isStopped = false;
        }

        public void StopPatrol()
        {
            if (_patrol != null)
                _patrol.enabled = false;
            if (_animator != null)
                _animator.SetBool(IsWalkingHash, false);
        }

        [Space]
        [Header("Grab Damage")]
        [SerializeField] int grabDamage = 100;

        public void Grab()
        {
            if (_grabComplete)
                return;

            if (_animator != null)
                _animator.SetBool(IsGrabbingHash, true);
            _isGrabbing = true;

            Vector2 origin = grabPoint != null ? (Vector2)grabPoint.position : (Vector2)transform.position;
            Collider2D hit = Physics2D.OverlapCircle(origin, grabRange, grabTargetLayers);
            if (hit != null)
            {
                _grabbedPlayer = hit.GetComponent<TopDownCharacterControl>();
                if (_grabbedPlayer != null)
                    _grabbedPlayer.BlockInput();
            }
        }

        /// <summary>
        /// Add as an Animation Event on the grab clip.
        /// Deals the actual grab damage partway through the animation.
        /// </summary>
        public void GrabDealDamage()
        {
            if (_grabbedPlayer != null)
            {
                _grabbedPlayer.TakeDamage(grabDamage);
                if (!_grabbedPlayer.enabled)
                    _grabComplete = true;
            }
        }

        /// <summary>
        /// Add as an Animation Event on the grab clip (no parameters).
        /// Clears the animator bool when the grab animation finishes.
        /// </summary>
        public void GrabAnimationEnd()
        {
            if (_grabbedPlayer != null)
            {
                _grabbedPlayer.UnblockInput();
                _grabbedPlayer = null;
            }

            _isGrabbing = false;
            if (_animator != null)
                _animator.SetBool(IsGrabbingHash, false);

            if (_grabComplete)
            {
                StopPathfinding();
                StopPatrol();
            }
        }

        public bool IsTargetInGrabRange(Vector2 targetPosition)
        {
            Vector2 origin = grabPoint != null ? (Vector2)grabPoint.position : (Vector2)transform.position;
            return Vector2.Distance(origin, targetPosition) <= grabRange;
        }

        private void OnDestroy()
        {
            if (_patrolRuntimeTargetA != null)
                Destroy(_patrolRuntimeTargetA.gameObject);
            if (_patrolRuntimeTargetB != null)
                Destroy(_patrolRuntimeTargetB.gameObject);
        }

        private void Update()
        {
            if (_pathfinder.desiredVelocity.x > 0)
                transform.localRotation = _rotationMovingLeft;
            else if (_pathfinder.desiredVelocity.x < 0)
                transform.localRotation = _rotationMovingRight;
        }

        void EnsureRuntimePatrolTargetTransforms()
        {
            if (_patrolRuntimeTargetA == null)
            {
                var goA = new GameObject($"{name}_PatrolGenA");
                _patrolRuntimeTargetA = goA.transform;
            }

            if (_patrolRuntimeTargetB == null)
            {
                var goB = new GameObject($"{name}_PatrolGenB");
                _patrolRuntimeTargetB = goB.transform;
            }
        }

        bool TryGenerateRandomPatrolPair(Vector2 center, float radius, out Vector2 a, out Vector2 b)
        {
            a = b = center;
            if (radius <= 0.01f)
                return false;

            float maxChord = radius * 2f;
            float minSep = Mathf.Min(patrolMinTargetSeparation, maxChord * 0.98f);

            for (int pair = 0; pair < patrolPairAttempts; pair++)
            {
                if (!TryRandomUnobstructedPoint(center, radius, out a))
                    continue;

                for (int inner = 0; inner < patrolRandomPointAttempts; inner++)
                {
                    if (!TryRandomUnobstructedPoint(center, radius, out b))
                        continue;
                    if (Vector2.Distance(a, b) >= minSep)
                        return true;
                }
            }

            return false;
        }

        void GetFallbackPatrolPair(Vector2 center, float radius, out Vector2 a, out Vector2 b)
        {
            float d = Mathf.Max(0.5f, radius * 0.65f);
            a = center + Vector2.left * d;
            b = center + Vector2.right * d;
            if (!IsPatrolPointClear(a))
                a = center + Vector2.up * d;
            if (!IsPatrolPointClear(b))
                b = center + Vector2.down * d;
        }

        bool TryRandomUnobstructedPoint(Vector2 center, float radius, out Vector2 p)
        {
            for (int i = 0; i < patrolRandomPointAttempts; i++)
            {
                p = center + Random.insideUnitCircle * radius;
                if (IsPatrolPointClear(p))
                    return true;
            }

            p = center;
            return false;
        }

        bool IsPatrolPointClear(Vector2 worldPoint)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPoint, patrolColliderCheckRadius, patrolObstacleLayers);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D c = hits[i];
                if (c == null)
                    continue;
                if (c.isTrigger)
                    continue;
                if (c.transform == transform || c.transform.IsChildOf(transform))
                    continue;
                return false;
            }

            return true;
        }

        static void ResetPatrolInternalState(Patrol patrol)
        {
            if (patrol == null)
                return;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = typeof(Patrol);
            type.GetField("index", flags)?.SetValue(patrol, -1);
            type.GetField("switchTime", flags)?.SetValue(patrol, float.NegativeInfinity);
        }
    }
}
