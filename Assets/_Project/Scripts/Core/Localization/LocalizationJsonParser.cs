using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TWR.Localization
{
    /// <summary>
    /// Parses a top-level JSON object containing string keys and string values.
    /// </summary>
    public static class LocalizationJsonParser
    {
        /// <summary>
        /// Parses JSON text into a dictionary of localization entries.
        /// </summary>
        public static bool TryParseStringMap(string json, out Dictionary<string, string> map, out string error)
        {
            map = new Dictionary<string, string>(StringComparer.Ordinal);
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(json))
            {
                error = "JSON content is empty.";
                return false;
            }

            int index = 0;
            SkipWhitespace(json, ref index);

            if (!Consume(json, ref index, '{'))
            {
                error = "Expected '{' at JSON root.";
                return false;
            }

            SkipWhitespace(json, ref index);
            if (TryConsume(json, ref index, '}'))
                return true;

            while (index < json.Length)
            {
                if (!TryParseString(json, ref index, out var key, out error))
                    return false;

                SkipWhitespace(json, ref index);
                if (!Consume(json, ref index, ':'))
                {
                    error = "Expected ':' after key.";
                    return false;
                }

                SkipWhitespace(json, ref index);
                if (!TryParseString(json, ref index, out var value, out error))
                    return false;

                map[key] = value;

                SkipWhitespace(json, ref index);
                if (TryConsume(json, ref index, ','))
                {
                    SkipWhitespace(json, ref index);
                    continue;
                }

                if (TryConsume(json, ref index, '}'))
                {
                    SkipWhitespace(json, ref index);
                    if (index != json.Length)
                    {
                        error = "Unexpected trailing content after root object.";
                        return false;
                    }

                    return true;
                }

                error = "Expected ',' or '}' after value.";
                return false;
            }

            error = "Unexpected end of JSON content.";
            return false;
        }

        static bool TryParseString(string json, ref int index, out string value, out string error)
        {
            value = string.Empty;
            error = string.Empty;

            if (!Consume(json, ref index, '"'))
            {
                error = "Expected string token opening quote '" + '"' + "'.";
                return false;
            }

            var builder = new StringBuilder();
            while (index < json.Length)
            {
                char c = json[index++];
                if (c == '"')
                {
                    value = builder.ToString();
                    return true;
                }

                if (c != '\\')
                {
                    builder.Append(c);
                    continue;
                }

                if (index >= json.Length)
                {
                    error = "Invalid escape sequence at end of string.";
                    return false;
                }

                char escaped = json[index++];
                switch (escaped)
                {
                    case '"': builder.Append('"'); break;
                    case '\\': builder.Append('\\'); break;
                    case '/': builder.Append('/'); break;
                    case 'b': builder.Append('\b'); break;
                    case 'f': builder.Append('\f'); break;
                    case 'n': builder.Append('\n'); break;
                    case 'r': builder.Append('\r'); break;
                    case 't': builder.Append('\t'); break;
                    case 'u':
                    {
                        if (index + 4 > json.Length)
                        {
                            error = "Invalid unicode escape sequence length.";
                            return false;
                        }

                        string hex = json.Substring(index, 4);
                        if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var unicodeValue))
                        {
                            error = "Invalid unicode escape sequence value.";
                            return false;
                        }

                        builder.Append((char)unicodeValue);
                        index += 4;
                        break;
                    }
                    default:
                        error = $"Unsupported escape sequence '\\{escaped}'.";
                        return false;
                }
            }

            error = "Unterminated JSON string token.";
            return false;
        }

        static void SkipWhitespace(string json, ref int index)
        {
            while (index < json.Length)
            {
                char c = json[index];
                if (!char.IsWhiteSpace(c)) return;
                index++;
            }
        }

        static bool Consume(string json, ref int index, char token)
        {
            if (index >= json.Length || json[index] != token)
                return false;
            index++;
            return true;
        }

        static bool TryConsume(string json, ref int index, char token)
        {
            if (index >= json.Length || json[index] != token)
                return false;
            index++;
            return true;
        }
    }
}
