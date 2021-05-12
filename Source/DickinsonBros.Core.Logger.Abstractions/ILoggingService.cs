using DickinsonBros.Core.Logger.Abstractions.Models;
using System;
using System.Collections.Generic;

namespace DickinsonBros.Core.Logger.Abstractions
{
    public interface ILoggerService<out T>
    {
        void LogDebugRedacted(string message, LogGroup LogGroup, IDictionary<string, object> properties = null);
        void LogInformationRedacted(string message, LogGroup LogGroup, IDictionary<string, object> properties = null);
        void LogWarningRedacted(string messagee, LogGroup LogGroup, IDictionary<string, object> properties = null);
        void LogErrorRedacted(string message, LogGroup LogGroup, Exception exception, IDictionary<string, object> properties = null);
    }
}
