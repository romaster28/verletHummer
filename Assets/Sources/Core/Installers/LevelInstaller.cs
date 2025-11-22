using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [Required] [SerializeField] private Transform _spawnPoint;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        BindServices();
        BindRope();
        BindCharacter();
        BindGameLoop();
    }

    private void BindRope()
    {
        Container.BindInterfacesAndSelfTo<RopeController>().AsSingle();
        Container.Bind<RopePresenter>().AsSingle().NonLazy();
        Container.Bind<RopeThrower>().AsSingle();
        
        Container.DeclareSignal<RopeSpawned>();
        Container.DeclareSignal<RopeDeSpawned>();
        Container.DeclareSignal<RopeDisconnected>();
    }

    private void BindCharacter()
    {
        Container.Bind<Character>().AsSingle().WithArguments(_spawnPoint.ToModel());
        Container.Bind<CharacterHead>().AsSingle().WithArguments(_spawnPoint.ToModel());

        Container.Bind<CharacterFacade>().AsSingle();
        
        Container.Bind<ICharacterStateMachine>().To<CharacterStateMachine>().AsSingle();
        Container.BindFactory<Type, BaseCharacterState, CharacterStateFactory>().FromFactory<PooledCharacterStateFactory>();

        Container.BindInterfacesAndSelfTo<CharacterController>().AsSingle();
        Container.BindInterfacesAndSelfTo<CharacterPresenter>().AsSingle();
        Container.BindInterfacesAndSelfTo<CharacterHeadController>().AsSingle();
        Container.BindInterfacesAndSelfTo<CharacterHeadPresenter>().AsSingle();
    }

    private void BindGameLoop()
    {
        Container.Bind<IHandler<StartTick>>().To<StartHandler>().AsSingle().WhenInjectedInto<GameLoop>();
        Container.Bind<IHandler<Tick>>().To<TickHandler>().AsSingle().WhenInjectedInto<GameLoop>();
        Container.Bind<IHandler<FixedTick>>().To<FixedTickHandler>().AsSingle().WhenInjectedInto<GameLoop>();
        Container.Bind<IHandler<DestroyTick>>().To<DestroyTickHandler>().AsSingle().WhenInjectedInto<GameLoop>();

        Container.Bind<GameLoop>().FromNewComponentOnNewGameObject().WithGameObjectName("Game Loop").AsSingle().NonLazy();
    }

    private void BindServices()
    {
        Container.Bind<IInputService>().To<OldEditorInputService>().AsSingle();
        Container.Bind<IAssetProvider>().To<ResourcesAssetProvider>().AsSingle();
        Container.Bind<ICameraService>().To<CinemachineCameraService>().AsSingle();
        Container.Bind<IRopeService>().To<RopeService>().AsSingle();
        
        #if UNITY_EDITOR
        Container.Bind<ILogService>().To<DebugLogService>().AsSingle();
        #else
        Container.Bind<ILogService>().To<StubLogService>().AsSingle();
        #endif
    }

#if UNITY_EDITOR
    [Button("Visualize Character in Editor")]
    private void VisualizeCharacter()
    {
        var assetProvider = new ResourcesAssetProvider();
        var characterViewPrefab = assetProvider.LoadAsset<CharacterView>("somePath");
        if (characterViewPrefab == null || _spawnPoint == null)
            return;

        var tempView = UnityEditor.PrefabUtility.InstantiatePrefab(characterViewPrefab) as CharacterView;
        if (tempView != null)
        {
            tempView.transform.position = _spawnPoint.position;
            tempView.gameObject.name = "Editor_Preview_Character";
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(tempView.gameObject.scene);
        }
    }
#endif
}