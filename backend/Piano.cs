public class Piano
{
    /// <summary>
    /// A unique identifier for the piano. When new pianos are added the service
    /// will assign a monotonically increasing ID based on the existing values.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Latitude coordinate of the piano's location. Positive values are north of the equator
    /// and negative values are south.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate of the piano's location. Positive values are east of the prime meridian
    /// and negative values are west.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// A human friendly name for the piano. This could be the city, landmark or any descriptive label.
    /// </summary>
    public string Name { get; set; } = "";
}