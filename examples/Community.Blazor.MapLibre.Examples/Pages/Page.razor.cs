using Community.Blazor.MapLibre.Examples.WebAssembly.Models;
using Community.Blazor.MapLibre.Models;
using Community.Blazor.MapLibre.Models.Marker;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;

namespace Community.Blazor.MapLibre.Examples.WebAssembly.Pages
{
    public partial class Page
    {
        //https://www.reddit.com/r/webdev/comments/1ii43ns/list_of_free_cors_proxies/
        private string CorsProxy = "https://api.codetabs.com/v1/proxy/?quest=";

        [Inject]
        public Blazored.LocalStorage.ILocalStorageService localStorage { get; set; } = default!;

        private SfDropDownList<Station, Station> stationDropdownRef { get; set; } = default!;
        private List<Station> Stations { get; set; } = [];
        private List<StationTrain> StationInfo { get; set; } = [];
        private SfGrid<StationTrain> StationInfoGrid { get; set; } = default!;
        private Station Station { get; set; } = default!;
        private int? TrainId { get; set; } = default!;
        private TimeOnly? LastUpdate { get; set; } = default!;
        private long RequestElapsedMilliseconds { get; set; } = default!;
        private Train Train { get; set; } = default!;
        private MapLibre _mapRef { get; set; } = default!;
        private Guid? TrainMarker { get; set; } = default!;
        private List<Guid> StationMarkers { get; set; } = []!;
        private readonly MapOptions _mapOptions = new()
        {
            Container = "UniqueMapId",
            Center = new LngLat()
            {
                Latitude = 38.7071,
                Longitude = -9.13549
            },
            Zoom = 7,
            Style = "https://api.protomaps.com/styles/v5/light/pt.json?key=c1d387e4690c625f",
        };

        public string TrainIdStorageKey = "cp_trainId_storage_key";
        public string StationStorageKey = "cp_station_storage_key";

        protected override async Task OnInitializedAsync()
        {
            using (HttpClient stationsClient = new())
            {

                HttpResponseMessage stationsRequestResponse = await stationsClient.GetAsync($"{CorsProxy}https://www.cp.pt/sites/spring/station-index");

                if (!stationsRequestResponse.IsSuccessStatusCode)
                    return;

                //ProxyResponse? stationsResponse = await stationsRequestResponse.Content.ReadFromJsonAsync<ProxyResponse>();

                //if (stationsResponse == null)
                //    return;

                //Dictionary<string, string>? stations = JsonSerializer.Deserialize<Dictionary<string, string>>(stationsResponse.contents);

                //if (stations == null)
                //    return;
                Dictionary<string, string>? stations = null!;

                try
                {
                    stations = await stationsRequestResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                }
                catch
                {

                }

                if (stations == null)
                    return;

                Stations = [.. stations.Select(s => new Station()
                {
                    Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.Key),
                    Code = s.Value
                })];

                await stationDropdownRef.RefreshDataAsync();

            }
            Station = await localStorage.GetItemAsync<Station>(StationStorageKey) ?? default!;
            if (Station != null)
                await SetStationDeparturesAsync();
            TrainId = await localStorage.GetItemAsync<int?>(TrainIdStorageKey) ?? default!;
            if (TrainId != null)
                await SetTrainAsync();

            while (true)
            {


                if (TrainId.HasValue && TrainId != 0)
                {
                    await SetTrainAsync();
                    await InvokeAsync(StateHasChanged);   // refresh everything 
                }
                //25 seg
                await Task.Delay(25000);

            }
            //await base.OnInitializedAsync();
        }

        private async Task OnOriginChangedAsync()
        {
            await localStorage.SetItemAsync(StationStorageKey, Station);
            await SetStationDeparturesAsync();

        }

