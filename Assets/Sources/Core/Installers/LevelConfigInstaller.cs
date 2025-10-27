using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Installers/LevelConfig")]
public class LevelConfigInstaller : ScriptableObjectInstaller<LevelConfigInstaller>
{
    [SerializeField] private CharacterConfig _character;
    
    public override void InstallBindings()
    {
        Container.BindInstances(_character);
    }
}