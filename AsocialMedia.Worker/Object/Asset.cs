﻿using Newtonsoft.Json;

namespace AsocialMedia.Worker.Object;

internal class Asset
{
    [JsonProperty(Required = Required.Always)]
    public string Url { get; set; } = string.Empty;

    public string? Credit { get; set; } = null;

    [JsonProperty(PropertyName = "end_time")]
    public TimeSpan? EndTime { get; set; } = null;

    [JsonProperty(PropertyName = "start_time")]
    public TimeSpan? StartTime { get; set; } = null;
}
