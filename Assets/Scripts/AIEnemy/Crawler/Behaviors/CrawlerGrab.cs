using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AIEnemy.Crawler.Behaviors
{
    [TaskCategory("HushAIEnemy")]
    public class CrawlerGrab : Action
    {
        public SharedVector2 TargetPosition;

        private CrawlerEnemy _crawler;

        public override void OnAwake()
        {
            _crawler = gameObject.GetComponent<CrawlerEnemy>();
        }

        public override void OnStart()
        {
            if (_crawler != null && !_crawler.IsGrabbing)
            {
                _crawler.StopPathfinding();
                _crawler.Grab();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_crawler.IsGrabbing)
                return TaskStatus.Running;

            return TaskStatus.Success;
        }
    }
}
