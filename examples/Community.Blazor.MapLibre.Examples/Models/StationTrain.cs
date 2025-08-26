namespace Community.Blazor.MapLibre.Examples.WebAssembly.Models
{  
    public class StationTrain
    {
        public int? delay { get; set; } = default!;
        public TrainOrigin trainOrigin { get; set; } = default!;
        public TrainDestination trainDestination { get; set; } = default!;
        public TimeOnly? departureTime { get; set; } = default!;
        public TimeOnly? arrivalTime { get; set; } = default!;
        public int trainNumber { get; set; }
        public TrainService trainService { get; set; } = default!;
        public string platform { get; set; } = default!;
        public int? occupancy { get; set; } = default!;
        public TimeOnly? eta { get; set; } = default!;
        public TimeOnly? etd { get; set; } = default!;
    }

    public class TrainDestination
    {
        public string code { get; set; } = default!;
        public string designation { get; set; } = default!;
    }

    public class TrainOrigin
    {
        public string code { get; set; } = default!;
        public string designation { get; set; } = default!;
    }

    public class TrainService
    {
        public string code { get; set; } = default!;
        public string designation { get; set; } = default!;
    }
}
