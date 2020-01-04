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
            if (input.EndsWith(suffix))
            {
                return input.Remove(input.Length - suffix.Length);
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

        public static string Indent(this string input, int level = 0)
        {
            return input.Insert(0,new string(' ', level * 4));
        }
    }
}
