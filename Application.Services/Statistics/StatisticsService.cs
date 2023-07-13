using Application.Data;
using Application.Services.Interfaces.Statistics;
using Application.Services.Models.Statistics;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public StatisticsService(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<StatisticsServiceModel> TotalStatisticsAsync()
        {
            return new StatisticsServiceModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalProjects = await _context.Projects.CountAsync(),
                TotalArticles = await _context.Articles
                .Where(a => a.IsDeleted == false)
                .CountAsync()
            };
        }
    }
}
