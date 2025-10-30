using UniRx;
using UnityEngine;

public class LookInput : InputHandler<Vector2>
{
    public LookInput(ReactiveProperty<Vector2> property) : base(property)
    {
    }
}