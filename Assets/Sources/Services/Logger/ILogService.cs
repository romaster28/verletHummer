using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ILogService
{
    void Log(LogKey log, string message);
}

public class StubLogService : ILogService
{
    public void Log(LogKey log, string message)
    {
        
    }
}

public class DebugLogService : ILogService
{
    private const string Separator = " ";
    
    private readonly Dictionary<LogKey, LogData> _base = new Dictionary<LogKey, LogData>()
    {
        { LogKey.Character, new LogData("Character", Color.green) }
    };
    
    public void Log(LogKey key, string message)
    {
        var data = _base[key];
        Debug.Log($"<color=#{data.Color.ToHexString()}>({data.Title}){Separator}</color>{message}");
    }

    public class LogData
    {
        public string Title { get; }
        public Color Color { get; }

        public LogData(string title, Color color)
        {
            Title = title;
            Color = color;
        }
    }
}