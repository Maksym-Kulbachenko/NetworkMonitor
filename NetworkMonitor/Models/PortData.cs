namespace NetworkMonitor.Models;

public class PortData
{
    public string Protocol { get; set; }
    public string LocalAddress { get; set; }
    public int LocalPort { get; set; }
    public string ForeignAddress { get; set; }
    public int ForeignPort { get; set; }
    public string State { get; set; }
    public int PID { get; set; }
    public string ProcessName { get; set; }
}
