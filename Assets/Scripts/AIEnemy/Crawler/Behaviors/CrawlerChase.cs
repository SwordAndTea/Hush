using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AIEnemy.Crawler.Behaviors
{
    [TaskCategory("HushAIEnemy")]
    public class CrawlerChase : Action
    {
        public SharedVector2 TargetPosition;

        private CrawlerEnemy _crawler;

        public override void OnAwake()
        {
            _crawler = gameObject.GetComponent<CrawlerEnemy>();
        }

        public override void OnStart()
        {
            _crawler.SetPathfindingTarget(TargetPosition.Value);
            _crawler.StartPathfindToTarget();
        }

        public override TaskStatus OnUpdate()
        {
            if (_crawler.IsTargetInGrabRange(TargetPosition.Value))
            {
                _crawler.StopPathfinding();
                return TaskStatus.Success;
            }

            _crawler.SetPathfindingTarget(TargetPosition.Value);
            return TaskStatus.Running;
        }
    }
}
