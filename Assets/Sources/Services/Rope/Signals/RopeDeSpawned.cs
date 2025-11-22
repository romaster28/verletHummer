using System;

public class RopeDeSpawned
{
    public Rope Rope { get; }

    public RopeDeSpawned(Rope rope)
    {
        Rope = rope ?? throw new ArgumentNullException(nameof(rope));
    }
}