using HECSFramework.Core;
using Systems;

namespace HECSFramework.Unity
{
    [Documentation(Doc.GameLogic, Doc.Strategy, "Этот кусочек бейз контроллера содержит базовые системы для общей логики стратегий")]
    public abstract partial class BaseGameController
    {
        partial void StrategiesInit()
        {
            var strategiesController = Entity.Get("Strategies Controller");
            strategiesController.AddHecsSystem(new StrategiesMainServiceSystem());
            strategiesController.AddHecsSystem(new StateUpdateSystem());
            strategiesController.Init();
        }
    }
}
