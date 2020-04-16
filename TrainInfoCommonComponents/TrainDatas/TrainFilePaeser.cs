using System;
using System.Collections.Generic;
using System.Linq;
using TrainInfo.Stations;

namespace TrainInfo.TrainDatas
{
    internal static class TrainFilePaeser
    {
        /// <summary>
        /// <see cref="RawTrainDataFile"/>を解析して<see cref="TrainDataFile"/>を取得します。
        /// </summary>
        /// <param name="rawTrainDataFile">解析対象のデータ</param>
        /// <returns></returns>
        public static TrainDataFile ParseRawTrainDataFile(RawTrainDataFile rawTrainDataFile)
        {
            var getedDateTime = DateTime.ParseExact(rawTrainDataFile.Departures_datetime.ToString(), "yyyyMMddHHmm", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            var station = StationReader.GetStationById(rawTrainDataFile.StationId);

            var gettedHour = getedDateTime.Hour;
            var arrivalTrainDatas = new Dictionary<JrhDestType, IReadOnlyList<TrainData>>();
            var departureTrainDatas = new Dictionary<JrhDestType, IReadOnlyList<TrainData>>();

            foreach (var rawArrData in rawTrainDataFile.Arrivals)
            {
                var dest = JrhDestTypeCreater.FromParseText(rawArrData.Key);

                var data = rawArrData.Value.Select(rtd => TrainDataParser.ParseTrainData(rtd, gettedHour, station, TrainData.ArrivalTypes.Arrival));
                arrivalTrainDatas.Add(dest, Array.AsReadOnly(data.ToArray()));
            }

            foreach (var rawDepData in rawTrainDataFile.Departures)
            {
                var dest = JrhDestTypeCreater.FromParseText(rawDepData.Key);
                var data = rawDepData.Value.Select(rtd => TrainDataParser.ParseTrainData(rtd, gettedHour, station, TrainData.ArrivalTypes.Departure));
                departureTrainDatas.Add(dest, Array.AsReadOnly(data.ToArray()));
            }

            return new TrainDataFile(station, getedDateTime, departureTrainDatas, arrivalTrainDatas);
        }
    }
}
