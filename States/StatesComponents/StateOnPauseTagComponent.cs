using HECSFramework.Core;
using HECSFramework.Documentation;
using System;

namespace Components
{
    [Serializable, Documentation(Doc.Tag, Doc.AI, Doc.State, "Этим компонентом мы отмечаем что стейт текущей ентити на паузе")]
    public class StateOnPauseTagComponent : BaseComponent
    {
        //если у нас будет много стейтов на компоненте, то сюда подъедет дикшенарь
    }
}