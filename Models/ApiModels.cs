using System.Text.Json.Serialization;

namespace CrossPubInsightUI.Models;

/// <summary>
/// Request model for starting a new analysis
/// </summary>
public class AnalysisRequest
{
    [JsonPropertyName("primary_repo")]
    public string PrimaryRepo { get; set; } = string.Empty;

    [JsonPropertyName("user_query")]
    public string UserQuery { get; set; } = string.Empty;

    [JsonPropertyName("comparison_repos")]
    public string[]? ComparisonRepos { get; set; }
}

/// <summary>
/// Response model when starting an analysis
/// </summary>
public class AnalysisResponse
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Response model for getting analysis results
/// </summary>
public class ResultsResponse
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("analysis_result")]
    public object? AnalysisResult { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Health check response model
/// </summary>
public class HealthResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Monitoring health response model
/// </summary>
public class MonitoringHealthResponse
{
    [JsonPropertyName("status")]
    public string OverallStatus { get; set; } = string.Empty;

    [JsonPropertyName("health_checks")]
    public Dictionary<string, object> Checks { get; set; } = new();

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Monitoring metrics response model
/// </summary>
public class MonitoringMetricsResponse
{
    [JsonPropertyName("memory_usage")]
    public Dictionary<string, object> System { get; set; } = new();

    [JsonPropertyName("application")]
    public Dictionary<string, object> Application { get; set; } = new();

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Analysis status enumeration for UI state management
/// </summary>
public enum AnalysisStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}

/// <summary>
/// UI state model for tracking analysis progress
/// </summary>
public class AnalysisState
{
    public string? SessionId { get; set; }
    public AnalysisStatus Status { get; set; } = AnalysisStatus.NotStarted;
    public string? ErrorMessage { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public object? Results { get; set; }
}