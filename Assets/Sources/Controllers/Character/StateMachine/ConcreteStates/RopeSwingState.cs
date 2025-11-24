using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class RopeSwingState : BaseCharacterState
{
    private readonly Character _character;
    private readonly RopeSwingConfig _config;
    private readonly IRopeService _ropeService;
    private readonly IInputService _inputService;
    private readonly ICharacterStateMachine _characterStateMachine;
    private readonly RopeThrower _thrower;
    private readonly SignalBus _signalBus;

    private Rope _connected;
    private PendulumSimulation _simulation;
    private CompositeDisposable _inputSubscribe;

    public RopeSwingState(RopeFacade ropeFacade, CharacterFacade characterFacade, IInputService inputService, SignalBus signalBus)
    {
        _character = characterFacade.Character;
        _ropeService = ropeFacade.Service;
        _config = ropeFacade.SwingConfig;
        _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        _characterStateMachine = characterFacade.StateMachine;
        _thrower = ropeFacade.Thrower;
        _signalBus = signalBus ?? throw new ArgumentNullException(nameof(signalBus));
    }

    public override void Enter()
    {
        StartSwing(_ropeService.GetSpawned().Last());

        _inputSubscribe = new();
        _inputService.GetHandler<JumpInput>().Pressed += OnJumpInput;
        _inputService.GetHandler<ThrowRopeInput>().Pressed += OnThrowRopeInput;
        
        _signalBus.Subscribe<RopeSpawned>(Callback);
    }

    public override void Exit()
    {
        _inputSubscribe?.Dispose();
        _signalBus.Unsubscribe<RopeSpawned>(Callback);
        _inputService.GetHandler<JumpInput>().Pressed -= OnJumpInput;
        _inputService.GetHandler<ThrowRopeInput>().Pressed -= OnThrowRopeInput;
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
        
        Vector3 directionToTarget = (rope.Target - _character.Position.Value).normalized;
        float distanceToTarget = Vector3.Distance(rope.Target, _character.Position.Value);
        Vector3 bobStart = _character.Position.Value + directionToTarget * MathHelper.GetPercentage(distanceToTarget, _config.ForceCropRopeDistancePercentage);
        var config = new PendulumConfig(bobStart, rope.Target, _config.Gravity);
        _simulation = new PendulumSimulation(config, -_config.InitialSwingForce);
        
        if (_connected != null && _ropeService.IsActive(_connected))
            _ropeService.Disconnect(_connected);
        
        _connected = rope;
    }

    private void OnThrowRopeInput()
    {
        _thrower.ThrowToCrosshair();
    }

    private void Callback(RopeSpawned signal)
    {
        if (!signal.Connected)
            return;

        StartSwing(signal.Rope);
    }

    private void OnJumpInput()
    {
        if (_ropeService.IsActive(_connected))
            _ropeService.Disconnect(_connected);
        
        _characterStateMachine.SetState<GroundMoveState>();
    }
}