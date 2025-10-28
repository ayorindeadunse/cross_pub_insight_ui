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

    // API returns version (string) and dependencies (dict) in the health response
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("dependencies")]
    public Dictionary<string, object>? Dependencies { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    // Backwards-compatible message property (UI expects Message)
    [JsonIgnore]
    public string Message => Version;
}

/// <summary>
/// Monitoring health response model
/// </summary>
public class MonitoringHealthResponse
{
    // Matches api.models.MonitoringHealthResponse
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("health_checks")]
    public Dictionary<string, object> HealthChecks { get; set; } = new();

    [JsonPropertyName("system_metrics")]
    public Dictionary<string, object> SystemMetrics { get; set; } = new();

    [JsonPropertyName("uptime_seconds")]
    public double UptimeSeconds { get; set; }

    // Backwards-compatible aliases used by the existing UI
    [JsonIgnore]
    public string OverallStatus => Status;

    [JsonIgnore]
    public Dictionary<string, object> Checks => HealthChecks;
}

/// <summary>
/// Monitoring metrics response model
/// </summary>
public class MonitoringMetricsResponse
{
    // Matches api.models.MonitoringMetricsResponse
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("memory_usage")]
    public Dictionary<string, object> MemoryUsage { get; set; } = new();

    [JsonPropertyName("operation_counts")]
    public Dictionary<string, object> OperationCounts { get; set; } = new();

    [JsonPropertyName("response_times")]
    public Dictionary<string, object> ResponseTimes { get; set; } = new();

    [JsonPropertyName("error_rates")]
    public Dictionary<string, object> ErrorRates { get; set; } = new();

    // Backwards-compatible aliases for older UI expectations
    [JsonIgnore]
    public Dictionary<string, object> System => MemoryUsage;

    [JsonIgnore]
    public Dictionary<string, object> Application => OperationCounts;
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