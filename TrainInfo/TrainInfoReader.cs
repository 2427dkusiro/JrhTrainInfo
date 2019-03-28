using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using TrainInfo.Models;
using TrainInfo.Stations;
using TrainInfo.Spliters;

namespace TrainInfo
{
    /// <summary>
    /// 列車運行情報の取得を提供します。
    /// </summary>
    public static class TrainInfoReader
    {
        private static HttpClient httpClient;
        static TrainInfoReader()
        {
            httpClient = new HttpClient();
        }

        /// <summary>
        /// 駅 ID を指定して列車情報を取得します。
        /// </summary>
        /// <param name="id">取得対象の駅 ID。</param>
        /// <returns>解析済みの列車情報。</returns>
        public static async Task<TrainDataFile> GetTrainDataById(int id)
        {
            string data = await GetTrainDataJsonById(id);
            return await Task.Run(() =>
            {
                RawStationTrainData rawStationTrainData = DeserializeTrainInfo(data);
                return ParseRawStationTrainData(rawStationTrainData);
            });
        }

        public static async Task<TrainDataFile[]> GetSpecialTrainDataById(int id)
        {
            string data = await GetTrainDataJsonById(id);
            return await Task.Run(() =>
            {
                RawStationTrainData[] rawStationTrainData = DeserializeSpecialTrainInfo(data);
                return rawStationTrainData.Select(rstd => ParseRawStationTrainData(rstd)).ToArray();
            });
        }

        /// <summary>
        /// 駅IDを指定して列車情報の Json データを取得します。
        /// デバッグ等で生データが必要な場合のみこのメソッドを使用してください。
        /// 通常は <see cref="GetTrainDataById(int)"/> の呼び出しが推奨されます。
        /// </summary>
        /// <param name="id">取得対象の駅 ID。</param>
        /// <returns>列車情報を表す Json データ。</returns>
        public async static Task<string> GetTrainDataJsonById(int id)
        {
            const string baseUrl = @"http://traininfo.jrhokkaido.co.jp/json/services";

            string idString;
            if (id.ToString().Length == 2)
                idString = "0" + id.ToString();
            else
                idString = id.ToString();

            string result;
            using (WebClient webClient = new WebClient())
            {
                for (int i = 0; ; i++)
                {
                    try
                    {
                        result = await httpClient.GetStringAsync(new Uri($"{baseUrl}{idString}.json?_={GetUnixTime()}"));
                        break;
                    }
                    catch (HttpRequestException ex)
                    {
                        if (i < 3)
                        {
                            await Task.Delay(1000);
                            continue;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            return result;
        }

        private static DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private static ulong GetUnixTime()
        {
            var time = DateTime.Now.ToUniversalTime() - unixTime;
            return (ulong)time.TotalSeconds;
        }

        public async static Task<(TrainDataWithPosition arrData, TrainDataWithPosition depData)> GetTrainDataByLine(JrhLine jrhLine)
        {
            int rangeCount = LineDataReader.GetStations(jrhLine).Length;

            switch (jrhLine)
            {
                case JrhLine.Hakodate_Iwamizawa:
                    {
                        var dataFile = await Task.WhenAll(GetTrainDataById(91), GetTrainDataById(99), GetTrainDataById(103));
                        var sapporoDatas = dataFile[0].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Hakodate_Iwamizawa);
                        var ebetsuDatas = dataFile[1].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Hakodate)
                            .Where(td => td.position >= 1 && td.position <= 8.5m);
                        var iwamizawaDatas = dataFile[2].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Hakodate)
                            .Where(td => (td.position >= 9 && td.position <= 13) || td.trainData.Name.TrainType == TrainTypes.Ltd_Exp);

                        var sapporoArrData = MapTrainDatas(new IEnumerable<(TrainData, decimal)>[] { sapporoDatas }, rangeCount);
                        var sapporoDepData = MapTrainDatas(new IEnumerable<(TrainData, decimal)>[] { ebetsuDatas, iwamizawaDatas }, rangeCount);
                        return (sapporoArrData, sapporoDepData);
                    }
                case JrhLine.Hakodate_Otaru:
                    {

                        var dataFile = await Task.WhenAll(GetTrainDataById(91), GetTrainDataById(85), GetTrainDataById(82), GetTrainDataById(76));
                        var sapporoData = dataFile[0].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Hakodate_Otaru);
                        var teineData = dataFile[1].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Hakodate)
                            .Where(td => td.position >= 1 && td.position <= 6.5m);
                        var hoshimiData = dataFile[2].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Hakodate)
                            .Where(td => td.position >= 7 && td.position <= 9.5m);
                        var otaruData = dataFile[3].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Hakodate)
                            .Where(td => (td.position >= 10 && td.position <= 15)
                            || td.trainData.Name.GetTypesByStation(StationReader.GetStationById(82)) == TrainTypes.Rapid && td.position >= 7 && td.position <= 15);

