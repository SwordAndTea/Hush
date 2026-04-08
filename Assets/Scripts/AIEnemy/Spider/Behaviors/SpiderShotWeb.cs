using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AIEnemy.Spider.Behaviors
{
    [TaskCategory("HushAIEnemy")]
    public class SpiderShotWeb : Action
    {
        public SharedVector2 TargetPosition;

        private SpiderEnemy _spider;

        public override void OnAwake()
        {
            _spider = gameObject.GetComponent<SpiderEnemy>();
        }

        public override void OnStart()
        {
            if (_spider != null && !_spider.IsShotingWeb) {
                _spider.ShootWeb(TargetPosition.Value);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (_spider == null)
                return TaskStatus.Failure;

            if (_spider.IsShotingWeb)
            {
                return TaskStatus.Running;
            }
            return TaskStatus.Success;
        }
    }
}
