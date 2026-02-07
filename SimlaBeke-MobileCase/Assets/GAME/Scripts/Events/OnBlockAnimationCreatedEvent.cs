using sb.eventbus;

public class OnBlockAnimationCreatedEvent : IEvent
{
    public BlockAnimation blockAnimation;

    public OnBlockAnimationCreatedEvent(BlockAnimation blockAnimation)
    {
        this.blockAnimation = blockAnimation;
    }
}
