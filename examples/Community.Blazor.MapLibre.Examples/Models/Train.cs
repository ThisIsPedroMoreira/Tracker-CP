namespace Community.Blazor.MapLibre.Examples.WebAssembly.Models
{  
    public class Train
    {
        public int trainNumber { get; set; }
        public ServiceCode serviceCode { get; set; } = default!;
        public int? delay { get; set; }
        public int? occupancy { get; set; } = default!;
        public string latitude { get; set; } = default!;
        public string longitude { get; set; } = default!;
        public string status { get; set; } = default!;
        public List<TrainStop> trainStops { get; set; } = default!;
    }

    public class ServiceCode
    {
        public string code { get; set; } = default!;
        public string designation { get; set; } = default!;
    }

    public class TrainStation
    {
        public string code { get; set; } = default!;
        public string designation { get; set; } = default!;
    }

    public class TrainStop
    {
        public TrainStation station { get; set; } = default!;
        public TimeOnly? arrival { get; set; } = default!;
        public TimeOnly? departure { get; set; } = default!;
        public string platform { get; set; } = default!;
        public string latitude { get; set; } = default!;
        public string longitude { get; set; } = default!;
        public int? delay { get; set; } = default!;
        public TimeOnly? eta { get; set; } = default!;
        public TimeOnly? etd { get; set; } = default!;
    }

}
