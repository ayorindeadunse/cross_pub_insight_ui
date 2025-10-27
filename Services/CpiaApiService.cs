using CrossPubInsightUI.Models;
using System.Text.Json;
using System.Text;

namespace CrossPubInsightUI.Services;

/// <summary>
/// Service for communicating with the Cross Publication Insight Assistant API
/// </summary>
public class CpiaApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public CpiaApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };
    }

    /// <summary>
    /// Check if the API is healthy and responsive
    /// </summary>
    public async Task<HealthResponse?> CheckHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/health/");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<HealthResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Health check failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Start a new repository analysis
    /// </summary>
    public async Task<AnalysisResponse?> StartAnalysisAsync(AnalysisRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/run-analysis/", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AnalysisResponse>(responseContent, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Start analysis failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get results for a specific analysis session
    /// </summary>
    public async Task<ResultsResponse?> GetResultsAsync(string sessionId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/results/{sessionId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ResultsResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get results failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get detailed health information from monitoring endpoint
    /// </summary>
    public async Task<MonitoringHealthResponse?> GetMonitoringHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/monitoring/health");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MonitoringHealthResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Monitoring health check failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get system metrics from monitoring endpoint
    /// </summary>
    public async Task<MonitoringMetricsResponse?> GetMonitoringMetricsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/monitoring/metrics");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MonitoringMetricsResponse>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get monitoring metrics failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Poll for analysis completion (helper method for UI)
    /// </summary>
    public async Task<AnalysisState> PollForCompletionAsync(string sessionId, int maxAttempts = 30, int delaySeconds = 5)
    {
        var state = new AnalysisState 
        { 
            SessionId = sessionId, 
            Status = AnalysisStatus.InProgress,
            StartTime = DateTime.Now
        };

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var result = await GetResultsAsync(sessionId);
            
            if (result == null)
            {
                state.Status = AnalysisStatus.Failed;
                state.ErrorMessage = "Failed to get results from API";
                state.EndTime = DateTime.Now;
                return state;
            }

            switch (result.Status.ToLower())
            {
                case "completed":
                    state.Status = AnalysisStatus.Completed;
                    state.Results = result.AnalysisResult;
                    state.EndTime = DateTime.Now;
                    return state;
                
                case "failed":
                case "error":
                    state.Status = AnalysisStatus.Failed;
                    state.ErrorMessage = result.ErrorMessage ?? "Analysis failed";
                    state.EndTime = DateTime.Now;
                    return state;
                
                case "processing":
                case "in_progress":
                    // Continue polling
                    break;
            }

            if (attempt < maxAttempts - 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        // Timeout
        state.Status = AnalysisStatus.Failed;
        state.ErrorMessage = "Analysis timed out";
        state.EndTime = DateTime.Now;
        return state;
    }
}