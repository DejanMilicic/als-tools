using AlsTools.Core.ValueObjects;

namespace AlsTools.Ui.Cli.CliOptions;

[Verb("locate", HelpText = "Locates projects containing given plugins by their names.")]
public class LocateOptions : CommonOptions
{
    [Option("plugin-names", Group = "locate options", Min = 1, HelpText = "The plugin names to locate projects by.")]
    public IReadOnlyCollection<string>? PluginNamesToLocate { get; set; }

    [Option("plugin-formats", Group = "locate options", Min = 1, HelpText = "The plugin formats to locate projects by.")]
    public IReadOnlyCollection<PluginFormat>? PluginFormatsToLocate { get; set; }

    public override bool IsEmpty => (PluginNamesToLocate == null || !PluginNamesToLocate.Any()) &&
                                    (PluginFormatsToLocate == null || !PluginFormatsToLocate.Any());
}