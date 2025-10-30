using System;
using UniRx;
using UnityEngine;

public class GroundMoveState : ICharacterState
{
    private readonly Character _player;
    private readonly CharacterModel _simulation;
    private readonly IInputService _inputService;
    private readonly CharacterConfig _config;

    private readonly CompositeDisposable _inputSubscribe = new();
    
    public GroundMoveState(Character player, CharacterConfig config, IInputService inputService)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        _simulation = new CharacterModel(config, player.Position.Value);
    }

    private void OnMoveInput(Vector2 input)
    {
        _simulation.MoveByDirection(new Vector3(input.x, 0, input.y));
    }

    private void OnJumpInput(bool isActive)
    {
        if (isActive)
            _simulation.TryJump();
    }

    public void Enter()
    {
        _inputService.GetHandler<MoveInput>().Property.Subscribe(OnMoveInput).AddTo(_inputSubscribe);
        _inputService.GetHandler<JumpInput>().Property.Subscribe(OnJumpInput).AddTo(_inputSubscribe);
    }

    public void FixedTick()
    {
        _simulation.TickSimulate();
        
        _player.Position.Value = _simulation.Transform.Position;
        _player.Rotation.Value = _simulation.Transform.Rotation;
        _player.Scale.Value = _simulation.Transform.Scale;
    }

    public void Exit()
    {
        _inputSubscribe?.Dispose();
    }
}