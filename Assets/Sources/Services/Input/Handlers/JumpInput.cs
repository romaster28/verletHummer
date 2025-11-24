using UniRx;

public class JumpInput : InputButtonHandler
{
    public JumpInput(ReactiveProperty<bool> property) : base(property)
    {
    }
}