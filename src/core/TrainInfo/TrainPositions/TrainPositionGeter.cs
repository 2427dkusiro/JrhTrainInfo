using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainInfo.Stations;

namespace TrainInfo.TrainPositions
{
    internal static class TrainPositionGeter
    {
        public static async Task<(TrainPositionData arrivalData, TrainPositionData departureTrainData)> GetTrainPosition(JrhLine jrhLine, Dictionary<int, TrainDataFile> dataFile = null)
        {
            switch (jrhLine)
            {
                case JrhLine.Hakodate_Iwamizawa:
                    {
                        dataFile = dataFile ?? await GetTrainDatas(91, 99, 103);
                        return await GetIwamizawaData(jrhLine, dataFile);
                    }
                case JrhLine.Hakodate_Otaru:
                    {
                        dataFile = dataFile ?? await GetTrainDatas(91, 85, 82, 76);
                        return await GetOtaruData(jrhLine, dataFile);
                    }
                case JrhLine.Chitose_Tomakomai:
                    {
                        dataFile = dataFile ?? await GetTrainDatas(91, 244, 245, 215);
                        return await GetChitoseData(jrhLine, dataFile);
                    }
                case JrhLine.Sassyo:
                    {
                        dataFile = dataFile ?? await GetTrainDatas(91, 259, 261, 262);
                        return await GetSassyoData(jrhLine, dataFile);
                    }
                case JrhLine.Sekisyo:
                    {
                        dataFile = dataFile ?? await GetTrainDatas(245, 220, 288, 297, 325);
                        return await GetSekisyoData(jrhLine, dataFile);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        /*
         * 函館線　岩見沢方面の位置計算
         * ・札幌方面はすべて札幌駅到着データで計算可能
         * ・岩見沢方面は江別止まりを考慮し、札幌-江別と江別-岩見沢で別に計算
         * ・ただし特急列車は江別を通過するので岩見沢のデータを利用する
         */
        private static async Task<(TrainPositionData, TrainPositionData)> GetIwamizawaData(JrhLine jrhLine, Dictionary<int, TrainDataFile> dataFile)
        {
            var sapporoDatas = GetPosition(dataFile[91].ArrivalTrainDatas[JrhDestType.Hakodate_Iwamizawa], jrhLine);

            var ebetsuDatas = GetPosition(dataFile[99].ArrivalTrainDatas[JrhDestType.Sapporo_Hakodate], jrhLine)
                .Where(tuple => tuple.position >= 0 && tuple.position <= 15);

            var iwamizawaDatas = GetPosition(dataFile[103].ArrivalTrainDatas[JrhDestType.Sapporo_Hakodate], jrhLine)
                .Where(tuple => (tuple.position >= 16 && tuple.position <= 24) || tuple.trainData.Name.TrainType == TrainData.TrainTypes.Ltd_Exp);

            var sapporoArrData = new TrainPositionData(jrhLine, sapporoDatas);
            var sapporoDepData = new TrainPositionData(jrhLine, ebetsuDatas, iwamizawaDatas);
            return (sapporoArrData, sapporoDepData);
        }

        /*
         * 函館線　小樽方面の位置計算
         * ・札幌方面はすべて札幌到着データを利用。
         * ・小樽方面のうち…
         * 　　札幌-手稲は手稲到着データ利用
         * 　　ほしみ到着データ(普通列車)のうち手稲-ほしみ間にある列車は反映
         *    小樽到着データはほしみ-小樽間の列車及び、ほしみ通過の快速列車については手稲-小樽間で利用。
         */
        private static async Task<(TrainPositionData, TrainPositionData)> GetOtaruData(JrhLine jrhLine, Dictionary<int, TrainDataFile> dataFile)
        {
            var sapporoData = GetPosition(dataFile[91].ArrivalTrainDatas[JrhDestType.Hakodate_Otaru], jrhLine);

            var teineData = GetPosition(dataFile[85].ArrivalTrainDatas[JrhDestType.Sapporo_Hakodate], jrhLine)
                .Where(tuple => tuple.position >= 0 && tuple.position <= 11);

            var hoshimiData = GetPosition(dataFile[82].ArrivalTrainDatas[JrhDestType.Sapporo_Hakodate], jrhLine)
                .Where(tuple => tuple.position >= 12 && tuple.position <= 17);

            var otaruData = GetPosition(dataFile[76].ArrivalTrainDatas[JrhDestType.Sapporo_Hakodate], jrhLine)
                .Where(tuple => (tuple.position >= 18 && tuple.position <= 28)
                || (tuple.trainData.Name.GetTypesByStation(StationReader.GetStationByName("ほしみ")) == TrainData.TrainTypes.Rapid && tuple.position >= 12 && tuple.position <= 28));

            var sapporoArrData = new TrainPositionData(jrhLine, new[] { sapporoData });
            var sapporoDepData = new TrainPositionData(jrhLine, new[] { teineData, hoshimiData, otaruData });
            return (sapporoArrData, sapporoDepData);
        }

        /*
         * 千歳線の位置計算
         * ・札幌方面はすべて札幌到着データ利用。
         * ・札幌-千歳間は特急以外反映。
         * ・札幌-南千歳は全反映。札幌-南千歳の特急も反映。
         * ・南千歳-苫小牧も全反映。
         */
        private static async Task<(TrainPositionData, TrainPositionData)> GetChitoseData(JrhLine jrhLine, Dictionary<int, TrainDataFile> dataFile)
        {
            var sapporoData = GetPosition(dataFile[91].ArrivalTrainDatas[JrhDestType.Chitose_Chitose], jrhLine);
            var sapporoRapidData = GetPosition(dataFile[91].ArrivalTrainDatas[JrhDestType.Chitose_Rapid_AP], jrhLine);

            var chitoseData = GetPosition(dataFile[244].ArrivalTrainDatas[JrhDestType.Sapporo_Chitose], jrhLine)
                .Where(tuple => tuple.position >= 0 && tuple.position <= 23 && tuple.trainData.Name.TrainType != TrainData.TrainTypes.Ltd_Exp);

            var minamiChitoseData = GetPosition(dataFile[245].ArrivalTrainDatas[JrhDestType.Chitose_LocalRapid_Sapporo], jrhLine)
                .Where(tuple => tuple.position >= 24 && tuple.position <= 25);

            var minamiChitoseExpData = GetPosition(dataFile[245].ArrivalTrainDatas[JrhDestType.Chitose_LimExp_Sapporo], jrhLine);

            var TomakomaiData = GetPosition(dataFile[215].ArrivalTrainDatas[JrhDestType.Sapporo_Chitose], jrhLine)
                .Where(tuple => tuple.position >= 26 && tuple.position <= 30);

            var sapporoArrData = new TrainPositionData(jrhLine, sapporoData, sapporoRapidData);
            var sapporoDepData = new TrainPositionData(jrhLine, chitoseData, minamiChitoseData, minamiChitoseExpData, TomakomaiData);
            return (sapporoArrData, sapporoDepData);
        }

        /*
         * 札沼線の位置計算
         * ・札幌到着はすべて反映。
         * ・あいの里公園、石狩当別、医療大学の札幌方面からの列車は順番に反映。
         * ・医療大-当別間の、月形方面からの列車を反映する為に当別到着のデータから月形方面のデータも反映。
         */
        private static async Task<(TrainPositionData, TrainPositionData)> GetSassyoData(JrhLine jrhLine, Dictionary<int, TrainDataFile> dataFile)
        {
            var sapporoData = GetPosition(dataFile[91].ArrivalTrainDatas[JrhDestType.Sassyo_IshikariTobetsu], jrhLine);

            var koenData = GetPosition(dataFile[259].ArrivalTrainDatas[JrhDestType.Sapporo_Sassyo], jrhLine)
                .Where(tuple => tuple.position >= 0 && tuple.position <= 19);

            var tobetsuData = GetPosition(dataFile[261].ArrivalTrainDatas[JrhDestType.Sapporo_Sassyo], jrhLine)
                .Where(tuple => tuple.position >= 20 && tuple.position <= 23);

            var tobetsuTsukigataData = GetPosition(dataFile[261].ArrivalTrainDatas[JrhDestType.Sassyo_Urausu], jrhLine);

            var iryodaiData = GetPosition(dataFile[262].ArrivalTrainDatas[JrhDestType.Sapporo_Sassyo], jrhLine)
                .Where(tuple => tuple.position >= 24);

            var sapporoArrData = new TrainPositionData(jrhLine, sapporoData, tobetsuTsukigataData);
            var sapporoDepData = new TrainPositionData(jrhLine, koenData, tobetsuData, iryodaiData);
            return (sapporoArrData, sapporoDepData);
        }

        private static async Task<(TrainPositionData, TrainPositionData)> GetSekisyoData(JrhLine jrhLine, Dictionary<int, TrainDataFile> dataFile)
        {
            var minamiChitoseData = GetPosition(dataFile[245].ArrivalTrainDatas[JrhDestType.Sekisyo_Oiwake_Obihiro], jrhLine);

            var oiwakeData = GetPosition(dataFile[220].ArrivalTrainDatas[JrhDestType.Sekisyo_MinamiChitose_Sapporo], jrhLine)
                .Where(tuple => tuple.trainData.Name.TrainType == TrainData.TrainTypes.Local);

            var shinyubariData = GetPosition(dataFile[288].ArrivalTrainDatas[JrhDestType.Sekisyo_Shiyubari], jrhLine)
                .Where(tuple => tuple.trainData.Name.TrainType == TrainData.TrainTypes.Local && tuple.position >= 6);

            var tomamuData = GetPosition(dataFile[297].ArrivalTrainDatas[JrhDestType.Sekisyo_Oiwake_Sapporo], jrhLine)
                .Where(tuple => tuple.trainData.Destination.StationID == 297);

            var shintokuData = GetPosition(dataFile[325].ArrivalTrainDatas[JrhDestType.Sekisyo_MinamiChitose_Sapporo], jrhLine);

            var arrData = new TrainPositionData(jrhLine, minamiChitoseData);
            var depData = new TrainPositionData(jrhLine, oiwakeData, shinyubariData, tomamuData, shintokuData);

            return (arrData, depData);
        }


        private static async Task<Dictionary<int, TrainDataFile>> GetTrainDatas(params int[] id)
        {
            var result = new Dictionary<int, TrainDataFile>();

            var stations = id.Select(n => StationReader.GetStationById(n));
            var inAreaStaion = stations.Where(std => std.StationArea == 0).ToArray();
            var outAreaStation = stations.Where(std => std.StationArea != 0);

            var count = 0;
            if (inAreaStaion.Any())
            {
                TrainDataFile[] array = default;
                do
                {
                    array = await Task.WhenAll(inAreaStaion.Select(n => TrainInfoReader.GetTrainDataAsync(n.StationID)));
                    for (var i = 0; i < array.Length; i++)
                    {
                        result.Add(inAreaStaion[i].StationID, array[i]);
                    }

                    count++;
                    if (count > 3)
                    {
                        throw new TrainDataTimeSyncException(result, "時刻が同期できません");
                    }

                    if (count > 1)
                    {
                        Debuggers.LogWriter.WriteLog($"列車位置計算用の列車情報の時刻同期に失敗...再試行します");
                    }
                }
                while (!CheckTrainDataFiles(array));
            }

            if (outAreaStation.Any())
            {
                var need = outAreaStation.Select(std => std.StationArea).Distinct().ToArray();
                var array = await Task.WhenAll(need.Select(n => TrainInfoReader.GetSpecialTrainDatasAsync(n)));

                foreach (var station in outAreaStation)
                {
                    var index = Array.IndexOf(need, station.StationArea);
                    result.Add(station.StationID, array[index].First(td => td.Station == station));
                }
            }

            return result;
        }

        private static async Task<Dictionary<int, TrainDataFile>> GetTrainDatas(IEnumerable<TrainDataFile> trainDataFiles, params int[] id)
        {
            var result = new Dictionary<int, TrainDataFile>();

            var stations = id.Select(n => StationReader.GetStationById(n));

            return await Task.Run(() =>
            {
                var keyValuePairs = new Dictionary<int, TrainDataFile>();
                foreach (var n in id)
                {
                    keyValuePairs.Add(n, trainDataFiles.First(tdf => tdf.Station.StationID == n));
                }
                return keyValuePairs;
            });
        }

        private static bool CheckTrainDataFiles(IEnumerable<TrainDataFile> trainDataFiles)
        {
            DateTime dateTime = default;
            var IsFirst = true;

            foreach (var dataFile in trainDataFiles)
            {
                if (IsFirst)
                {
                    dateTime = dataFile.GetedDateTime;
                    IsFirst = false;
                }
                else
                {
                    if (dateTime != dataFile.GetedDateTime)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static IEnumerable<(TrainData trainData, int position)> GetPosition(IEnumerable<TrainData> trainDatas, JrhLine jrhLine)
        {
            foreach (var trainData in trainDatas)
            {
                var pos = trainData.NowPosition?.GetPositionCode(jrhLine) ?? -1;
                if (pos == -1)
                {
                    continue;
                }
                else
                {
                    yield return (trainData, pos);
                }
            }
        }


        [Serializable]
        public class TrainDataTimeSyncException : Exception
        {
            public Dictionary<int, TrainDataFile> TrainDataFiles { get; private set; }
            public TrainDataTimeSyncException(Dictionary<int, TrainDataFile> trainDataFiles) => TrainDataFiles = trainDataFiles;

            public TrainDataTimeSyncException(Dictionary<int, TrainDataFile> trainDataFiles, string message) : base(message) => TrainDataFiles = trainDataFiles;

            public TrainDataTimeSyncException(Dictionary<int, TrainDataFile> trainDataFiles, string message, Exception inner) : base(message, inner) => TrainDataFiles = trainDataFiles;

            protected TrainDataTimeSyncException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
