using Microsoft.AspNetCore.Mvc;
using NetworkMonitor.Models;
using NetworkMonitor.Persistence;
using NetworkMonitor.Services;

namespace NetworkMonitor.Controllers;

public class TrafficController : Controller
{
    private readonly TrafficDbContext _context;
    private readonly SshService _sshService;

    public TrafficController(TrafficDbContext context, SshService sshService)
    {
        _context = context;
        _sshService = sshService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetOverallTrafficData()
    {
        var data = _context.NetworkStatistics
            .OrderByDescending(x => x.Timestamp)
            .Take(1000)
            .ToList();

        return PartialView("OverallTrafficDataPartial", data);
    }

    public IActionResult GetPortData()
    {
        var portData = _sshService.GetPortData();
        return PartialView("PortDataPartial", portData);
    }

    public IActionResult GetConnectedDevices()
    {
        var connectedDevices = _sshService.GetConnectedDevices();
        return PartialView("ConnectedDevicesPartial", connectedDevices);
    }
}
