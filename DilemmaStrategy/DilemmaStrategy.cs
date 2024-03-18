using HECSFramework.Core;
using UnityEngine;

namespace Strategies
{

    [CreateAssetMenu(menuName = "Strategies/Dilemma Strategy")]
    [Documentation(Doc.GameLogic, Doc.HECS, Doc.Strategy, "This variant of strategy have two decisions on exit, not restart of strategy, u should use it how part complex decisions, not a main strategy as well")]
    public sealed class DilemmaStrategy : Strategy
    {
    }
}