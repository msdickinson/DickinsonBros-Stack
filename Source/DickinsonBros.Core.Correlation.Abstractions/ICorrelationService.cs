namespace DickinsonBros.Core.Correlation.Abstractions
{
    public interface ICorrelationService
    {
        string CorrelationId { get; set; }
    }
}
