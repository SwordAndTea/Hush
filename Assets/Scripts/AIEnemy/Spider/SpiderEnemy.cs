using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;

namespace AIEnemy.Spider
{
    [TaskCategory("HushAIEnemy")]
    public class SpiderEnemy : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private AIPath _aiPath;

        public bool ReachedDestination => _aiPath.reachedEndOfPath;
        public bool disableAIPathfinder = true;
    

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _aiPath = GetComponent<AIPath>();
            _aiPath.isStopped = disableAIPathfinder; // don't put this to Start method, as the Start Method maybe call after SpiderMovement action OnStart method
        }

        public void SetPathfindingTarget(Vector2 targetPosition)
        {
            _aiPath.destination = targetPosition;
        }

        public void StartPathfindToTarget()
        {
            _aiPath.isStopped = false;
        }

        public void StopPathfinding()
        {
            _aiPath.isStopped = true;
        }


        // Update is called once per frame
        private void Update()
        {
            if (_aiPath.desiredVelocity.x > 0)
            {
                _spriteRenderer.flipX = false;
            }
            else if (_aiPath.desiredVelocity.x < 0)
            {
                _spriteRenderer.flipX = true;
            }
        }
    }
}