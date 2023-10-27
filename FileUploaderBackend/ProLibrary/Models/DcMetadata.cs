using System;
using System.Collections.Generic;

namespace ProLibrary.Models
{
    public partial class DcMetadata
    {
        public int ItemId { get; set; }
        public string? ItemSource { get; set; }
        public string? Host { get; set; }
        public string? ItemContainer { get; set; }
        public string? ItemName { get; set; }
        public string? ItemType { get; set; }
        public string? ItemComment { get; set; }
        public string? Unit { get; set; }
        public double? Scaling { get; set; }
        public double? UpdateCycle { get; set; }
        public string? Sensor { get; set; }
        public double? MinVal { get; set; }
        public double? MaxVal { get; set; }
        public string? Orientation { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}
