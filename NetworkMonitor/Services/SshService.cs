using NetworkMonitor.Configurations;
using NetworkMonitor.Models;
using Renci.SshNet;
using System.Text.RegularExpressions;

namespace NetworkMonitor.Services;

public class SshService
{
    private readonly SshConfiguration _config;

    public SshService(SshConfiguration config)
    {
        _config = config;
    }

    public NetworkStatistics GetOverallTrafficData()
    {
        var networkStatistics = new NetworkStatistics();

        using var client = new SshClient(_config.Host, _config.Username, _config.Password);
        client.Connect();

        var command = client.CreateCommand("netstat -e");
        var result = command.Execute();

        client.Disconnect();

        ParseNetworkStatistics(result, networkStatistics);
        return networkStatistics;
    }

    private void ParseNetworkStatistics(string data, NetworkStatistics networkStatistics)
    {
        var regex = new Regex(@"Bytes\s+(\d+)\s+(\d+)\s+Unicast packets\s+(\d+)\s+(\d+)\s+Non-unicast packets\s+(\d+)\s+(\d+)\s+Discards\s+(\d+)\s+(\d+)\s+Errors\s+(\d+)\s+(\d+)\s+Unknown protocols\s+(\d+)");

        var match = regex.Match(data);

        if (match.Success)
        {
            networkStatistics.BytesReceived = long.Parse(match.Groups[1].Value);
            networkStatistics.BytesSent = long.Parse(match.Groups[2].Value);
            networkStatistics.UnicastPacketsReceived = int.Parse(match.Groups[3].Value);
            networkStatistics.UnicastPacketsSent = int.Parse(match.Groups[4].Value);
            networkStatistics.NonUnicastPacketsReceived = int.Parse(match.Groups[5].Value);
            networkStatistics.NonUnicastPacketsSent = int.Parse(match.Groups[6].Value);
            networkStatistics.DiscardsReceived = int.Parse(match.Groups[7].Value);
            networkStatistics.DiscardsSent = int.Parse(match.Groups[8].Value);
            networkStatistics.ErrorsReceived = int.Parse(match.Groups[9].Value);
            networkStatistics.ErrorsSent = int.Parse(match.Groups[10].Value);
            networkStatistics.UnknownProtocolsReceived = int.Parse(match.Groups[11].Value);
        }
        else
        {
            throw new FormatException("Unable to parse network statistics data.");
        }
    }

    public IEnumerable<PortData> GetPortData()
    {
        var portDataList = new List<PortData>();

        using var client = new SshClient(_config.Host, _config.Username, _config.Password);
        client.Connect();

        var command = client.CreateCommand("netstat -anob");
        var result = command.Execute();

        client.Disconnect();

        portDataList = ParsePortData(result);

        return portDataList;
    }

    private List<PortData> ParsePortData(string data)
    {
        var portDataList = new List<PortData>();

        var regex = new Regex(@"(?<protocol>TCP|UDP)\s+(?<localAddress>[\d.:]+):(?<localPort>\d+)\s+(?<foreignAddress>[\d.:]+):(?<foreignPort>\d+)\s+(?<state>\S+)\s+(?<pid>\d+)\s+(?<processName>\[.*?\])?");

        var matches = regex.Matches(data);

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                portDataList.Add(new PortData
                {
                    Protocol = match.Groups["protocol"].Value,
                    LocalAddress = match.Groups["localAddress"].Value,
                    LocalPort = int.Parse(match.Groups["localPort"].Value),
                    ForeignAddress = match.Groups["foreignAddress"].Value,
                    ForeignPort = int.Parse(match.Groups["foreignPort"].Value),
                    State = match.Groups["state"].Value,
                    PID = match.Groups["pid"].Success ? int.Parse(match.Groups["pid"].Value) : -1,
                    ProcessName = match.Groups["processName"].Success ? match.Groups["processName"].Value.Trim('[', ']') : ""
                });
            }
        }

        return portDataList;
    }

    public List<ConnectedDevice> GetConnectedDevices()
    {
        var connectedDevicesList = new List<ConnectedDevice>();

        using var client = new SshClient(_config.Host, _config.Username, _config.Password);
        client.Connect();

        var command = client.CreateCommand("arp -a");
        var result = command.Execute();

        client.Disconnect();

        connectedDevicesList = ParseConnectedDevices(result);

        return connectedDevicesList;
    }

    private List<ConnectedDevice> ParseConnectedDevices(string data)
    {
        var connectedDevicesList = new List<ConnectedDevice>();

        var regex = new Regex(@"(?<ip>(?:\d{1,3}\.){3}\d{1,3})\s+(?<mac>(?:[0-9a-fA-F]{2}-){5}[0-9a-fA-F]{2})\s+(?<type>\S+)");

        var matches = regex.Matches(data);

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                connectedDevicesList.Add(new ConnectedDevice
                {
                    IpAddress = match.Groups["ip"].Value,
                    MacAddress = match.Groups["mac"].Value,
                    Type = match.Groups["type"].Value
                });
            }
        }

        return connectedDevicesList;
    }
}
