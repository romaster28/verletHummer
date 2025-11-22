using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class RopeSwingState : BaseCharacterState
{
    private readonly Character _character;
    private readonly RopeMoveConfig _config;
    private readonly IRopeService _ropeService;
    private readonly IInputService _inputService;
    private readonly ICharacterStateMachine _characterStateMachine;
    private readonly RopeThrower _thrower;
    private readonly SignalBus _signalBus;

    private Rope _connected;
    private PendulumSimulation _simulation;
    private CompositeDisposable _inputSubscribe;

    public RopeSwingState(Character character, RopeMoveConfig config, IRopeService ropeService,
        IInputService inputService, ICharacterStateMachine stateMachine, RopeThrower thrower, SignalBus signalBus)
    {
        _character = character ?? throw new ArgumentNullException(nameof(character));
        _ropeService = ropeService ?? throw new ArgumentNullException(nameof(ropeService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        _characterStateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        _thrower = thrower ?? throw new ArgumentNullException(nameof(thrower));
        _signalBus = signalBus ?? throw new ArgumentNullException(nameof(signalBus));
    }

    public override void Enter()
    {
        StartSwing(_ropeService.GetSpawned().Last());

        _inputSubscribe = new();
        _inputService.GetHandler<JumpInput>().Property.Subscribe(OnJumpInputChanged).AddTo(_inputSubscribe);
        _inputService.GetHandler<ThrowRopeInput>().Property.Subscribe(OnThrowRopeInput).AddTo(_inputSubscribe);
        
        _signalBus.Subscribe<RopeSpawned>(Callback);
    }

    public override void Exit()
    {
        _inputSubscribe?.Dispose();
        _signalBus.Unsubscribe<RopeSpawned>(Callback);
    }

    public override void FixedTick()
    {
        _simulation.Simulate();
        _character.Position.Value = _simulation.Bob.Value;
    }

    private void StartSwing(Rope rope)
    {
        if (_connected == rope)
            return;
        
        var config = new PendulumConfig(_character.Position.Value, rope.Target, _config.Gravity);
        _simulation = new PendulumSimulation(config, -_config.InitialSwingForce);
        
        if (_connected != null && _ropeService.IsActive(_connected))
            _ropeService.Disconnect(_connected);
        
        _connected = rope;
    }

    private void OnThrowRopeInput(bool isActive)
    {
        if (!isActive)
            return;

        _thrower.ThrowToCrosshair();
    }

    private void Callback(RopeSpawned signal)
    {
        if (!signal.Connected)
            return;

        StartSwing(signal.Rope);
    }

    private void OnJumpInputChanged(bool pressed)
    {
        if (!pressed)
            return;

        _ropeService.Disconnect(_connected);
        _characterStateMachine.SetState<GroundMoveState>();
    }
}