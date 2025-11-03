using UniRx;

public class ThrowRopeInput : InputHandler<bool>
{
    public ThrowRopeInput(ReactiveProperty<bool> property) : base(property)
    {
    }
}