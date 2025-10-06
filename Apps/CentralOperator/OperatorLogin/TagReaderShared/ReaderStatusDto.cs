using Newtonsoft.Json;
using System;

namespace TagReaderShared
{
    [Serializable]
    public sealed class ReaderStatusDto
    {
        [JsonProperty("LocationID")]
        public int LocationId { get; set; }

        [JsonProperty("MachineID")]
        public int MachineId { get; set; }

        [JsonProperty("SubID")]
        public int SubId { get; set; }

        [JsonProperty("IsConnected")]
        public bool IsConnected { get; set; }

        // Keep JSON as CurrentLED, but code uses LedColor
        [JsonProperty("CurrentLED")]
        public string LedColor { get; set; } = "GREY";

        [JsonProperty("IsActiveStation")]
        public bool IsActiveStation { get; set; }

        [JsonProperty("IpAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("Port")]
        public int Port { get; set; }

        [JsonProperty("LastUpdate")]
        public DateTime LastUpdate { get; set; }
    }
}
