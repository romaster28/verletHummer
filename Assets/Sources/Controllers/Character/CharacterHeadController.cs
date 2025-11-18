using System;
using UniRx;
using UnityEngine;

public class CharacterHeadController : IHandler<StartTick>, IHandler<Tick>, IHandler<FixedTick>
{
    private readonly IInputService _input;
    private readonly ISettingsService _settings;
    private readonly CharacterConfig _config;
    private readonly CharacterHead _head;
    private readonly Character _character;
    
    private float _targetX;
    private float _targetY;
    
    private float _velocityX;
    private float _velocityY;
    
    private Vector2 _currentInput;

    public CharacterHeadController(IInputService input, ISettingsService settings, CharacterFacade characterFacade)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _config = characterFacade.Config;
        _head = characterFacade.Head;
        _character = characterFacade.Character;
    }

    private void OnLookInput(Vector2 input)
    {
        float sensitivity = _settings.Read<SensitivityValue>().Property.Value;
        _currentInput = input * sensitivity;
    }

    public void Handle(StartTick _)
    {
        _input.GetHandler<LookInput>().Property.Subscribe(OnLookInput);
        
    }

    public void Handle(Tick _)
    {
        if (_currentInput.sqrMagnitude < 0.001f)
            return;

        float smoothTime = _settings.Read<MouseSmoothValue>().Property.Value;
        
        _targetY += _currentInput.x;
        _targetX -= _currentInput.y;
        
        _targetX = Mathf.Clamp(_targetX, _config.VerticalAngleRange.x, _config.VerticalAngleRange.y);
        
        Vector3 euler = _head.Rotation.Value.eulerAngles;
        float currentX = NormalizeAngle(euler.x);
        float currentY = NormalizeAngle(euler.y);
        
        ResetVelocityIfNeeded(ref _velocityX, -_currentInput.y);
        ResetVelocityIfNeeded(ref _velocityY, _currentInput.x);
        
        float newX = Mathf.SmoothDampAngle(currentX, _targetX, ref _velocityX, smoothTime);
        float newY = Mathf.SmoothDampAngle(currentY, _targetY, ref _velocityY, smoothTime);
        
        _head.Rotation.Value = Quaternion.Euler(newX, newY, 0f);
        
        _currentInput = Vector2.zero;
    }

    public void Handle(FixedTick eventData)
    {
        _head.Position.Value = _character.Position.Value + Vector3.up * _config.Height;
    }

    private void ResetVelocityIfNeeded(ref float velocity, float inputAxis)
    {
        if (Mathf.Abs(inputAxis) < 0.1f) return;

        float inputSign = Mathf.Sign(inputAxis);
        float velocitySign = Mathf.Sign(velocity);
        
        if (!Mathf.Approximately(inputSign, velocitySign) && Mathf.Abs(velocity) > 50f)
        {
            velocity = 0f;
        }
    }

    // Приводим угол в диапазон [-180, 180]

    private static float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}