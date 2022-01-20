using HECSFramework.Core;
using Strategies;
using Systems;

namespace Commands
{
	[Documentation(Doc.AI, Doc.Strategy, "Эта команда означает что пора перезапустить стратегию и пройти всю логическую цепочку заново, отправляется обычно из " + nameof(CompleteFinalNode) + " обрабатывается обычно в " + nameof(AINPCSystem))]
	public struct NeedDecisionCommand : ICommand
	{
	}
}