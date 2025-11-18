using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Installers/LevelConfig")]
public class LevelConfigInstaller : ScriptableObjectInstaller<LevelConfigInstaller>
{
    [SerializeField] private CoreConfig _core;
    [SerializeField] private RopeMoveConfig _ropeMove;
    [SerializeField] private CharacterConfig _character;
    [SerializeField] private RopeConfig _rope;
    
    public override void InstallBindings()
    {
        Container.BindInstances(_character, _rope, _core, _ropeMove);
    }
}