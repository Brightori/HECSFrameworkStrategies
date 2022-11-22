using Components;
using HECSFramework.Core;
using Strategies;

[Documentation(Doc.Strategy, Doc.Animation, Doc.Abilities, "this strategy node where we set animation event, for this event we take checkout")]
public sealed class SetAnimationEventCheckoutNode : InterDecision
{
    public override string TitleOfNode { get; } = "Set checkout node";
    private HECSMask AnimationCheckouts = HMasks.GetMask<AnimationCheckOutsHolderComponent>();

    [DropDownIdentifier("AnimationEventIdentifier")]
    public int AnimationEventID;

    [Connection(ConnectionPointType.In, "<float> Animation speed multiplier")]
    public GenericNode<float> Multiplier;

    protected override async void Run(IEntity entity)
    {
        if (entity.TryGetHecsComponent(AnimationCheckouts, out AnimationCheckOutsHolderComponent animationCheckOutsHolder))
        {
            if (animationCheckOutsHolder.TryGetCheckout(AnimationEventID, null, out var info))
            {
                var multiplier = Multiplier == null ? 1 : Multiplier.Value(entity); 
                var wait = new WaitCheckOut { TimeProvider = entity.World.GetSingleComponent<ITimeProvider>(), Timer = info.Timing / multiplier };

                await wait.RunJob(entity.World);
                Next.Execute(entity);
                return;
            }
        }
        
        Next.Execute(entity);
    }

    public struct WaitCheckOut : IHecsJob
    {
        public float Timer;
        public ITimeProvider TimeProvider;

        public bool IsComplete()
        {
            return Timer <= 0;
        }

        public void Run()
        {
            Timer -= TimeProvider.DeltaTime;
        }
    }
}