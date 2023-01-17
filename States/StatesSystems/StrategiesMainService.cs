using Components;
using HECSFramework.Core;

namespace Systems
{
    [Documentation(Doc.GameLogic, Doc.AI, Doc.Strategy, "Эта система живет  мире и добавляется через парт часть гейм контроллера, она отвечает за поддержку глобальной логики для стратегий")]
    public class StrategiesMainServiceSystem : BaseSystem
    {
        private HECSList<IEntity> stackInfos;

        public override void InitSystem()
        {
            //todo filter
            //stackInfos = Owner.World.Filter(StateInfoComponentMask);

#if UNITY_EDITOR
            Owner.World.GlobalUpdateSystem.FinishUpdate += React;
#endif
        }

#if UNITY_EDITOR
        private void React()
        {
            var count = stackInfos.Count;
            var direct = stackInfos.Data;

            for (int i = 0; i < count; i++)
            {
                var info = direct[i].GetComponent<StateInfoComponent>();

                if (info.NeedInfo)
                {
                    if (info.NeedClean)
                    {
                        info.StateStack.Clear();
                        info.CurrentCycle = 0;
                        info.NeedClean = false;
                    }

                    if (info.CurrentCycle > info.MaxCycles)
                        info.NeedInfo = false;

                    info.CurrentCycle++;
                }
            }
        }
#endif
    }
}
