using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using RMotownFestival.Api.Data;
using RMotownFestival.DAL;
using RMotownFestival.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RMotownFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FestivalController : ControllerBase
    {
        private readonly MotownDbContext _context;
        private readonly TelemetryClient _telemetryClient;

        public FestivalController(MotownDbContext context, TelemetryClient telemetryClient)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _telemetryClient = telemetryClient ?? throw new System.ArgumentNullException(nameof(telemetryClient));
        }
        [HttpGet("LineUp")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Schedule))]
        public ActionResult GetLineUp()
        {
            return Ok(FestivalDataSource.Current.LineUp);
        }

        [HttpGet("Artists")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Artist>))]
        public ActionResult GetArtists(bool? withRatings)
        {
            if(withRatings.HasValue && withRatings.Value)
            {
                _telemetryClient.TrackEvent("List of artists with rating");
            }
            else
            {
                _telemetryClient.TrackEvent("List of artists without rating");
            }
            return Ok(_context.Artists.ToArray());
        }

        [HttpGet("Stages")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Stage>))]
        public ActionResult GetStages() => Ok(_context.Stages.ToArray());

        [HttpPost("Favorite")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ScheduleItem))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult SetAsFavorite(int id)
        {
            var schedule = FestivalDataSource.Current.LineUp.Items
                .FirstOrDefault(si => si.Id == id);
            if (schedule != null)
            {
                schedule.IsFavorite = !schedule.IsFavorite;
                return Ok(schedule);
            }
            return NotFound();
        }

    }
}