using Application.Services.Interfaces.Statistics;
using Application.Services.Models.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.WebApi.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsApiController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsApiController(IStatisticsService statisticsService)
        {
            this._statisticsService = statisticsService;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(StatisticsServiceModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                StatisticsServiceModel serviceModel = await this._statisticsService.TotalStatisticsAsync();
                return this.Ok(serviceModel);
            }
            catch (Exception)
            {

                return this.BadRequest();
            }
        }
    }
}
