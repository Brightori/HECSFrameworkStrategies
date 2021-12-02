using HECSFramework.Core;
using HECSFramework.Documentation;

namespace Systems
{
    [Documentation(Doc.GameLogic, Doc.Strategy, "Эта система живет  мире и добавляется через парт часть гейм контроллера, она отвечает за поддержку глобальной логики для стратегий")]
    public class StrategiesMainServiceSystem : BaseSystem
    {
        private ConcurrencyList<IEntity> stackInfos;

        public override void InitSystem()
        {
            Owner.World.GlobalUpdateSystem.FinishUpdate += React;
            stackInfos = Owner.World.Filter(HMasks.StateInfoComponent);
        }

        private void React()
        {
#if UNITY_EDITOR
            var count = stackInfos.Count;
            var direct = stackInfos.DirectAccess();

            for (int i = 0; i < count; i++)
                direct[i].GetStateInfoComponent().StateStack.Clear();
#endif
        }
    }
}
