using Amazon.Lambda.Core;
using FluentResults;
using FluentValidation.Results;
using Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Dtos;
using Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Fraktal_Nuaio.Tracking.Validation.WebLogistic.Lambda;

public class Function
{

    private readonly List<string> _alloredOriginList;
    private ILambdaContext _context;

    public Function()
    {
        _alloredOriginList = new() { "WL" };
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
        if (string.IsNullOrEmpty(requets.Event?.EventType ?? string.Empty))
            return Result.Fail($"[{requets.Event?.EventType ?? string.Empty}] is not a valid Origin");

        var validatorResult = await DoValidations(requets);

        if (!validatorResult.IsValid)
        {
            var validationResult = string.Join(" - ", validatorResult.Errors.Select(x => x.ErrorMessage));
            _context.Logger.LogInformation($"FunctionHandler() :: There are some validations... {validationResult}");
            //TODO: Send to Fail Queue
            return Result.Fail(validationResult);
        }

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
