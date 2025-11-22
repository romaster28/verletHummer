using System;

public class RopeDisconnected
{
    public Rope Rope { get; }

    public RopeDisconnected(Rope rope)
    {
        Rope = rope ?? throw new ArgumentNullException(nameof(rope));
    }
}