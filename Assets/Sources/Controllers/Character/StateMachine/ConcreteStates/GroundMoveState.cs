using System;
using UniRx;
using UnityEngine;

public class GroundMoveState : BaseCharacterState
{
    private readonly Character _player;
    private readonly CharacterHead _head;
    private readonly IInputService _inputService;
    private readonly CharacterConfig _config;
    private readonly RopeThrower _thrower;

    private CharacterModel _simulation;
    private CompositeDisposable _inputSubscribe;

    public GroundMoveState(CharacterFacade characterFacade, RopeThrower thrower, IInputService inputService)
    {
        _player = characterFacade.Character;
        _config = characterFacade.Config;
        _head = characterFacade.Head;
        _thrower = thrower ?? throw new ArgumentNullException(nameof(thrower));
        _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
    }

    private void OnMoveInput(Vector2 input)
    {
        _simulation.MoveByDirection(new Vector3(input.x, 0, input.y));
    }

    private void OnHeadRotationChanged(Quaternion rotation)
    {
        _simulation.RotateToAngle(rotation.eulerAngles.y);
    }

    private void OnJumpInput(bool isActive)
    {
        if (isActive)
            _simulation.TryJump();
    }

    private void OnThrowRopeInput(bool isActive)
    {
        if (!isActive)
            return;

        _thrower.ThrowToCrosshair();
    }

    public override void Enter()
    {
        _simulation = new CharacterModel(_config, _player.Position.Value);
        
        _inputSubscribe = new CompositeDisposable();
        _inputService.GetHandler<MoveInput>().Property.Subscribe(OnMoveInput).AddTo(_inputSubscribe);
        _inputService.GetHandler<JumpInput>().Property.Subscribe(OnJumpInput).AddTo(_inputSubscribe);
        _inputService.GetHandler<ThrowRopeInput>().Property.Subscribe(OnThrowRopeInput).AddTo(_inputSubscribe);

        _head.Rotation.Subscribe(OnHeadRotationChanged);
    }

    public override void FixedTick()
    {
        _simulation.TickSimulate();

        _player.Position.Value = _simulation.Transform.Position;
        _player.Rotation.Value = _simulation.Transform.Rotation;
        _player.Scale.Value = _simulation.Transform.Scale;
    }

    public override void Exit()
    {
        _inputSubscribe?.Dispose();
    }
}