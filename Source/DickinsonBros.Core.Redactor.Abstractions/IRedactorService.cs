namespace DickinsonBros.Core.Redactor.Abstractions
{
    public interface IRedactorService
    {
        string Redact(object value);
        string Redact(string json);
    }
}
