using BehaviorDesigner.Runtime.Tasks;

namespace AIEnemy.Crawler.Behaviors
{
    [TaskCategory("HushAIEnemy")]
    public class CrawlerPatrol : Action
    {
        private CrawlerEnemy _crawler;

        public override void OnAwake()
        {
            _crawler = gameObject.GetComponent<CrawlerEnemy>();
        }

        public override void OnStart()
        {
            if (_crawler != null && _crawler.CanPatrol)
                _crawler.StartPatrol();
        }

        public override TaskStatus OnUpdate()
        {
            if (_crawler == null || !_crawler.CanPatrol)
                return TaskStatus.Failure;

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            if (_crawler != null)
                _crawler.StopPatrol();
        }
    }
}
