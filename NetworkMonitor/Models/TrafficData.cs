namespace NetworkMonitor.Models;

public class TrafficData
{
    public int Id { get; set; }
    public string Port { get; set; }
    public long BytesReceived { get; set; }
    public long BytesSent { get; set; }
    public DateTime Timestamp { get; set; }
}
