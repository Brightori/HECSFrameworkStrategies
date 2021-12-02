using HECSFramework.Core;
using HECSFramework.Documentation;
using Strategies;
using System;
using System.Collections.Generic;

namespace Components
{
    [Serializable, Documentation(Doc.AI, Doc.Strategy, Doc.State, "Компонент который вешается на ентити которая участвует в данном стейте")]
    public class StateInfoComponent : BaseComponent
    {
        //если у нас на ентити будет несколько стейтов, то скорее всего здесь будет дикшенарь стейтов
        public Stack<LogNode> StateStack = new Stack<LogNode>();
    }
}