using System;
using UniRx;
using UnityEngine;

public class CharacterHeadPresenter : IHandler<StartTick>
{
    private readonly CharacterHead _head;
    private readonly ICameraService _camera;

    public CharacterHeadPresenter(CharacterHead head, ICameraService camera)
    {
        _head = head ?? throw new ArgumentNullException(nameof(head));
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
    }

    public void Handle(StartTick eventData)
    {
        var head = new GameObject("Head").transform;
        
        _head.Position.Subscribe(x =>
        {
            head.position = x;
        });
        _head.Rotation.Subscribe(y =>
        {
            head.rotation = y;
        });
        
        _camera.StartFollow(head);
    }
}