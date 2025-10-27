using System;
using UniRx;
using UnityEngine;

public class Character
{
    public ReactiveProperty<Vector3> Position { get; }
    public ReactiveProperty<Quaternion> Rotation { get; }
    public ReactiveProperty<Vector3> Scale { get; }
    public ReactiveProperty<Vector3> Velocity { get; }

    public Character(TransformModel transform)
    {
        if (transform == null)
            throw new ArgumentNullException(nameof(transform));

        Position = new ReactiveProperty<Vector3>(transform.Position);
        Rotation = new ReactiveProperty<Quaternion>(transform.Rotation);
        Scale = new ReactiveProperty<Vector3>(transform.Scale);
        Velocity = new ReactiveProperty<Vector3>(Vector3.zero);
    }
}