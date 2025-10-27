using UniRx;
using UnityEngine;

public class MoveInput : InputHandler<Vector2>
{
    public MoveInput(ReactiveProperty<Vector2> property) : base(property)
    {
    }
}