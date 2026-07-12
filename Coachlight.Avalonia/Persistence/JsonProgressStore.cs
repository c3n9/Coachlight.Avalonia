using System.Reflection;
using System.Text.Json;

namespace Coachlight.Avalonia.Persistence;

public class JsonProgressStore : IProgressStore
{
    private readonly string _filePath;
    private readonly object _lock = new();
    private HashSet<string> _completed;

    public JsonProgressStore(string? filePath = null)
    {
        _filePath = filePath ?? DefaultPath();
        _completed = Load();
    }

    public bool IsCompleted(string tourId)
    {
        lock (_lock)
            return _completed.Contains(tourId);
    }

    public void MarkCompleted(string tourId)
    {
        lock (_lock)
        {
            _completed.Add(tourId);
            Save();
        }
    }

    public void Reset(string tourId)
    {
        lock (_lock)
        {
            _completed.Remove(tourId);
            Save();
        }
    }

    private static string DefaultPath()
    {
        var app = Assembly.GetEntryAssembly()?.GetName().Name ?? "App";
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            app, "coachlight");
        return Path.Combine(dir, "progress.json");
    }

    private HashSet<string>? Load()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return new HashSet<string>(StringComparer.Ordinal);
            }

            var json = File.ReadAllText(_filePath);
            var ids = JsonSerializer.Deserialize<List<string>>(json);
            return ids is null
                ? new HashSet<string>(StringComparer.Ordinal)
                : new HashSet<string>(ids, StringComparer.Ordinal);
        }
        catch
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }
    }

    private void Save()
    {
        try
        {
            var directoryName = Path.GetDirectoryName(_filePath);
            if (string.IsNullOrWhiteSpace(directoryName))
            {
                Directory.CreateDirectory(DefaultPath());
            }

            File.WriteAllText(_filePath, JsonSerializer.Serialize(new List<string>(_completed)));
        }
        catch
        {
            // TODO
        }
    }
}