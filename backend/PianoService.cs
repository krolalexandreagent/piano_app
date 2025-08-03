using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Service responsible for loading, caching and persisting piano locations.
/// Piano locations are stored in a simple CSV file on disk (pianos.csv) with the
/// following format per line: <id>,<latitude>,<longitude>,<name>.
/// The service exposes methods to retrieve all pianos, refresh the cache from disk
/// and add new pianos. It maintains an in-memory list of pianos to avoid reading
/// the file on every request.
/// </summary>
public class PianoService
{
    private readonly string _filePath = "pianos.csv";
    private readonly List<Piano> _pianos = new();

    public PianoService()
    {
        LoadPianos();
    }

    /// <summary>
    /// Loads piano data from the CSV file into the in-memory list. If the file does not
    /// exist no pianos will be loaded. This method clears the current cache before loading.
    /// </summary>
    private void LoadPianos()
    {
        _pianos.Clear();
        if (!File.Exists(_filePath))
        {
            return;
        }

        var lines = File.ReadAllLines(_filePath);
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            // Expecting exactly four parts: id, latitude, longitude, name
            if (parts.Length >= 4 &&
                int.TryParse(parts[0], out int id) &&
                double.TryParse(parts[1], out double latitude) &&
                double.TryParse(parts[2], out double longitude))
            {
                var name = string.Join(',', parts.Skip(3)); // In case name itself contains commas
                _pianos.Add(new Piano
                {
                    Id = id,
                    Latitude = latitude,
                    Longitude = longitude,
                    Name = name
                });
            }
        }
    }

    /// <summary>
    /// Writes the current in-memory list of pianos to the CSV file. Each line is composed
    /// of id, latitude, longitude and name separated by commas. Existing contents of the
    /// file will be overwritten.
    /// </summary>
    private void SavePianos()
    {
        var lines = _pianos.Select(p => $"{p.Id},{p.Latitude},{p.Longitude},{p.Name}");
        File.WriteAllLines(_filePath, lines);
    }

    /// <summary>
    /// Returns a copy of all pianos currently in the cache. A copy is returned so callers
    /// cannot inadvertently modify the internal list.
    /// </summary>
    public List<Piano> GetPianos()
    {
        return new List<Piano>(_pianos);
    }

    /// <summary>
    /// Reloads the piano cache from disk. Any unsaved additions will be lost. This can be
    /// called to refresh the state after manual edits to the CSV or other file system changes.
    /// </summary>
    public void Refresh()
    {
        LoadPianos();
    }

    /// <summary>
    /// Adds a new piano to the cache and persists it to disk. An ID will be automatically
    /// assigned based on the current highest ID. If there are no existing pianos the ID
    /// will start at 1.
    /// </summary>
    /// <param name="latitude">Latitude coordinate of the piano.</param>
    /// <param name="longitude">Longitude coordinate of the piano.</param>
    /// <param name="name">Human friendly name for the piano.</param>
    public void AddPiano(double latitude, double longitude, string name)
    {
        var nextId = _pianos.Any() ? _pianos.Max(p => p.Id) + 1 : 1;
        var piano = new Piano
        {
            Id = nextId,
            Latitude = latitude,
            Longitude = longitude,
            Name = name
        };
        _pianos.Add(piano);
        SavePianos();
    }
}