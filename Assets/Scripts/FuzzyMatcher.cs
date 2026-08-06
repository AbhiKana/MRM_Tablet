using System;
using System.Collections.Generic;
using System.Linq;

public static class FuzzyMatcher
{
    // Calculates the number of edits required to turn one string into another
    private static int CalculateLevenshteinDistance(string s1, string s2)
    {
        // Optimization: check for empty strings or exact matches first
        if (string.IsNullOrEmpty(s1)) return string.IsNullOrEmpty(s2) ? 0 : s2.Length;
        if (string.IsNullOrEmpty(s2)) return s1.Length;
        if (s1 == s2) return 0;

        int len1 = s1.Length;
        int len2 = s2.Length;

        // Create the distance matrix
        int[,] dist = new int[len1 + 1, len2 + 1];

        // Initialize the matrix
        for (int i = 0; i <= len1; i++) dist[i, 0] = i;
        for (int j = 0; j <= len2; j++) dist[0, j] = j;

        // Fill the matrix
        for (int i = 1; i <= len1; i++)
        {
            for (int j = 1; j <= len2; j++)
            {
                int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;

                dist[i, j] = Math.Min(
                    Math.Min(dist[i - 1, j] + 1,      // Deletion
                             dist[i, j - 1] + 1),     // Insertion
                             dist[i - 1, j - 1] + cost); // Substitution
            }
        }

        return dist[len1, len2];
    }

