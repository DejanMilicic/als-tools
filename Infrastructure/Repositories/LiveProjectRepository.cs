using System;
using System.Collections.Generic;
using System.Linq;
using AlsTools.Core.Entities;
using AlsTools.Core.Interfaces;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace AlsTools.Infrastructure.Repositories
{
    public class LiveProjectRepository : ILiveProjectRepository
    {
        private readonly LiteDatabase liteDb;
        private readonly ILogger<LiveProjectRepository> logger;
        private readonly ILiteDbContext dbContext;

        public LiveProjectRepository(ILogger<LiveProjectRepository> logger, ILiteDbContext dbContext)
        {
            this.liteDb = dbContext.Database;
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public int CountProjects()
        {
            return liteDb.GetCollection<LiveProject>("LiveProject").Count();
        }

        public void DeleteAll()
        {
            var deletedCount = liteDb.GetCollection<LiveProject>("LiveProject").DeleteAll();
            logger.LogDebug("Deleted {DeletedCount} projects", deletedCount);
        }

        public IEnumerable<LiveProject> GetAllProjects()
        {
            return liteDb.GetCollection<LiveProject>("LiveProject").FindAll();
        }

        public IEnumerable<LiveProject> GetProjectsContainingPlugins(string[] pluginsToLocate)
        {
            //TODO: implement it correctly, using DB query
            var projects = GetAllProjects();
            IList<LiveProject> res = new List<LiveProject>();
            foreach (var proj in projects)
            {
                if (proj.Tracks.Any(track => track.Plugins.Any(plugin => pluginsToLocate.Any(plugToLocate => plugin.Key.Contains(plugToLocate, StringComparison.InvariantCultureIgnoreCase)))))
                    res.Add(proj);
            }

            return res.AsEnumerable();
        }

        public bool Insert(LiveProject project)
        {
            var col = liteDb.GetCollection<LiveProject>("LiveProject");
            var res = col.Insert(project);

            // Create an index over the Name property (if it doesn't exist)
            col.EnsureIndex(x => x.Name);

            logger.LogDebug("Insert result {Result}", res);

            return res;
        }

        public int Insert(IEnumerable<LiveProject> projects)
        {
            var col = liteDb.GetCollection<LiveProject>("LiveProject");
            var res = col.InsertBulk(projects);

            // Create an index over the Name property (if it doesn't exist)
            col.EnsureIndex(x => x.Name);

            logger.LogDebug("Inserted {InsertedProjects} projects", res);

            return res;
        }
    }
}
