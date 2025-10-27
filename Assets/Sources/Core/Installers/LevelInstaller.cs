using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [Required] [SerializeField] private Transform _spawnPoint;

    private CharacterController _characterController;

    public override void InstallBindings()
    {
        BindServices();
        BindCharacter();
        BindGameLoop();
    }

    private void BindCharacter()
    {
        Container.Bind<Character>().AsSingle().WithArguments(_spawnPoint.ToModel());
        Container.Bind<ICharacterStateMachine>().To<CharacterStateMachine>().AsSingle();
        Container.Bind<CharacterStatesCachedPoolProvider>().AsSingle().WhenInjectedInto<CharacterStateMachine>();
        Container.Bind<IEnumerable<ICharacterState>>().FromMethod(CreateCharacterStates).WhenInjectedInto<CharacterStatesCachedPoolProvider>();

        _characterController = Container.Instantiate<CharacterController>();

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
                _characterController
            }))
            .WhenInjectedInto<GameLoop>();

        Container.Bind<IHandler<Tick>>()
            .FromInstance(new CompositeHandler<Tick>(new IHandler<Tick>[]
            {
                
            })).WhenInjectedInto<GameLoop>();

        Container.Bind<IHandler<FixedTick>>()
            .FromInstance(new CompositeHandler<FixedTick>(new IHandler<FixedTick>[]
            {
                _characterController
            }))
            .WhenInjectedInto<GameLoop>();

        Container.InstantiateComponent<GameLoop>(new GameObject("Game Loop"));
    }

    private void BindServices()
    {
        Container.Bind<IInputService>().To<OldEditorInputService>().AsSingle();
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