    /// <summary>
    /// Tries to find a value in the dictionary. 
    /// If an exact match isn't found, it looks for the closest match based on the threshold.
    /// </summary>
    public static TValue GetWholeValueFuzzy<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey input, double threshold = 0.7) where TKey : class
    {
        // 1. Attempt exact match first (Fastest)
        if (dict.ContainsKey(input))
        {
            return dict[input];
        }

        // Ensure we are working with strings
        string inputStr = input as string;
        if (inputStr == null) return default(TValue);

        // 2. Search for best fuzzy match
        TKey bestMatchKey = default(TKey);
        double highestSimilarity = 0.0;

        foreach (var pair in dict)
        {
            string keyStr = pair.Key as string;
            if (keyStr == null) continue;

            // Optional: Optimization to skip keys with huge length differences
            if (Math.Abs(keyStr.Length - inputStr.Length) > inputStr.Length * 0.5) continue;

            int distance = CalculateLevenshteinDistance(inputStr, keyStr);
            int maxLen = Math.Max(inputStr.Length, keyStr.Length);

            // Calculate similarity ratio (1.0 = identical, 0.0 = completely different)
            double similarity = 1.0 - ((double)distance / maxLen);

            if (similarity > highestSimilarity)
            {
                highestSimilarity = similarity;
                bestMatchKey = pair.Key;
            }
        }

        // 3. Return value if the best match meets the threshold
        if (highestSimilarity >= threshold && bestMatchKey != null)
        {
            // Optional: Log the substitution
            // Console.WriteLine($"Fuzzy Match: '{input}' -> '{bestMatchKey}' ({highestSimilarity:P0})");
            return dict[bestMatchKey];
        }

        // 4. No match found
        return default(TValue);
    }

    /// <summary>
    /// Returns all keys whose string value fuzzy-matches the input.
    /// Case-insensitive, and always includes exact (case-insensitive) matches.
    /// </summary>
    public static List<TKey> GetWholeKeysByFuzzyValue<TKey, TValue>(Dictionary<TKey, TValue> dict, string input, double threshold = 0.7) where TValue : class
    {
        var results = new List<TKey>();
        if (string.IsNullOrEmpty(input)) return results;

        string inputLower = input.ToLower();

        foreach (var pair in dict)
        {
            string valueStr = pair.Value as string;
            if (valueStr == null) continue;

            string valueLower = valueStr.ToLower();

            // 1. Exact (case-insensitive) match — always include
            if (valueLower == inputLower)
            {
                results.Add(pair.Key);
                continue;
            }

            // 2. Skip if length difference is too large (cheap pre-filter)
            if (Math.Abs(valueLower.Length - inputLower.Length) > inputLower.Length * 0.5)
                continue;

            // 3. Fuzzy compare
            int distance = CalculateLevenshteinDistance(inputLower, valueLower);
            int maxLen = Math.Max(inputLower.Length, valueLower.Length);
            double similarity = 1.0 - ((double)distance / maxLen);

            if (similarity >= threshold)
            {
                results.Add(pair.Key);
            }
        }

        return results;
    }

    /// <summary>
    /// Searches dictionary values using a multi-tier word-matching algorithm.
    /// Great for "search-as-you-type" because it matches prefixes and partial words.
    /// </summary>
    public static List<TKey> SearchKeysByValue<TKey, TValue>(
        Dictionary<TKey, TValue> dict,
        string input,
        int maxTyposPerWord = 2) where TValue : class
    {
        var results = new List<KeyValuePair<TKey, int>>(); // Key, Score

        if (string.IsNullOrWhiteSpace(input))
            return results.Select(kvp => kvp.Key).ToList();

        string inputLower = input.ToLower().Trim();

        // Split query into words (e.g., "blu mar" -> ["blu", "mar"])
        string[] queryWords = inputLower.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in dict)
        {
            string valueStr = pair.Value as string;
            if (valueStr == null) continue;

            string valueLower = valueStr.ToLower().Trim();

            // Calculate how well this value matches the input
            int score = CalculateMatchScore(inputLower, queryWords, valueLower, maxTyposPerWord);

            if (score > 0)
            {
                results.Add(new KeyValuePair<TKey, int>(pair.Key, score));
            }
        }

        // Sort by score descending (best matches at the top)
        return results.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
    }

    private static int CalculateMatchScore(string inputLower, string[] queryWords, string valueLower, int maxTypos)
    {
        // Tier 1: Exact match (Highest priority)
        if (valueLower == inputLower) return 10000;

        // Tier 2: Whole query is a prefix of the value (e.g., typing "blue mar" -> "blue marble")
        if (valueLower.StartsWith(inputLower)) return 9000;

        // Tier 3: Whole query is somewhere inside the value (e.g., typing "marb" -> "blue marble")
        if (valueLower.Contains(inputLower)) return 8000;

        // Tier 4: Token-based matching (Evaluate word-by-word)
        string[] targetWords = valueLower.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        int totalScore = 0;
        bool allWordsMatched = true;

        foreach (var qWord in queryWords)
        {
            bool wordMatched = false;
            int bestWordScore = 0;

            foreach (var tWord in targetWords)
            {
                if (tWord == qWord)
                {
                    wordMatched = true;
                    bestWordScore = Math.Max(bestWordScore, 1000); // Exact word match
                }
                else if (tWord.StartsWith(qWord))
                {
                    wordMatched = true;
                    bestWordScore = Math.Max(bestWordScore, 800); // Prefix match (Typing "blu" -> "blue")
                }
                else if (tWord.Contains(qWord))
                {
                    wordMatched = true;
                    bestWordScore = Math.Max(bestWordScore, 600); // Substring match
                }
                else
                {
                    // Fuzzy typo matching per word
                    if (Math.Abs(qWord.Length - tWord.Length) <= maxTypos)
                    {
                        int dist = CalculateLevenshteinDistance(qWord, tWord);
                        if (dist <= maxTypos)
                        {
                            wordMatched = true;
                            // Score based on how close it was (closer = higher score)
                            bestWordScore = Math.Max(bestWordScore, 400 - (dist * 50));
                        }
                    }
                }
            }

            if (!wordMatched)
            {
                allWordsMatched = false;
                break; // If any word in the query fails to match, the whole match fails
            }

            totalScore += bestWordScore;
        }

        if (allWordsMatched) return totalScore;

        return 0; // No match
    }
}
