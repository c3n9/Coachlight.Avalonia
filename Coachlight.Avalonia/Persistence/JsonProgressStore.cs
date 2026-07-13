using System.Reflection;
using System.Text.Json;

namespace Coachlight.Avalonia.Persistence;

/// <summary>
/// Default <see cref="IProgressStore"/> implementation: persists completed tour ids as JSON.
/// By default the file lives under the OS's application data folder, namespaced by the entry
/// assembly's name (so multiple applications using Coachlight don't collide) and by
/// "coachlight" (so it doesn't collide with the host application's own settings).
/// </summary>
public class JsonProgressStore : IProgressStore
{
    private readonly string _filePath;
    private readonly object _lock = new();
    private HashSet<string> _completed;

    /// <summary>Creates a store. Pass <paramref name="filePath"/> to override the default location.</summary>
    public JsonProgressStore(string? filePath = null)
    {
        _filePath = filePath ?? DefaultPath();
        _completed = Load();
    }

    /// <inheritdoc/>
    public bool IsCompleted(string tourId)
    {
        lock (_lock)
            return _completed.Contains(tourId);
    }

    /// <inheritdoc/>
    public void MarkCompleted(string tourId)
    {
        lock (_lock)
        {
            if (_completed.Add(tourId))
                Save();
        }
    }

    /// <inheritdoc/>
    public void Reset(string tourId)
    {
        lock (_lock)
        {
            if (_completed.Remove(tourId))
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

    private HashSet<string> Load()
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
            // A corrupt or unreadable file must not crash the host application.
            return new HashSet<string>(StringComparer.Ordinal);
        }
    }

    private void Save()
    {
        try
        {
            var directoryName = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(_filePath, JsonSerializer.Serialize(new List<string>(_completed)));
        }
        catch
        {
            // Not being able to persist progress must not crash the host application.
        }
    }
}
