using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Core
{
    internal static class Extensions
    {

        public static IEnumerable<string> SplitOnWhitespace(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) yield return null;

            int len = input.Length;

            int totalRead = 0;

            while (totalRead < len)
            {
                var word = FindNextWord(input.AsSpan(totalRead), out int read);
                totalRead += read;

                if (word != null)
                    yield return word;
            }


        }

        private static string FindNextWord(ReadOnlySpan<char> src, out int read)
        {
            read = 0;

            if (src.IsEmpty) return null;

            int lastIndex = 0;
            bool inDoubleQuotes = false;
            bool inLiteral = false;
            bool exitedDoubleQuotes = false;
            bool exitedLiteral = false;

            for (int i = 0; i < src.Length; i++)
            {
                read++;

                if (src[i] == DoubleQuote)
                {
                    inDoubleQuotes = !inDoubleQuotes;
                    exitedDoubleQuotes = true;
                }
                else if (src[i] == SingleQuote)
                {
                    inLiteral = !inLiteral;
                    exitedLiteral = true;
                }

                if ((exitedDoubleQuotes || exitedLiteral) && !inDoubleQuotes && !inLiteral)
                {
                    var endOffset = exitedDoubleQuotes ? 1 : 2;

                    var substr = src.Substring(1, i - lastIndex - endOffset);

                    lastIndex = i + 1;

                    exitedLiteral = false;
                    exitedDoubleQuotes = false;

                    if (!substr.IsEmpty) return substr.ToString();
                }

                if (!inDoubleQuotes && !inLiteral && src[i] == Space)
                {
                    var substr = src.Substring(lastIndex, i - lastIndex);

                    lastIndex = i + 1;

                    if (!substr.IsEmpty) return substr.ToString();
                }
            }

            return src.ToString();
        }

        public static ReadOnlySpan<char> Substring(this ReadOnlySpan<char> src, int start, int len)
        {
            if (src.IsEmpty) return src;

            return src.Slice(start, len);
        }

        public static string RemoveLiteralsAndQuotes(this ReadOnlySpan<char> value)
        {
            if (value.IsEmpty) return new string(value.ToArray());

            int startTrim = 0, endTrim = 0;
            int len = value.Length - 1;

            if (value.Length == 0) return new string(value.ToArray());

            if (value[0] == DoubleQuote || value[0] == SingleQuote)
                startTrim = 1;

            if (value[len] == DoubleQuote || value[len] == SingleQuote)
                endTrim = 1;

            if (startTrim == 0 && endTrim == 0) return new string(value.ToArray());

            return new string(value.Substring(startTrim, len - endTrim).ToArray());
        }

        private const char DoubleQuote = '"';
        private const char SingleQuote = '\'';
        private const char Space = ' ';

    }
}
