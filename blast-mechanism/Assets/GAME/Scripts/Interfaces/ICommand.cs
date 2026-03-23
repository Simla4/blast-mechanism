
using DG.Tweening;

public interface ICommand 
{
    Tween Execute();
    float Duration { get; }
}
