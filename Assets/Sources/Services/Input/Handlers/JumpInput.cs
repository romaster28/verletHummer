using UniRx;

public class JumpInput : InputHandler<bool>
{
    public JumpInput(ReactiveProperty<bool> property) : base(property)
    {
    }
}