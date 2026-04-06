using BehaviorDesigner.Runtime.Tasks;

namespace AIEnemy.Spider.Behaviors
{
    [TaskCategory("HushAIEnemy")]
    public class SpiderPatrol : Action
    {
        private SpiderEnemy _spider;

        public override void OnAwake()
        {
            _spider = gameObject.GetComponent<SpiderEnemy>();
        }

        public override void OnStart()
        {
            if (_spider != null && _spider.CanPatrol)
                _spider.StartPatrol();
        }

        public override TaskStatus OnUpdate()
        {
            if (_spider == null || !_spider.CanPatrol)
                return TaskStatus.Failure;

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            if (_spider != null)
                _spider.StopPatrol();
        }
    }
}
