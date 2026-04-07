using BehaviorDesigner.Runtime.Tasks;

namespace AIEnemy.Crawler.Behaviors
{
    [TaskCategory("HushAIEnemy")]
    public class IsCrawlerGrabbing : Conditional
    {
        CrawlerEnemy _crawler;

        public override void OnAwake()
        {
            _crawler = gameObject.GetComponent<CrawlerEnemy>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_crawler == null)
                return TaskStatus.Failure;

            return _crawler.IsGrabbing ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