                        var sapporoArrData = MapTrainDatas(new IEnumerable<(TrainData, decimal)>[] { sapporoData }, rangeCount);
                        var sapporoDepData = MapTrainDatas(new IEnumerable<(TrainData, decimal)>[] { teineData, hoshimiData, otaruData }, rangeCount);
                        return (sapporoArrData, sapporoDepData);
                    }
                case JrhLine.Chitose_Tomakomai:
                    {
                        var dataFile = await Task.WhenAll(GetTrainDataById(91), GetTrainDataById(244), GetTrainDataById(255), GetTrainDataById(215));
                        var sapporoData = dataFile[0].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Chitose_Chitose);
                        var sapporoRapidData = dataFile[0].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Chitose_Rapid_AP);
                        var chitoseData = dataFile[1].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Chitose)
                            .Where(td => td.position >= 1 && td.position <= 12.5m && td.trainData.Name.TrainType != TrainTypes.Ltd_Exp);
                        var minamiChitoseData = dataFile[2].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Chitose_LocalRapid_Sapporo)
                            .Where(td => td.position >= 13 && td.position <= 13.5m);
                        var minamiChitoseExpData = dataFile[2].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Chitose_LimExp_Sapporo);
                        var TomakomaiData = dataFile[3].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Chitose)
                            .Where(td => td.position >= 14 && td.position <= 16);

                        var sapporoArrData = MapTrainDatas(new IEnumerable<(TrainData, decimal)>[] { sapporoData, sapporoRapidData }, rangeCount);
                        var sapporoDepData = MapTrainDatas(new IEnumerable<(TrainData, decimal)>[] { chitoseData, minamiChitoseData, minamiChitoseExpData, TomakomaiData }, rangeCount);
                        return (sapporoArrData, sapporoDepData);
                    }
                case JrhLine.Sassyo:
                    {
                        var dataFile = await Task.WhenAll(GetTrainDataById(91), GetTrainDataById(259), GetTrainDataById(261), GetTrainDataById(262));
                        var sapporoData = dataFile[0].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sassyo_IshikariTobetsu);
                        var koenData = dataFile[1].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Sassyo)
                            .Where(td => td.position <= 10.5m);

                        if (DateTime.Now.Month > 5)
                            throw new NotImplementedException("札沼線廃線に伴う更新が必要です");
                        var tobetsuData = dataFile[2].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Sassyo)
                            .Where(td => td.position >= 11 && td.position <= 12.5m);
                        var tobetsuTsukigataData = dataFile[2].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sassyo_Urausu);
                        var iryodaiData = dataFile[3].SearchArrivalTrainDataWithPosition(jrhLine, JrhDestType.Sapporo_Sassyo)
                            .Where(td => td.position >= 13);
                        var sapporoArrData = MapTrainDatas(new IEnumerable<(TrainData, decimal)>[] { sapporoData, tobetsuTsukigataData }, rangeCount);
                        var sapporoDepData = MapTrainDatas(new IEnumerable<(TrainData, decimal)>[] { koenData, tobetsuData, iryodaiData }, rangeCount);
                        return (sapporoArrData, sapporoDepData);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private static TrainDataWithPosition MapTrainDatas(IEnumerable<IEnumerable<(TrainData, decimal)>> trainDatas, int rangeLenght)
        {
            TrainDataWithPosition trainDataWithPosition = new TrainDataWithPosition(rangeLenght);

            foreach (var data in trainDatas)
            {
                foreach (var (trainData, position) in data)
                {
                    trainDataWithPosition[position].Add(trainData);
                }
            }
            return trainDataWithPosition;
        }

        /// <summary>
        /// 運行情報の Json データをデシリアライズします。
        /// </summary>
        /// <param name="trainInfoJson">列車情報の Json データ。</param>
        /// <returns>運行情報の生データ。</returns>
        public static RawStationTrainData DeserializeTrainInfo(string trainInfoJson)
        {
            JObject jObj = JObject.Parse(trainInfoJson);
            RawStationTrainData result = null;

            jObj = (JObject)jObj.Properties().First().Value;
            int id = int.Parse(jObj.Properties().First().Name);
            jObj = (JObject)jObj.Properties().First().Value;
            result = jObj.ToObject<RawStationTrainData>();
            result.StationId = id;

            if (result == null)
                throw new RawTrainDataFormatExeption("列車Jsonデータのデシリアライズに失敗しました");
            else
                return result;
        }

        /// <summary>
        /// 運行情報の Json データをデシリアライズします。
        /// </summary>
        /// <param name="trainInfoJson">列車情報の Json データ。</param>
        /// <returns>運行情報の生データ。</returns>
        public static RawStationTrainData[] DeserializeSpecialTrainInfo(string trainInfoJson)
        {
            JObject jObj = JObject.Parse(trainInfoJson);
            List<RawStationTrainData> result = new List<RawStationTrainData>();

            jObj = (JObject)jObj.Properties().First().Value;
            foreach (var dataObj in jObj)
            {
                var data = dataObj.Value.ToObject<RawStationTrainData>();
                data.StationId = int.Parse(dataObj.Key);
                result.Add(data);
            }

            if (!result.Any())
                throw new RawTrainDataFormatExeption("列車Jsonデータのデシリアライズに失敗しました");
            else
                return result.ToArray();
        }

        /// <summary>
        /// 運行情報の生データを解析します。
        /// </summary>
        /// <param name="data">運行情報の生データ。</param>
        /// <returns>解析された運行情報データ。</returns>
        public static TrainDataFile ParseRawStationTrainData(RawStationTrainData data)
        {
            TrainDataFile result = new TrainDataFile
            {
                GetedDateTime = DateTime.ParseExact(data.Departures_datetime.ToString(), "yyyyMMddHHmm", System.Globalization.DateTimeFormatInfo.InvariantInfo),
                Station = StationReader.GetStationById(data.StationId)
            };

            result.DepartureTrainDatas = ParseTrainDataSet(data.Departures, result.GetedDateTime.Hour).ToArray();
            result.ArrivalTrainDatas = ParseTrainDataSet(data.Arrivals, result.GetedDateTime.Hour).ToArray();
            return result;
        }

        private static IEnumerable<TrainDataSet> ParseTrainDataSet(Dictionary<string, RawTrainData[]> data, int hour)
        {
            foreach (var keyValue in data)
            {
                TrainDataSet trainDataSet = new TrainDataSet();

                string lineName = keyValue.Key;
                trainDataSet.DestType = LineTypeSpliter.GetDestType(lineName);

                List<TrainData> trainDatas = new List<TrainData>();
                foreach (var rawTrainData in keyValue.Value)
                {
                    TrainData trainData = new TrainData();
                    trainData.SetTrainName(rawTrainData.TrainName, rawTrainData.TrainType);
                    trainData.SetTime(rawTrainData.Time, hour);
                    trainData.SetDestination(rawTrainData.Destination);
                    trainData.SetCondition(rawTrainData.Status, rawTrainData.Addition, rawTrainData.NowPosition);
                    trainData.SetNowPosition(rawTrainData.NowPosition);
                    trainDatas.Add(trainData);
                }

                trainDataSet.TrainDatas = trainDatas.ToArray();
                yield return trainDataSet;
            }
        }
    }

    /// <summary>
    /// 未加工列車データの正式が正しくないときに発生する例外です。
    /// </summary>
    public class RawTrainDataFormatExeption : Exception
    {
        /// <summary>
        /// <see cref="RawTrainDataFormatExeption"/> クラスの新しいインスタンスを、規定のエラーメッセージで初期化します。
        /// </summary>
        public RawTrainDataFormatExeption() : base("未加工列車データの形式が正しくありません")
        {

        }

        /// <summary>
        /// <see cref="RawTrainDataFormatExeption"/> クラスの新しいインスタンスを、エラーメッセージを指定して初期化します。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        public RawTrainDataFormatExeption(string message) : base(message)
        {

        }

        /// <summary>
        /// <see cref="RawTrainDataFormatExeption"/> クラスの新しいインスタンスを、エラーメッセージと内部例外を指定して初期化します。
        /// </summary>
        /// <param name="message">エラーメッセージ。</param>
        /// <param name="exception">内部例外。</param>
        public RawTrainDataFormatExeption(string message, Exception exception) : base(message, exception)
        {

        }
    }

    /// <summary>
    /// 位置情報つきの列車情報を表します。
    /// </summary>
    public class TrainDataWithPosition
    {
        private List<TrainData>[] TrainDatas;

        /// <summary>
        /// <see cref="TrainDataWithPosition" /> クラスの新しいインスタンスを駅数から初期化します。
        /// </summary>
        /// <param name="stationCount"></param>
        public TrainDataWithPosition(int stationCount)
        {
            TrainDatas = new List<TrainData>[stationCount * 2 - 1].Select(tdl => new List<TrainData>()).ToArray();
        }

        public List<TrainData> this[decimal positionID]
        {
            get
            {
                return TrainDatas[(int)((positionID - 1) * 2)];
            }
        }
    }
}
