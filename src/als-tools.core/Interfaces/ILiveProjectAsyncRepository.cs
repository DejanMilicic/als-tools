using AlsTools.Core.Entities;
using AlsTools.Core.Queries;

namespace AlsTools.Core.Interfaces;

public interface ILiveProjectAsyncRepository
{
    Task InsertAsync(LiveProject project);

    Task InsertAsync(IEnumerable<LiveProject> projects);

    Task<IReadOnlyList<LiveProject>> Search(QuerySpecification specification);

    Task<IReadOnlyList<LiveProject>> GetAllProjectsAsync();

    Task DeleteAllAsync();

    Task<int> CountProjectsAsync();
}
