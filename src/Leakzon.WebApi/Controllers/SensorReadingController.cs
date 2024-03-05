using AutoMapper;
using Leakzone.Backend.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Leakzone.Backend.Managers.Models;
using Leakzon.WebApi.ViewModels;

namespace Leakzon.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingController : ControllerBase
    {

        private readonly ILogger<SensorReadingController> _logger;
        private readonly IMapper _mapper;
        private readonly ISensorReadingManager _sensorManager;

        public SensorReadingController(ILogger<SensorReadingController> logger,
                                      IMapper mapper,
                                      ISensorReadingManager sensorManager)
        {
            _logger = logger;
            _mapper = mapper;
            _sensorManager = sensorManager;
        }

        [HttpPost]
        public async Task<IActionResult> SaveSensors([FromBody] List<JsonObject> sensor)
        {
            await _sensorManager.InsertSensorsReadingAsync(sensor);
            return NoContent();
        }

        [HttpGet("get-latest-reading/{sensorId}")]
        public async Task<IActionResult> GetLatestReading(string sensorId)
        {
            var result  = await _sensorManager.GetLatestReadingAsync(sensorId);
            return Ok(_mapper.Map<SensorInfoViewModel[]>(result));
        }

        [HttpGet("get-latest-reading")]
        public async Task<IActionResult> GetLatestReading()
        {
            var result = await _sensorManager.GetLatestReadingAsync(null);
            return Ok(_mapper.Map<SensorInfoViewModel[]>(result));
        }

        [HttpGet("get-oldest-reading/{sensorId}")]
        public async Task<IActionResult> GetOldestReading(string sensorId)
        {
            var result = await _sensorManager.GetOldestReading(sensorId);
            return Ok(_mapper.Map<SensorInfoViewModel[]>(result));
        }

        [HttpGet("get-oldest-reading")]
        public async Task<IActionResult> GetOldestReading()
        {
            var result = await _sensorManager.GetOldestReading(null);
            return Ok(_mapper.Map<SensorInfoViewModel[]>(result));
        }

        [HttpPost("get-hourly-consumption")]
        public async Task<IActionResult> HourlyConsumption([FromBody] HourlyConsumptionRequestViewModel request)
        {
            var result = await  _sensorManager.CalculateHourlyConsumptions(_mapper.Map<HourlyConsumptionRequest>(request));
            return Ok(_mapper.Map<SensorHourlyConsumptionViewModel[]>(result));
        }
    }
}
