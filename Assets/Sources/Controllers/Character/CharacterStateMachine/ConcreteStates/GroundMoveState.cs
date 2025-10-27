using System;
using UniRx;
using UnityEngine;

public class GroundMoveState : ICharacterState
{
    private readonly Character _player;
    private readonly CharacterModel _simulation;
    private readonly IInputService _inputService;
    private readonly CharacterConfig _config;

    private IDisposable _moveSubscribe;
    
    public GroundMoveState(Character player, CharacterConfig config, IInputService inputService)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        _simulation = new CharacterModel(config, player.Position.Value);
    }

    public void Enter()
    {
        _moveSubscribe = _inputService.GetHandler<MoveInput>().Property.Subscribe(OnMoveInput);
    }

    public void Exit()
    {
        _moveSubscribe?.Dispose();
    }

    public void FixedTick()
    {
        _simulation.TickSimulate();
    }

    private void OnMoveInput(Vector2 input)
    {
        _simulation.MoveByDirection(input);
    }
}