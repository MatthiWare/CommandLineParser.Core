using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.CommandLine.Core.Usage
{
    internal class DamerauLevenshteinSuggestionProvider : ISuggestionProvider
    {
        private const double threshold = 0.33;

        public IEnumerable<string> GetSuggestions(string input, ICommandLineCommandContainer command)
        {
            if (command is null)
            {
                return Array.Empty<string>();
            }

            var suggestions = GetAvailableSuggestions(command);

            return GetBestMatchesOrdered(suggestions, input);
        }

        private IEnumerable<string> GetAvailableSuggestions(ICommandLineCommandContainer commandContext)
        {
            foreach (var cmd in commandContext.Commands)
            {
                yield return cmd.Name;
            }

            foreach (var option in commandContext.Options)
            {
                if (option.HasShortName)
                {
                    yield return option.ShortName;
                }

                if (option.HasLongName)
                {
                    yield return option.LongName;
                }
            }
        }

        private IEnumerable<string> GetBestMatchesOrdered(IEnumerable<string> suggestions, string input)
        {
            int inputLength = input.Length;

            return suggestions
                .Select(suggestion =>
                {
                    var dist = FindDistance(input, suggestion);
                    var len = Math.Max(inputLength, suggestion.Length);
                    var normalized = NormalizeDistance(dist, len);
                    return (suggestion, normalized);
                })
                .Where(_ => _.normalized >= threshold)
                .OrderByDescending(_ => _.normalized)
                .Select(_ => _.suggestion);
        }

        private double NormalizeDistance(int distance, int length)
        {
            if (length == 0)
            {
                return 0;
            }

            if (distance == 0)
            {
                return 1;
            }

            return 1.0d - distance / (double)length;
        }

        /// <summary>
        /// Damerau-Levenshtein Distance algorithm implemented using Optimal String Alignment Distance. 
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
        /// </remarks>
        /// <param name="input">string A</param>
        /// <param name="suggestion">string B</param>
        /// <returns>Damerau-Levenshtein Distance</returns>
        internal int FindDistance(in string input, in string suggestion)
        {
            // Short circuit so we don't waste cpu time
            if (input == suggestion)
            {
                return 0;
            }

            if (string.IsNullOrEmpty(input))
            {
                return !string.IsNullOrEmpty(suggestion) ? suggestion.Length : 0;
            }

            if (string.IsNullOrEmpty(suggestion))
            {
                return !string.IsNullOrEmpty(input) ? input.Length : 0;
            }

            var inputLength = input.Length;
            var suggestionLength = suggestion.Length;

            var matrix = CreateMatrix(in inputLength, in suggestionLength);

            for (var i = 1; i <= inputLength; i++)
            {
                for (var j = 1; j <= suggestionLength; j++)
                {
                    CalculateCost(in i, in j, in matrix, in input, in suggestion);
                }
            }

            return matrix[inputLength, suggestionLength];
        }

        private void CalculateCost(in int i, in int j, in int[,] matrix, in string input, in string suggestion)
        {
            var cost = input[i - 1] == suggestion[j - 1] ? 0 : 1;

            var changes = new[]
            {
                //Insertion
                matrix[i, j - 1] + 1,
                //Deletion
                matrix[i - 1, j] + 1,
                //Substitution
                matrix[i - 1, j - 1] + cost
            };

            var distance = changes.Min();

            if (i > 1 && j > 1 && input[i - 1] == suggestion[j - 2] && input[i - 2] == suggestion[j - 1])
            {
                // Transposition of two successive symbols
                distance = Math.Min(distance, matrix[i - 2, j - 2] + cost);
            }

            matrix[i, j] = distance;
        }

        private int[,] CreateMatrix(in int sizeA, in int sizeB)
        {
            var matrix = new int[sizeA + 1, sizeB + 1];

            for (var i = 0; i <= sizeA; i++)
            {
                matrix[i, 0] = i;
            }

            for (var j = 0; j <= sizeB; j++)
            {
                matrix[0, j] = j;
            }

            return matrix;
        }
    }
}
