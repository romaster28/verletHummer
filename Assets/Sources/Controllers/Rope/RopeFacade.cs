using System;

public class RopeFacade
{
    public IRopeService Service { get; }
    public RopeSwingConfig SwingConfig { get; }
    public RopeConfig Config { get; }
    public RopeThrower Thrower { get; }

    public RopeFacade(IRopeService service, RopeSwingConfig swingConfig, RopeConfig config, RopeThrower thrower)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        SwingConfig = swingConfig ?? throw new ArgumentNullException(nameof(swingConfig));
        Config = config ?? throw new ArgumentNullException(nameof(config));
        Thrower = thrower ?? throw new ArgumentNullException(nameof(thrower));
    }
}