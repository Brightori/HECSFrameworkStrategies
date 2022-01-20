using HECSFramework.Core;
using Strategies;
using Systems;

namespace Commands
{
	[Documentation(Doc.AI, Doc.Strategy, "��� ������� �������� ��� ���� ������������� ��������� � ������ ��� ���������� ������� ������, ������������ ������ �� " + nameof(CompleteFinalNode) + " �������������� ������ � " + nameof(AINPCSystem))]
	public struct NeedDecisionCommand : ICommand
	{
	}
}