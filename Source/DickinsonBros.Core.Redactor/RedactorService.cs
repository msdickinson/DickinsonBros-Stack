using DickinsonBros.Core.Redactor.Abstractions;
using DickinsonBros.Core.Redactor.Models;
using Microsoft.Extensions.Options;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DickinsonBros.Core.Redactor
{
    public class RedactorService : IRedactorService
    {
        internal const string REDACTED_REPLACEMENT_VALUE = "***REDACTED***";
        internal readonly HashSet<string> _propertiesToRedact;
        internal readonly List<Regex> _regexValuesToRedact;

        readonly static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public RedactorService(IOptions<RedactorServiceOptions> options)
        {
            _regexValuesToRedact = new List<Regex>
            (
                (options.Value.RegexValuesToRedact ?? new string[] { }).Select(valueToRedact => new Regex(valueToRedact))
            );

            _propertiesToRedact = new HashSet<string>
            (
                options.Value.PropertiesToRedact ?? new string[] { },
                StringComparer.OrdinalIgnoreCase
            );
        }

        public string Redact(object value)
        {
            if (value is string)
            {
                return Redact((string)value);
            }

            return Redact(JsonConvert.SerializeObject(value, _jsonSettings));
        }
        public string Redact(string json)
        {
            var jToken = Parse(json);

            if (jToken == null)
            {
                if (IsRedactedValue(json))
                {
                    return REDACTED_REPLACEMENT_VALUE;
                }

                return json;
            }

            return Redact(jToken).ToString();
        }
        internal JToken Redact(JToken node)
        {
            node?.ForEach(token => Redact(token));

            if (node is JValue jValue &&
                jValue.Type == JTokenType.String)
            {
                JToken embeddedJson = Parse(jValue.Value.ToString());

                if (embeddedJson != null)
                {
                    Redact(embeddedJson);
                    jValue.Value = embeddedJson.ToString();
                }

                else if (IsRedactedValue(jValue))
                {
                    jValue.Value = REDACTED_REPLACEMENT_VALUE;
                }
            }

            else if (node is JProperty property &&
                     IsRedactedValue(property))
            {
                property.Value = new JValue(REDACTED_REPLACEMENT_VALUE);
            };

            return node;
        }

        internal bool IsRedactedValue(string value)
        {
            return _regexValuesToRedact.Any(valueToRedact => valueToRedact.IsMatch(value));
        }
        internal bool IsRedactedValue(JValue jValue)
        {
            return _regexValuesToRedact.Any(valueToRedact => valueToRedact.IsMatch(jValue.Value.ToString()));
        }
        internal bool IsRedactedValue(JProperty property)
        {
            return _propertiesToRedact.Contains(property.Name);
        }

        internal JToken Parse(string value)
        {
            try
            {
                var reducedValue = value?.Trim();
                if
                (
                    reducedValue != null &&
                    (reducedValue.First() == '{' && reducedValue.Last() == '}') ||
                    (reducedValue.First() == '[' && reducedValue.Last() == ']') ||
                    string.Equals("null", value, StringComparison.OrdinalIgnoreCase)
                )
                {
                    return JToken.Parse(value);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
