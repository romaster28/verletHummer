using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Installers/LevelConfig")]
public class LevelConfigInstaller : ScriptableObjectInstaller<LevelConfigInstaller>
{
    [SerializeField] private CoreConfig _core;
    [FormerlySerializedAs("_ropeMove")] [SerializeField] private RopeSwingConfig _ropeSwing;
    [SerializeField] private CharacterConfig _character;
    [SerializeField] private RopeConfig _rope;
    
    public override void InstallBindings()
    {
        Container.BindInstances(_character, _rope, _core, _ropeSwing);
    }
}