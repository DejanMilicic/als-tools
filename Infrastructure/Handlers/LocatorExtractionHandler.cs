using System.Collections.Generic;
using System.Xml.XPath;
using AlsTools.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace AlsTools.Infrastructure.Handlers;

public class LocatorExtractionHandler : ILocatorExtractionHandler
{
    private readonly ILogger<LocatorExtractionHandler> logger;

    public LocatorExtractionHandler(ILogger<LocatorExtractionHandler> logger)
    {
        this.logger = logger;
    }

    public IReadOnlyList<Locator> ExtractFromXml(XPathNavigator nav)
    {
        logger.LogDebug("Extracting Locators from XML...");

        var expression = $"/Ableton/LiveSet/Locators/Locators/Locator";
        var locatorsIterator = nav.Select(expression);
        var locators = new List<Locator>();

        foreach (XPathNavigator locatorNode in locatorsIterator)
        {
            var locator = new Locator()
            {
                Number = locatorNode.SelectSingleNode(@"@Id")?.ValueAsInt,
                Name = locatorNode.SelectSingleNode(@"Name/@Value")?.Value,
                Annotation = locatorNode.SelectSingleNode(@"Annotation/@Value")?.Value,
                Time = locatorNode.SelectSingleNode(@"Time/@Value")?.ValueAsInt,
                IsSongStart = locatorNode.SelectSingleNode(@"IsSongStart/@Value")?.ValueAsBoolean
            };

            locators.Add(locator);
        }

        return locators;
    }
}