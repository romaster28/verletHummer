using System;
using UniRx;
using Object = UnityEngine.Object;

public class CharacterPresenter : IHandler<StartTick>, IHandler<DestroyTick>
{
    private const string ViewPath = "Level/CharacterView";
    
    private readonly Character _character;
    private readonly IAssetProvider _assetProvider;
    private readonly ICameraService _camera;
    private CharacterView _view;
    private CharacterView _prefab;

    public CharacterPresenter(Character character, IAssetProvider assetProvider, ICameraService camera)
    {
        _character = character ?? throw  new ArgumentNullException(nameof(character));
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
    }

    public void Handle(StartTick eventData)
    {
        _prefab = _assetProvider.LoadAsset<CharacterView>(ViewPath);
        _view = Object.Instantiate(_prefab);
        
        _character.Position.Subscribe(_view.UpdatePosition).AddTo(_view);
        _character.Rotation.Subscribe(_view.UpdateRotation).AddTo(_view);
        _character.Scale.Subscribe(_view.UpdateScale).AddTo(_view);
        
        _camera.StartFollow(_view.transform);
    }

    public void Handle(DestroyTick eventData)
    {
        _assetProvider.UnloadAsset(_prefab);
    }
}