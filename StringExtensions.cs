using System;
using System.Linq;
using System.Text;

namespace EntityManager
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new Exception("Null or empty input");
            }

            var inputArray = input.ToCharArray();
            inputArray[0] = char.ToUpper(inputArray[0]);

            return new string(inputArray);
        }

        public static string FirstCharToLower(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new Exception("Null or empty input");
            }

            var inputArray = input.ToCharArray();
            inputArray[0] = char.ToLower(inputArray[0]);

            return new string(inputArray);
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
                return input.FirstCharToLower();
            }

            var words = input.Split("_", StringSplitOptions.RemoveEmptyEntries);
            var stringBuilder = new StringBuilder();

            foreach (string word in words)
            {
                stringBuilder.Append(word.Substring(0, 1).ToUpper());
                stringBuilder.Append(word.Substring(1));
            }

            return stringBuilder.ToString().FirstCharToLower();
        }

        public static string Indent(this string input, int level = 0) =>  input.Insert(0, new string(' ', level * 4));
    }
}
