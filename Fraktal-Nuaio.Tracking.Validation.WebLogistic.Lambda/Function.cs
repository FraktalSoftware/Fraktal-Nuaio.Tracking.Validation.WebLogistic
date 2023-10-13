using Amazon.Lambda.Core;
using FluentResults;
using FluentValidation.Results;
using Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Dtos;
using Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Helpers;
using Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Fraktal_Nuaio.Tracking.Validation.WebLogistic.Lambda;

public class Function
{
    private readonly List<string> _alloredOriginList;
    private readonly string _connectionString;
    private ILambdaContext _context;

    public Function()
    {
        _alloredOriginList = new() { "WL" };
        _connectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? throw new ArgumentException("Missing ConnectionString variable");

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<Result> FunctionHandler(string input, ILambdaContext context)
    {
        _context = context;

        var requets = JsonConvert.DeserializeObject<RequestDto<WebLogisticEventDto>>(input);
        if (requets == null)
            throw new ArgumentNullException("Request came in wrong format");

        if (!_alloredOriginList.Contains(requets.Origin))
            return Result.Fail($"[{requets.Origin}] is not a valid Origin");
        var eventType = requets.Event?.EventType ?? string.Empty;
        if (requets.Event == null || string.IsNullOrEmpty(eventType))
            return Result.Fail($"[{eventType}] is not a valid Event Type");

        var validatorResult = await DoValidations(requets);

        if (!validatorResult.IsValid)
        {
            var validationResult = string.Join(" - ", validatorResult.Errors.Select(x => x.ErrorMessage));
            _context.Logger.LogInformation($"FunctionHandler() :: There are some validations... {validationResult}");
            //TODO: Send to Fail Queue
            return Result.Fail(validationResult);
        }

        //TODO: Create json object to send to Queue
        var jsonToQueue = string.Empty;

        var processDetailId = await SqlServerHelper.IsUcidInBookingProcess(_connectionString, requets.Event.Ucid);
        if (!processDetailId.HasValue)
        {
            await SqlServerHelper.InsertIntoUnparentedUcidsTable(_connectionString, requets.Origin, requets.Event.Ucid, input, jsonToQueue);
            return Result.Fail($"{requets.Event.Ucid} is not present in any booking process.");
        }

        await SqlServerHelper.InsertProcesDetailStatusHistoryTable(_connectionString, requets.Origin, processDetailId.Value, false, true, requets.Event.StatusCode, requets.Event.StatusName, DateTime.Parse(requets.Event.StatusDate));

        //TODO: Send to Ok Queue if Booking of UCID is marked as NUAIO = TRUE

        return Result.Ok();
    }

    private async Task<ValidationResult> DoValidations(RequestDto<WebLogisticEventDto> request)
    {
        _context.Logger.LogInformation($"DoValidations() :: Start method...");
        var webLogisticEventValidator = new WebLogisticEventValidator();
        return await webLogisticEventValidator.ValidateAsync(request.Event);
    }
}
