using Components;
using HECSFramework.Core;
using System.Linq;

namespace Systems
{
    [Documentation(Doc.GameLogic, Doc.AI, Doc.Strategy, "Эта система живет  мире и добавляется через парт часть гейм контроллера, она отвечает за поддержку глобальной логики для стратегий")]
    public class StrategiesMainServiceSystem : BaseSystem
    {
        private ConcurrencyList<IEntity> stackInfos;
        private HECSMask StateInfoComponentMask = HMasks.GetMask<StateInfoComponent>();


        public override void InitSystem()
        {
            stackInfos = Owner.World.Filter(StateInfoComponentMask);

#if UNITY_EDITOR
            Owner.World.GlobalUpdateSystem.FinishUpdate += React;
#endif
        }

#if UNITY_EDITOR
        private void React()
        {
            var count = stackInfos.Count;
            var direct = stackInfos.DirectAccess();

            for (int i = 0; i < count; i++)
            {
                var info = direct[i].GetHECSComponent<StateInfoComponent>(ref StateInfoComponentMask);

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
