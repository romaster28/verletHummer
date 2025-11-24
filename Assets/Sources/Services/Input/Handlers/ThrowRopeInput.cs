using UniRx;

public class ThrowRopeInput : InputButtonHandler
{
    public ThrowRopeInput(ReactiveProperty<bool> property) : base(property)
    {
    }
}