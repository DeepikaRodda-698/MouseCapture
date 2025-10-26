using MouseCapture.Models;
using System;

namespace MouseCapture.Models
{
    public sealed class MouseEventInfo
    {
        public int X { get; init; }
        public int Y { get; init; }
        public uint MouseData { get; init; }
        public uint Flags { get; init; }
        public uint Time { get; init; }
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
        public string Message { get; init; }
        public override string ToString() => $"{OccurredAt.ToLocalTime().ToString()} - {Message}";
    }
}