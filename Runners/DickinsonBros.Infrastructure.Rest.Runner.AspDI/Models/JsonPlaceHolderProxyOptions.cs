namespace DickinsonBros.Infrastructure.Rest.Runner.AspDI.Services.JsonPlaceHolderProxy.Models
{
    public class JsonPlaceHolderProxyOptions
    {
        public string BaseURL { get; set; }
        public double GetTodosTimeoutInSeconds { get; set; }
        public int GetTodosRetrys { get; set; }
        public string GetTodosResource { get; set; }
    }
}