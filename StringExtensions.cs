using System;
using System.Linq;

namespace EntityManager
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.First().ToString().ToUpper() + input.Substring(1);
            }
            return string.Empty;
        }

        public static string FirstCharToLower(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.First().ToString().ToLower() + input.Substring(1);
            }
            return string.Empty;
        }

        public static string DropSuffix(this string input, string suffix)
        {
            var lcaseInput = input.ToLower();
            var lcaseSuffix = suffix.ToLower();

            if (lcaseInput.EndsWith(lcaseSuffix))
            {
                return input.Remove(lcaseInput.Length - lcaseSuffix.Length);
            }
            return input;
        }

        public static string ToSnakeCase(this string input)
        {
            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
        
        public static string SnakeCaseToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (!input.Contains("_"))
            {
                return input;
            }

            string[] words = input.Split("_", StringSplitOptions.RemoveEmptyEntries);
            string result = string.Empty;

            foreach (string word in words)
            {
                result += word.Substring(0, 1).ToUpper() + word.Substring(1);
            }

            return result;
        }
    }
}