        public async Task SetStationDeparturesAsync()
        {
            if (Station == null)
            {
                StationInfo = [];
            }
            else
            {

                using HttpClient stationsDeparturesClient = new();

                HttpResponseMessage stationsDeparturesRequestResponse = await stationsDeparturesClient.GetAsync($"{CorsProxy}https://www.cp.pt/sites/spring/station/trains?stationId={Station.Code}");

                if (!stationsDeparturesRequestResponse.IsSuccessStatusCode)
                    return;

                //ProxyResponse? stationsDeparturesResponse = await stationsDeparturesRequestResponse.Content.ReadFromJsonAsync<ProxyResponse>();

                //if (stationsDeparturesResponse == null)
                //    return;

                //List<StationTrain>? stationsDepartures = JsonSerializer.Deserialize<List<StationTrain>>(stationsDeparturesResponse.contents);

                //if (stationsDepartures == null)
                //    return;

                List<StationTrain>? stationsDepartures = null!;
                try
                {
                    stationsDepartures = await stationsDeparturesRequestResponse.Content.ReadFromJsonAsync<List<StationTrain>>();
                }
                catch
                {

                }

                if (stationsDepartures == null)
                    return;

                StationInfo = stationsDepartures;
            }

            if (StationInfoGrid != null)
                await StationInfoGrid.Refresh();

            await InvokeAsync(StateHasChanged);

        }
        private async Task OnTrainIdChangedAsync(int? trainId)
        {
            TrainId = trainId;
            await localStorage.SetItemAsync(TrainIdStorageKey, TrainId);
            await SetTrainAsync();

        }

        public async Task SetTrainAsync()
        {
            if (TrainMarker.HasValue)
            {
                await _mapRef.RemoveMarker(TrainMarker.Value);
                TrainMarker = default!;
            }
            foreach (Guid stationMarker in StationMarkers)
            {
                await _mapRef.RemoveMarker(stationMarker);
            }
            StationMarkers = [];

            if (!TrainId.HasValue || TrainId == 0)
            {
                Train = default!;
            }
            else
            {
                using HttpClient trainClient = new();
                Stopwatch sw = Stopwatch.StartNew();
                HttpResponseMessage trainRequestResponse = await trainClient.GetAsync($"{CorsProxy}https://www.cp.pt/sites/spring/station/trains/train?trainId={TrainId}");
                sw.Stop();
                if (!trainRequestResponse.IsSuccessStatusCode)
                    return;

                //ProxyResponse? trainResponse = await trainRequestResponse.Content.ReadFromJsonAsync<ProxyResponse>();

                //if (trainResponse == null)
                //    return;

                //Train? train = JsonSerializer.Deserialize<Train>(trainResponse.contents);

                //if (train == null)
                //    return;

                Train? train = null!;

                try
                {
                    train = await trainRequestResponse.Content.ReadFromJsonAsync<Train>();

                }
                catch
                {
                }


                if (train == null)
                    return;

                Train = train;

                LastUpdate = TimeOnly.FromDateTime(DateTime.Now);
                RequestElapsedMilliseconds = sw.ElapsedMilliseconds;

                if (_mapRef != null)
                {
                    foreach (TrainStop trainStop in train.trainStops)
                    {
                        if (trainStop.latitude != null && trainStop.longitude != null)
                        {
                            MarkerOptions options = new()
                            {
                                Extensions = new MarkerOptionsExtensions
                                {
                                    HtmlContent = "<div><img src='https://upload.wikimedia.org/wikipedia/commons/7/77/Logo_CP_2.svg' width='30' height='30' class='border border-white border-3 rounded-circle shadow-lg'/></div>"
                                }
                            };
                            LngLat coordinates = GetCoordinates(trainStop.longitude, trainStop.latitude);
                            StationMarkers.Add(await _mapRef.AddMarker(options, coordinates));
                        }
                    }

                    if (train.latitude != null && train.longitude != null)
                    {
                        MarkerOptions options = new()
                        {
                            Extensions = new MarkerOptionsExtensions
                            {
                                HtmlContent = "<div><img src='https://cdn-icons-png.flaticon.com/512/7721/7721842.png' width='30' height='30' class='border border-white border-3 rounded-circle shadow-lg'/></div>"
                            }
                        };
                        LngLat coordinates = GetCoordinates(train.longitude, train.latitude);
                        TrainMarker = await _mapRef.AddMarker(options, coordinates);
                        await _mapRef.SetCenter(coordinates);
                    }


                }
            }
            await InvokeAsync(StateHasChanged);

        }

        private static LngLat GetCoordinates(string longitude, string latitude)
        {
            longitude = longitude.Replace(",", ".");
            latitude = latitude.Replace(",", ".");
            return new(double.Parse(longitude, NumberStyles.Float, NumberFormatInfo.InvariantInfo), double.Parse(latitude, NumberStyles.Float, NumberFormatInfo.InvariantInfo));
        }


    }
}