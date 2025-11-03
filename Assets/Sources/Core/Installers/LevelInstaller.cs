using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [Required] [SerializeField] private Transform _spawnPoint;

    private CharacterController _characterController;
    private CharacterPresenter _characterPresenter;
    private CharacterHeadController _characterHeadController;
    private CharacterHeadPresenter _characterHeadPresenter;
    private RopeController _ropeController;

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
        Container.Bind<RopeThrower>().AsSingle();
        Container.DeclareSignal<RopeSpawned>();
    }
    
    private void BindCharacter()
    {
        Container.Bind<Character>().AsSingle().WithArguments(_spawnPoint.ToModel());
        Container.Bind<CharacterHead>().AsSingle().WithArguments(_spawnPoint.ToModel());
        
        Container.Bind<ICharacterStateMachine>().To<CharacterStateMachine>().AsSingle();
        Container.Bind<CharacterStatesCachedPoolProvider>().AsSingle().WhenInjectedInto<CharacterStateMachine>();
        Container.Bind<IEnumerable<ICharacterState>>().FromMethod(CreateCharacterStates).WhenInjectedInto<CharacterStatesCachedPoolProvider>();

        _characterController = Container.Instantiate<CharacterController>();
        _characterPresenter = Container.Instantiate<CharacterPresenter>();

        _characterHeadController = Container.Instantiate<CharacterHeadController>();
        _characterHeadPresenter = Container.Instantiate<CharacterHeadPresenter>();

        _ropeController = Container.Instantiate<RopeController>(); 
        IEnumerable<ICharacterState> CreateCharacterStates(InjectContext ctx)
        {
            return new ICharacterState[]
            {
                ctx.Container.Instantiate<GroundMoveState>(),
                ctx.Container.Instantiate<RopeMoveState>()
            };
        }
    }

    private void BindGameLoop()
    {
        Container.Bind<IHandler<StartTick>>()
            .FromInstance(new CompositeHandler<StartTick>(new IHandler<StartTick>[]
            {
                _characterController,
                _characterPresenter,
                _characterHeadController,
                _characterHeadPresenter
            })).WhenInjectedInto<GameLoop>();

        Container.Bind<IHandler<Tick>>()
            .FromInstance(new CompositeHandler<Tick>(new IHandler<Tick>[]
            {
                _characterHeadController
            })).WhenInjectedInto<GameLoop>();

        Container.Bind<IHandler<FixedTick>>()
            .FromInstance(new CompositeHandler<FixedTick>(new IHandler<FixedTick>[]
            {
                _characterController,
                _characterHeadController,
                _ropeController
            })).WhenInjectedInto<GameLoop>();

        Container.Bind<IHandler<DestroyTick>>().FromInstance(new CompositeHandler<DestroyTick>(
            new IHandler<DestroyTick>[]
            {
                _characterPresenter
            }));

        Container.InstantiateComponent<GameLoop>(new GameObject("Game Loop"));
    }

    private void BindServices()
    {
        Container.Bind<IInputService>().To<OldEditorInputService>().AsSingle();
        Container.Bind<IAssetProvider>().To<ResourcesAssetProvider>().AsSingle();
        Container.Bind<ICameraService>().To<CinemachineCameraService>().AsSingle();
        Container.Bind<IRopeService>().To<RopeService>().AsSingle();
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