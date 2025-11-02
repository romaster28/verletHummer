using System.Linq;
using UniRx;

public interface ISettingsService
{
    T Read<T>() where T : ISettingValue;

    public interface ISettingValue
    {
    }
}

public class SettingsService : ISettingsService
{
    private readonly ISettingsService.ISettingValue[] _values;

    public SettingsService()
    {
        _values = new ISettingsService.ISettingValue[]
        {
            new SensitivityValue(3),
            new MouseSmoothValue(0),
        };
    }

    public T Read<T>() where T : ISettingsService.ISettingValue
    {
        return (T)_values.First(x => x is T);
    }
}

public class BaseSettingHandler<T> : ISettingsService.ISettingValue
{
    private readonly ReactiveProperty<T> _property;

    public IReadOnlyReactiveProperty<T> Property => _property;

    public BaseSettingHandler(T startValue)
    {
        _property = new ReactiveProperty<T>(startValue);
    }
}