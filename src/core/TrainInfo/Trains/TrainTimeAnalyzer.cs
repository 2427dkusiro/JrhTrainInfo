using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainInfo;
using TrainInfo.Stations;
using TrainInfo.TrainDatas;
using TrainInfo.TrainPositions;

namespace TrainInfo.Trains
{
    public static class TrainTimeAnalyzer
    {
        public static async Task<List<Train>> GetTrainTime(IEnumerable<TrainDataFile> trainDataFiles)
        {
            var time = trainDataFiles.First().GetedDateTime;
            if (trainDataFiles.Any(tdf => tdf.GetedDateTime != time))
            {
                throw new ArgumentException();
            }

            var posData = await GetPositionDatas(trainDataFiles);

            var trainDataList = new List<TrainData>();

            foreach (var tdf in trainDataFiles)
            {
                foreach (var arr in tdf.ArrivalTrainDatas)
                {
                    trainDataList.AddRange(arr.Value);
                }
                foreach (var dep in tdf.DepartureTrainDatas)
                {
                    trainDataList.AddRange(dep.Value);
                }
            }

            var valid = GetValidRange(posData);
            var result = GetTrainList(valid, trainDataList);
            return result;
        }

        private async static Task<IEnumerable<TrainPositionData>> GetPositionDatas(IEnumerable<TrainDataFile> trainDataFiles)
        {
            Dictionary<int, TrainDataFile> dict = new Dictionary<int, TrainDataFile>();
            foreach (var tdf in trainDataFiles)
            {
                dict.Add(tdf.Station.StationId, tdf);
            }

            var lines = Enum.GetValues(typeof(JrhLine)).Cast<JrhLine>();
            var result = new List<TrainPositionData>();

            foreach (var line in lines)
            {
                if (line == JrhLine.Sekisyo)
                {
                    continue;
                }

                try
                {
                    var (arrivalData, departureData) = await TrainInfoReader.GetTrainPositionDataAsync(line, dict);
                    result.Add(arrivalData);
                    result.Add(departureData);
                }
                catch (NotImplementedException ex)
                {

                }
            }

            return result;
        }

        private static List<LineRange> GetValidRange(IEnumerable<TrainPositionData> trainPositionDatas)
        {
            var result = new List<LineRange>();

            foreach (var tpd in trainPositionDatas)
            {
                foreach (var tuple in tpd)
                {
                    var rangeData = tuple.Item2.Select(td => td.NowPosition);
                    var range = rangeData.FirstOrDefault();
                    if (range is null)
                    {
                        continue;
                    }

                    if (rangeData.Any(r => r != range))
                    {
                        throw new InvalidOperationException();
                    }

                    if (range.IsStation)
                    {
                        var station = range.StartPos;
                        if (station.IsEndStation && station.Position.Count > 1)
                        {
                            result.RemoveAll(r => r == range);
                            continue;
                        }
                    }


                    if (tuple.Item2.Count == 1)
                    {
                        if (result.Any(r => r == range))
                        {
                            result.RemoveAll(r => r == range);
                        }
                        else
                        {
                            result.Add(range);
                        }
                    }
                    else if (tuple.Item2.Count > 1)
                    {
                        result.RemoveAll(r => r == range);
                    }
                }
            }

            return result;
        }

        private static List<Train> GetTrainList(IEnumerable<LineRange> lineRanges, IEnumerable<TrainData> trainDatas)
        {
            List<(LineRange, Train)> list = new List<(LineRange, Train)>();

            foreach (var td in trainDatas)
            {
                if (lineRanges.Any(l => l == td.NowPosition))
                {
                    //tdは有効

                    if (list.Any(l => l.Item1 == td.NowPosition))
                    {
                        var train = list.First(l => l.Item1 == td.NowPosition).Item2;
                        if (train.TimeData.TryGetValue(td.DepartureStation, out var stationTrainTime))
                        {
                            AddTimeData(ref stationTrainTime, td);
                        }
                        else
                        {
                            var stt = new StationTrainTime();
                            AddTimeData(ref stt, td);
                            train.TimeData.Add(td.DepartureStation, stt);
                        }
                    }
                    else
                    {
                        var train = new Train();
                        train.Destination = td.Destination;
                        train.TrainName = td.Name;
                        var stt = new StationTrainTime();
                        AddTimeData(ref stt, td);
                        train.TimeData.Add(td.DepartureStation, stt);
                        list.Add((td.NowPosition, train));
                    }
                }
            }

            return new List<Train>(list.Select(tuple => tuple.Item2));
        }

        private static void AddTimeData(ref StationTrainTime stationTrainTime, TrainData td)
        {
            if (td.ArrivalType == TrainData.ArrivalTypes.Arrival)
            {
                stationTrainTime.ArrivalTime = td.Time;
            }
            else if (td.ArrivalType == TrainData.ArrivalTypes.Departure)
            {
                stationTrainTime.DepartureTime = td.Time;
            }
        }
    }
}
