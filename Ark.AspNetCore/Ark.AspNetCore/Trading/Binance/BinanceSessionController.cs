using System;
using System.Linq;
using System.Threading.Tasks;
using Ark.Api.Binance;
using Ark.App.Diagnostics;
using Ark.AspNetCore;
using Ark.Data;
using Ark.Net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;


namespace Ark.AspNetCore
{
    /// <summary>
    /// Controller exposing operations to manage Binance client sessions.
    /// </summary>

    [ApiController]
    [ApiExplorerSettings(GroupName = "ο Binance")]
    public class BinanceSessionControllerBase<TUserProfileData> : ControllerBase<TUserProfileData>
            where TUserProfileData : new()
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Ark.AspNetCore.BinanceSessionControllerBase{TUserProfileData}"/> class.
        /// </summary>
        public BinanceSessionControllerBase()
        {
        }

        #endregion Constructors

        /// <summary>
        /// Creates a new Binance client session.
        /// </summary>
        /// <param name="options">Options used to configure the client.</param>
        /// <returns>The identifier of the created session.</returns>
        /// <remarks>
        /// ## Example ##
        /// ```json
        /// POST api/binance/sessions
        /// ```
        /// </remarks>
        /// <response code="200">Success - session created.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpPost("binance/sessions")]
        #region Methods (Public)

        public ResultDto<Guid> CreateSession([FromBody] BinanceOptions options)
            => ExecuteBl(() => SessionBusiness.CreateSession(options));

        /// <summary>
        /// Gets the options used when the session was created.
        /// </summary>
        /// <param name="id">Identifier of the session.</param>
        /// <returns>The stored options or NotFound if the session does not exist.</returns>
        /// <remarks>
        /// ## Example ##
        /// ```
        /// GET api/binance/sessions/{id}
        /// ```
        /// </remarks>
        /// <response code="200">Success - session options returned.</response>
        /// <response code="404">Session not found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpGet("binance/sessions/{id}")]
        public ResultDto<BinanceOptions> GetSessionOptions(Guid id)
            => ExecuteBl(() => SessionBusiness.GetSessionOptions(id));

        /// <summary>
        /// Retrieves aggregated session data including orders, positions
        /// and ticker history for the given time window.
        /// </summary>
        /// <param name="id">Session identifier.</param>
        /// <param name="window">Time window used to filter data.</param>
        /// <remarks>
        /// ## Example ##
        /// ```
        /// GET api/binance/sessions/{id}/overview?window=1.00:00:00
        /// ```
        /// </remarks>
        /// <response code="200">Success - session overview returned.</response>
        /// <response code="404">Session not found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpGet("binance/sessions/{id}/overview")]
        public Task<ResultDto<SessionOverviewDto>> GetSessionOverview(Guid id, [FromQuery] TimeWindow window)
                => ExecuteBlAsync(() => SessionBusiness.GetSessionOverviewAsync(id, window));

        /// <summary>
        /// Simple ping endpoint used for health checks.
        /// </summary>
        /// <returns>"pong" if the service is reachable.</returns>
        /// <remarks>
        /// ## Example ##
        /// ```
        /// GET api/binance/ping
        /// ```
        /// </remarks>
        /// <response code="200">Service reachable.</response>
        [HttpGet("binance/ping")]
        public ResultDto<string> Ping()
            => ExecuteBl(() => SessionBusiness.Ping());

        #endregion Methods (Public)
    }
}
