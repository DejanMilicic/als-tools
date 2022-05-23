using System.Xml.XPath;
using AlsTools.Core.ValueObjects.Devices;
using Microsoft.Extensions.Logging;

namespace AlsTools.Infrastructure.Extractors.StockDevices;

public abstract class BaseStockDeviceExtractor : IStockDeviceExtractor
{
    private readonly ILogger<BaseStockDeviceExtractor> logger;

    public DeviceSort DeviceSort { get; protected set; }

    public BaseStockDeviceExtractor(ILogger<BaseStockDeviceExtractor> logger, DeviceSort sort)
    {
        this.logger = logger;
        this.DeviceSort = sort;
    }

    public virtual IDevice ExtractFromXml(XPathNavigator deviceNode)
    {
        logger.LogDebug("Extracting Live Stock {DeviceSort} device from XML...", DeviceSort);

        var device = new LiveDevice(DeviceSort);

        device.Name = deviceNode.Name;

        logger.LogDebug("Device name: {DeviceName}", device.Name);

        device.UserName = deviceNode.SelectSingleNode(@"UserName/@Value")?.Value;
        device.Annotation = deviceNode.SelectSingleNode(@"Annotation/@Value")?.Value;
        device.Id = deviceNode.SelectSingleNode(@"@Id").ValueAsInt;

        return device;
    }
}