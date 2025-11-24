using System;
using UniRx;
using UnityEngine;
using Zenject;

public class GroundMoveState : BaseCharacterState
{
    private readonly Character _player;
    private readonly CharacterHead _head;
    private readonly IInputService _inputService;
    private readonly CharacterConfig _config;
    private readonly RopeThrower _thrower;
    private readonly SignalBus _signalBus;
    private readonly RopeSwingConfig _ropeSwing;
    private readonly ICharacterStateMachine _characterStateMachine;
    
    private CharacterModel _simulation;
    private CompositeDisposable _inputSubscribe;

    public GroundMoveState(CharacterFacade characterFacade, RopeThrower thrower, IInputService inputService, SignalBus signalBus, RopeSwingConfig ropeSwingConfig)
    {
        _player = characterFacade.Character;
        _config = characterFacade.Config;
        _head = characterFacade.Head;
        _characterStateMachine = characterFacade.StateMachine;
        _thrower = thrower ?? throw new ArgumentNullException(nameof(thrower));
        _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        _signalBus = signalBus ?? throw new ArgumentNullException(nameof(signalBus));
        _ropeSwing = ropeSwingConfig ?? throw new ArgumentNullException(nameof(ropeSwingConfig));
    }

    private void OnMoveInput(Vector2 input)
    {
        _simulation.MoveByDirection(new Vector3(input.x, 0, input.y));
    }

    private void OnHeadRotationChanged(Quaternion rotation)
    {
        _simulation.RotateToAngle(rotation.eulerAngles.y);
    }

    private void OnJumpInput()
    {
        _simulation.TryJump();
    }

    private void OnThrowRopeInput()
    {
        _thrower.ThrowToCrosshair();
    }

    private void OnRopeSpawned(RopeSpawned signal)
    {
        if (!signal.Connected)
            return;

        float differenceConnectY = signal.End.y - signal.Start.y;

        if (differenceConnectY >= _ropeSwing.MinDistanceYConnectForUpMove)
            _characterStateMachine.SetState<RopeSwingState>();
    }

    public override void Enter()
    {
        _simulation = new CharacterModel(_config, _player.Position.Value);

        _inputSubscribe = new CompositeDisposable();
        _inputService.GetHandler<MoveInput>().Property.Subscribe(OnMoveInput).AddTo(_inputSubscribe);
        _inputService.GetHandler<JumpInput>().Pressed += OnJumpInput;
        _inputService.GetHandler<ThrowRopeInput>().Pressed += OnThrowRopeInput;
        _head.Rotation.Subscribe(OnHeadRotationChanged).AddTo(_inputSubscribe);
        
        _signalBus.Subscribe<RopeSpawned>(OnRopeSpawned);
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
        _signalBus.Unsubscribe<RopeSpawned>(OnRopeSpawned);
        _inputService.GetHandler<JumpInput>().Pressed -= OnJumpInput;
        _inputService.GetHandler<ThrowRopeInput>().Pressed -= OnThrowRopeInput;
    }
}