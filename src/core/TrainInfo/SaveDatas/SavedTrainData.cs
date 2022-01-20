using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using TrainInfo;
using TrainInfo.Stations;

namespace TrainInfo.SaveDatas
{
    /// <summary>
    /// 解析済み列車データを表現する、シリアライズ時の容量が少なくなるよう設計されたクラスです。
    /// </summary>
    public class AnalyzedTrainData
    {
        public AnalyzedTrainData()
        {

        }

        public static readonly Version DataVersion = new Version(1, 0, 0, 0);

        #region TrainName 構築用
        [JsonProperty("tt")]
        public TrainData.TrainTypes TrainTypes { get; set; }
        [JsonProperty("na")]
        public string Name { get; set; }
        [JsonProperty("nu")]
        public int? Number { get; set; }
        [JsonProperty("st")]
        public TrainData.TrainTypes? SubTrainTypes { get; set; }
        [JsonProperty("ss")]
        public string SubTypeStart { get; set; }
        [JsonProperty("se")]
        public string SubTypeEnd { get; set; }
        #endregion

        [JsonProperty("ti")]
        public DateTime Time { get; set; }

        [JsonProperty("ds")]
        public string DepartureStation { get; set; }

        [JsonProperty("at")]
        public TrainData.ArrivalTypes ArrivalType { get; set; }

        [JsonProperty("de")]
        public string Destination { get; set; }

        #region TrainCondition 構築用プロパティ
        [JsonProperty("tc")]
        public TrainData.TrainConditions TrainCondition { get; set; }
        [JsonProperty("rs")]
        public string SuspendRangeStart { get; set; }
        [JsonProperty("re")]
        public string SuspendRangeEnd { get; set; }
        [JsonProperty("dn")]
        public int? DelayTimeMin { get; set; }
        [JsonProperty("dx")]
        public int? DelayTimeMax { get; set; }
        #endregion

        [JsonProperty("ps")]
        public string NowPositionStart { get; set; }

        [JsonProperty("pe")]
        public string NowPositionEnd { get; set; }

        public static AnalyzedTrainData FromTrainData(TrainData trainData)
        {
            return new AnalyzedTrainData()
            {
                TrainTypes = trainData.Name.TrainType,
                Name = trainData.Name.Name,
                Number = trainData.Name.Number,
                SubTrainTypes = trainData.Name.SubTrainType,
                SubTypeStart = ToStationString(trainData.Name.SubTrainTypeRange?.StartPos),
                SubTypeEnd = ToStationString(trainData.Name.SubTrainTypeRange?.EndPos),
                Time = trainData.Time,
                DepartureStation = ToStationString(trainData.DepartureStation),
                ArrivalType = trainData.ArrivalType,
                Destination = ToStationString(trainData.Destination),
                TrainCondition = trainData.Condition.Condition,
                SuspendRangeStart = ToStationString(trainData.Condition.SuspendRange?.StartPos),
                SuspendRangeEnd = ToStationString(trainData.Condition.SuspendRange?.EndPos),
                DelayTimeMin = trainData.Condition.DelayTimeMin,
                DelayTimeMax = trainData.Condition.DelayTimeMax,
            };
        }

        public TrainData ToTrainData()
        {
            LineRange SubRange;
            if (SubTypeStart is null || SubTypeEnd is null)
            {
                SubRange = null;
            }
            else
            {
                SubRange = new LineRange(FromStationString(SubTypeStart), FromStationString(SubTypeEnd));
            }
            var name = new TrainData.TrainName(TrainTypes, Name, Number, SubTrainTypes, SubRange);

            LineRange suspendRange;
            if (SuspendRangeStart is null || SuspendRangeEnd is null)
            {
                suspendRange = null;
            }
            else
            {
                suspendRange = new LineRange(FromStationString(SuspendRangeStart), FromStationString(SuspendRangeEnd));
            }

            TrainData.TrainCondition condition;
            if (DelayTimeMin is int min && DelayTimeMax is int max)
            {
                condition = new TrainData.TrainCondition(TrainCondition, max, min);
            }
            else
            {
                condition = new TrainData.TrainCondition(TrainCondition, suspendRange);
            }

            LineRange position;
            if (NowPositionStart is null || NowPositionEnd is null)
            {
                position = null;
            }
            else
            {
                position = new LineRange(FromStationString(NowPositionStart), FromStationString(NowPositionEnd));
            }

            return new TrainData(name, Time, FromStationString(DepartureStation), ArrivalType, FromStationString(Destination), condition, position);
        }

        private static string ToStationString(Station station)
        {
            if (station is null)
            {
                return null;
            }
            else if (station.StationId == -1)
            {
                return station.Name;
            }
            else
            {
                return station.StationId.ToString();
            }
        }

        private static Station FromStationString(string str)
        {
            if (int.TryParse(str, out var n))
            {
                return StationReader.GetStationById(n);
            }
            else
            {
                if (str is null)
                {
                    return null;
                }
                else
                {
                    return StationReader.GetOrCreateStationByName(str);
                }
            }
        }

    }

    /// <summary>
    /// 解析済み列車ファイルを表現する、シリアライズ時の容量が少なくなるよう設計されたクラスです。
    /// </summary>
    public class AnalyzedTrainFile
    {
        public AnalyzedTrainFile()
        {

        }

        [JsonProperty("st")]
        public int Station { get; set; }

        [JsonProperty("dt")]
        public DateTime GetedDateTime { get; set; }

        [JsonProperty("dp")]
        public Dictionary<JrhDestType, AnalyzedTrainData[]> DepartureTrainDatas { get; set; }

        [JsonProperty("ar")]
        /// <summary>
        /// 到着列車の情報を取得します。
        /// </summary>
        public Dictionary<JrhDestType, AnalyzedTrainData[]> ArrivalTrainDatas { get; set; }


        public static AnalyzedTrainFile FromTrainDataFile(TrainDataFile trainDataFile)
        {
            var result = new AnalyzedTrainFile
            {
                DepartureTrainDatas = new Dictionary<JrhDestType, AnalyzedTrainData[]>(),
                ArrivalTrainDatas = new Dictionary<JrhDestType, AnalyzedTrainData[]>()
            };

            foreach (var kvp in trainDataFile.DepartureTrainDatas)
            {
                result.DepartureTrainDatas.Add(kvp.Key, kvp.Value.Select(td => AnalyzedTrainData.FromTrainData(td)).ToArray());
            }
            foreach (var kvp in trainDataFile.ArrivalTrainDatas)
            {
                result.ArrivalTrainDatas.Add(kvp.Key, kvp.Value.Select(td => AnalyzedTrainData.FromTrainData(td)).ToArray());
            }

            result.Station = trainDataFile.Station.StationId;
            result.GetedDateTime = trainDataFile.GetedDateTime;

            return result;
        }

        public TrainDataFile ToTrainDataFile()
        {
            var dep = new Dictionary<JrhDestType, IReadOnlyList<TrainData>>();
            var arr = new Dictionary<JrhDestType, IReadOnlyList<TrainData>>();

            foreach (var kvp in DepartureTrainDatas)
            {
                dep.Add(kvp.Key, kvp.Value.Select(atd => atd.ToTrainData()).ToList());
            }
            foreach (var kvp in ArrivalTrainDatas)
            {
                arr.Add(kvp.Key, kvp.Value.Select(atd => atd.ToTrainData()).ToList());
            }
            return new TrainDataFile(StationReader.GetStationById(Station), GetedDateTime, dep, arr);
        }
    }
}
