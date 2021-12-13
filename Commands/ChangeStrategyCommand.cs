using HECSFramework.Core;
using HECSFramework.Documentation;
using Strategies;

namespace Commands
{
	[Documentation(Doc.NPC, Doc.AI, "Этой командой мы изменяем в целом стратегию поведения у нпс")]
	public struct ChangeStrategyCommand : ICommand
	{
		public Strategy Strategy;
	}
}