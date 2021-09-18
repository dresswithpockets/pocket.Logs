using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pocket.Logs.Core.Data;

namespace pocket.Logs.Api.Controllers
{
    [ApiController]
    [Route("logs")]
    public class LogsController
    {
        private readonly LogsContext _logsContext;

        public LogsController(LogsContext logsContext) => _logsContext = logsContext;

        [HttpGet("/records/{logId:int}")]
        public async Task<IActionResult> GetLogRecord(int logId)
        {
            var log = await _logsContext.RetrievedLogs.FirstOrDefaultAsync(l => l.LogId == logId);

            if (log == null)
                return new NotFoundResult();

            return new JsonResult(log);
        }

        [HttpGet("/records")]
        public async Task<IActionResult> GetLogRecords(int? from = null, [Range(0, 1000)] int limit = 1000,
            SortOrder sortOrder = SortOrder.Descending)
        {
            var query = _logsContext.RetrievedLogs.AsNoTracking();

            query = sortOrder switch
            {
                SortOrder.Ascending when from == null => query.OrderBy(q => q.LogId),
                SortOrder.Ascending => query.Where(q => q.LogId < from).OrderBy(q => q.LogId),

                SortOrder.Unspecified or SortOrder.Descending when from == null =>
                    query.OrderByDescending(q => q.LogId),
                SortOrder.Unspecified or SortOrder.Descending => query.Where(q => q.LogId > from)
                    .OrderByDescending(q => q.LogId),
                _ => throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null)
            };

            var logs = await query.Take(limit).ToListAsync();

            return new JsonResult(logs);
        }
    }
}