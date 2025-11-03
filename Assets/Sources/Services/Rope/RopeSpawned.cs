using System;
using UnityEngine;

public class RopeSpawned
{
    public Rope Rope { get; }
    public Vector3 Start { get; }
    public Vector3 End { get; }
    public bool Connected { get; }
    public Vector3 Direction => (End - Start).normalized;

    public RopeSpawned(Rope rope, Vector3 start, Vector3 end, bool connected)
    {
        Rope = rope ?? throw new ArgumentNullException(nameof(rope));
        Start = start;
        End = end;
        Connected = connected;
    }
}