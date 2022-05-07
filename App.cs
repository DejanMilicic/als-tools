using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlsTools.CliOptions;
using AlsTools.Core.Entities;
using AlsTools.Core.Interfaces;
using AlsTools.Exceptions;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace AlsTools
{
    public class App
    {
        private readonly ILogger<App> logger;
        private readonly ILiveProjectAsyncService liveProjectService;

        public App(ILogger<App> logger, ILiveProjectAsyncService liveProjectService)
        {
            this.logger = logger;
            this.liveProjectService = liveProjectService;
        }

        public async Task Run(ParserResult<object> parserResult)
        {
            logger.LogDebug("App start");

            await parserResult.WithParsedAsync<InitDbOptions>(options => RunInitDb(options));
            await parserResult.WithParsedAsync<CountOptions>(options => RunCount(options));
            await parserResult.WithParsedAsync<ListOptions>(options => RunList(options));
            await parserResult.WithParsedAsync<LocateOptions>(options => RunLocate(options));
            await parserResult.WithNotParsedAsync(errors => { throw new CommandLineParseException(errors); });
        }

        private async Task RunInitDb(InitDbOptions options)
        {
            int count = 0;
            if (options.Files.Any())
                count = await liveProjectService.InitializeDbFromFilesAsync(options.Files);
            else
                count = await liveProjectService.InitializeDbFromFoldersAsync(options.Folders, options.IncludeBackups);

            await Console.Out.WriteLineAsync($"\nTotal of projects loaded into DB: {count}");
        }

        private async Task RunCount(CountOptions options)
        {
            int count = await liveProjectService.CountProjectsAsync();

            await Console.Out.WriteLineAsync($"\nTotal of projects in the DB: {count}");
        }

        private async Task RunList(ListOptions options)
        {
            var projects = (await liveProjectService.GetAllProjectsAsync()).ToList();
            await PrintProjectsAndPlugins(projects);
            await Console.Out.WriteLineAsync($"\nTotal of projects: {projects.Count}");
        }

        private async Task RunLocate(LocateOptions options)
        {
            var projects = (await liveProjectService.GetProjectsContainingPluginsAsync(options.PluginsToLocate)).ToList();
            await PrintProjectsAndPlugins(projects);
            await Console.Out.WriteLineAsync($"\nTotal of projects: {projects.Count}");
        }
    
        private async Task PrintProjectsAndPlugins(IEnumerable<LiveProject> projects)
        {
            foreach (var p in projects)
                await PrintProjectAndPlugins(p);
        }

        private async Task PrintProjectAndPlugins(LiveProject project)
        {
            await Console.Out.WriteLineAsync("------------------------------------------------------------------------------");
            await Console.Out.WriteLineAsync($"Project name: {project.Name}");
            await Console.Out.WriteLineAsync($"Live version (creator): {project.LiveVersion}");
            await Console.Out.WriteLineAsync($"Full path: {project.Path}");
            await Console.Out.WriteLineAsync("\tTracks and plugins:");

            if (project.Tracks.Count == 0)
                await Console.Out.WriteLineAsync("\t\tNo tracks found!");

            foreach (var tr in project.Tracks)
            {
                await Console.Out.WriteLineAsync($"\t\tName = {tr.Name} | Type = {tr.Type}");

                await Console.Out.WriteLineAsync("\t\t\tLive Devices:");
                foreach (var ld in tr.Devices)
                    await Console.Out.WriteLineAsync($"\t\t\t\tName = {ld.Key}");

                await Console.Out.WriteLineAsync("\t\t\tPlugins:");
                foreach (var p in tr.Plugins)
                    await Console.Out.WriteLineAsync($"\t\t\t\tName = {p.Key} | Type = {p.Value.PluginType}");
            }
        }
    }
}
