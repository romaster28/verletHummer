using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ISettingsService>().To<SettingsService>().AsSingle();
        Cursor.lockState = CursorLockMode.Locked;
    }
}