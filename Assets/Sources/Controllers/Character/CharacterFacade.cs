using System;

public class CharacterFacade
{
    private readonly CharacterConfig _config;
    private readonly Character _character;
    private readonly CharacterHead _head;
    private readonly ICharacterStateMachine _stateMachine;

    public CharacterFacade(CharacterConfig config, Character character, CharacterHead head, ICharacterStateMachine stateMachine)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _character = character ?? throw new ArgumentNullException(nameof(character));
        _head = head ?? throw new ArgumentNullException(nameof(head));
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
    }

    public CharacterConfig Config => _config;
    public Character Character => _character;
    public CharacterHead Head => _head;
    public ICharacterStateMachine StateMachine => _stateMachine;
}