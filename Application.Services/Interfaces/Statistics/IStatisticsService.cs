
namespace Application.Services.Interfaces.Statistics;

using Application.Services.Models.Statistics;

public interface IStatisticsService
{
    Task<StatisticsServiceModel> TotalStatisticsAsync();
}
