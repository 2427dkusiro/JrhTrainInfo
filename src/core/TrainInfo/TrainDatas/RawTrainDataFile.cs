using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrainInfo.TrainDatas
{
    /// <summary>
    /// Webから取得された運行情報のファイルを表します。
    /// </summary>
    internal class RawTrainDataFile
    {
        public RawTrainDataFile()
        {

        }

        /// <summary>
        /// 駅 ID を取得または設定します。
        /// </summary>
        public int StationId { get; set; }

        /// <summary>
        /// 出発情報の取得日時を取得または設定します。
        /// </summary>
        [JsonProperty("departures_datetime")]
        public long Departures_datetime { get; set; }

        /// <summary>
        /// 出発列車と方面を表す <see cref="Dictionary{TKey, TValue}"/> を取得または設定します
        /// </summary>
        [JsonProperty("departures")]
        public Dictionary<string, RawTrainData[]> Departures { get; set; }

        /// <summary>
        /// 到着情報の取得日時を取得または設定します。
        /// </summary>
        [JsonProperty("arrivals_datetime")]
        public long Arrivals_datetime { get; set; }

        /// <summary>
        /// 到着列車と方面を表す <see cref="Dictionary{TKey, TValue}"/> を取得または設定します
        /// </summary>
        [JsonProperty("arrivals")]
        public Dictionary<string, RawTrainData[]> Arrivals { get; set; }
    }
}
