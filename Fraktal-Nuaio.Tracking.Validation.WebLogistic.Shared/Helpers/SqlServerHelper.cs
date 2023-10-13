using Dapper;
using Microsoft.Data.SqlClient;

namespace Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Helpers
{
    public static class SqlServerHelper
    {
        public async static Task<Guid?> IsUcidInBookingProcess(string connectionString, string ucid)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new
                {
                    ucid
                };
                var sql = "SELECT Id FROM ProcessDetail WHERE Ucid = @Ucid";

                var result = await connection.ExecuteScalarAsync<Guid?>(sql, parameters);
                return result;
            }
        }

        public async static Task<bool> InsertIntoUnparentedUcidsTable(string connectionString, string origin, string ucid, string requestJson, string jsonToQueue)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new
                {
                    Id = Guid.NewGuid(),
                    Origin = origin,
                    Ucid = ucid,
                    RequestJson = requestJson,
                    JsonToQueue = jsonToQueue,
                    GenerationDate = DateTime.Now
                };
                var sql = "INSERT INTO UnparentedUcids (Id,Origin,Ucid,RequestJson,JsonToQueue,GenerationDate) VALUES (@Id,@Origin,@Ucid,@RequestJson,@JsonToQueue,@GenerationDate)";

                return (await connection.ExecuteAsync(sql, parameters)) > 0;
            }
        }

        public async static Task<bool> InsertProcesDetailStatusHistoryTable(string connectionString, string origin, Guid processDetailId, bool isEvent, bool isStatus, int code, string status, DateTime date)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var parameters = new
                {
                    Id = Guid.NewGuid(),
                    Origin = origin,
                    ProcessDetailId = processDetailId,
                    IsEvent = isEvent,
                    IsStatus = isStatus,
                    Code = code,
                    Status = status,
                    Date = date,
                    GenerationDate = DateTime.Now
                };
                var sql = "INSERT INTO ProcesDetailStatusHistory (Id,Origin,ProcessDetailId,IsEvent,IsStatus,Code,Status,Date,GenerationDate) VALUES (@Id,@Origin,@ProcessDetailId,@IsEvent,@IsStatus,@Code,@Status,@Date,@GenerationDate)";

                return (await connection.ExecuteAsync(sql, parameters)) > 0;
            }
        }
    }
}
