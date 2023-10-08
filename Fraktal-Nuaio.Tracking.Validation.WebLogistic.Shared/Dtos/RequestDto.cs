namespace Fraktal_Nuaio.Tracking.Validation.WebLogistic.Shared.Dtos
{
    public class RequestDto<T>
    {
        public string Origin { get; set; } = null!;
        public T Event { get; set; } = default;
    }
}
