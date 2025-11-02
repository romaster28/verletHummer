using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class CharacterConfig : SerializableCharacterConfig
{
    [Min(0)] [SerializeField] private float _height = 3;
    [Vector2AsRange] [SerializeField] private Vector2 _verticalAngleRange = new(-65, 65);

    public float Height => _height;
    public Vector2 VerticalAngleRange => _verticalAngleRange;
}