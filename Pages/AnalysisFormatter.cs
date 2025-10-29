using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CrossPubInsightUI.Models;

namespace CrossPubInsightUI.Pages
{
    /// <summary>
    /// Utility class for formatting analysis results
    /// </summary>
    public static class AnalysisFormatter
    {
        /// <summary>
        /// Formats raw analysis results into a readable string format
        /// </summary>
        /// <param name="results">The raw analysis results object</param>
        /// <returns>Formatted string representation of the analysis results</returns>
        public static string FormatResults(object? results)
        {
            if (results == null) return "No results available";
            
            try
            {
                // Parse the analysis results
                var jsonString = results.ToString() ?? "";
                var analysisResults = JsonSerializer.Deserialize<AnalysisResultItem[]>(jsonString);
                
                if (analysisResults == null || analysisResults.Length == 0)
                {
                    return "No analysis results found";
                }

                var formattedOutput = new StringBuilder();
                
                foreach (var result in analysisResults)
                {
                    // Repository Information
                    if (!string.IsNullOrEmpty(result.ComparisonRepo))
                    {
                        formattedOutput.AppendLine("📁 REPOSITORY ANALYSIS");
                        formattedOutput.AppendLine("=" + new string('=', 50));
                        formattedOutput.AppendLine($"Repository: {result.ComparisonRepo}");
                        formattedOutput.AppendLine();
                    }

                    // Primary Analysis
                    if (!string.IsNullOrEmpty(result.AnalysisResult))
                    {
                        formattedOutput.AppendLine("🔍 PRIMARY ANALYSIS");
                        formattedOutput.AppendLine("=" + new string('=', 50));
                        formattedOutput.AppendLine(result.AnalysisResult);
                        formattedOutput.AppendLine();
                    }

                    // Fact Check Results
                    if (!string.IsNullOrEmpty(result.FactCheckResult))
                    {
                        formattedOutput.AppendLine("🧪 FACT CHECK ANALYSIS");
                        formattedOutput.AppendLine("=" + new string('=', 50));
                        formattedOutput.AppendLine(FormatMarkdownText(result.FactCheckResult));
                        formattedOutput.AppendLine();
                    }

                    // Aggregate Query Results
                    if (!string.IsNullOrEmpty(result.AggregateQueryResult) && 
                        result.AggregateQueryResult != "No aggregate query generated.")
                    {
                        formattedOutput.AppendLine("📊 AGGREGATE ANALYSIS");
                        formattedOutput.AppendLine("=" + new string('=', 50));
                        formattedOutput.AppendLine(result.AggregateQueryResult);
                        formattedOutput.AppendLine();
                    }

                    // Final Summary
                    if (!string.IsNullOrEmpty(result.FinalSummary))
                    {
                        formattedOutput.AppendLine("📋 EXECUTIVE SUMMARY");
                        formattedOutput.AppendLine("=" + new string('=', 50));
                        formattedOutput.AppendLine(FormatMarkdownText(result.FinalSummary));
                        formattedOutput.AppendLine();
                    }
                }
                
                return formattedOutput.ToString();
            }
            catch (Exception ex)
            {
                return $"Error formatting results: {ex.Message}\n\nRaw data:\n{JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true })}";
            }
        }

        /// <summary>
        /// Formats markdown text for better readability in plain text format
        /// </summary>
        /// <param name="text">The markdown text to format</param>
        /// <returns>Cleaned up text suitable for display</returns>
        private static string FormatMarkdownText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            
            // Simple markdown formatting for better readability
            var formatted = text;
            
            // Clean up multiple newlines
            formatted = Regex.Replace(formatted, @"\n{3,}", "\n\n");
            
            // Format headers (remove # symbols for plain text display)
            formatted = Regex.Replace(formatted, @"^#+\s*(.+)$", "$1", RegexOptions.Multiline);
            
            // Add spacing around sections
            formatted = Regex.Replace(formatted, @"^(Solution:|Question:|Exercise:|FACT CHECK RESULT)", "\n$1", RegexOptions.Multiline);
            
            // Clean up confidence rating
            formatted = Regex.Replace(formatted, @"Confidence Rating:\s*\*\*(\w+)\*\*", "🎯 CONFIDENCE LEVEL: $1", RegexOptions.IgnoreCase);
            
            // Clean up fact check banner
            formatted = Regex.Replace(formatted, @"#{40,}.*?FACT CHECK RESULT.*?#{40,}", "🧪 FACT CHECK ANALYSIS", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            
            return formatted;
        }
    }
}