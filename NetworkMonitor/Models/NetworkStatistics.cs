namespace NetworkMonitor.Models;

public class NetworkStatistics
{

    public int Id { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public int UnicastPacketsReceived { get; set; }
    public int UnicastPacketsSent { get; set; }
    public int NonUnicastPacketsReceived { get; set; }
    public int NonUnicastPacketsSent { get; set; }
    public int DiscardsReceived { get; set; }
    public int DiscardsSent { get; set; }
    public int ErrorsReceived { get; set; }
    public int ErrorsSent { get; set; }
    public int UnknownProtocolsReceived { get; set; }

    public DateTime Timestamp { get; set; }
}
