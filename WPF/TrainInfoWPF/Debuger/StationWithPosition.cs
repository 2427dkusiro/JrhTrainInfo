using System;
using TrainInfo.Stations;
using Newtonsoft.Json;

namespace TrainInfoWPF.Debuger
{
    /// <summary>
    /// 駅を表します。
    /// </summary>
    class StationWithPosition : IComparable
    {
        /// <summary>
        /// <see cref="Station"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="name"></param>
        /// <param name="hiraName"></param>
        /// <param name="kataName"></param>
        /// <param name="englishName"></param>
        public StationWithPosition(int stationID, string name, string hiraName = null, string kataName = null, string englishName = null)
        {
            StationID = stationID;
            Name = name;
            HiraName = hiraName;
            KataName = kataName;
            EnglishName = englishName;
        }

        /// <summary>
        /// 情報取得に必要な駅IDを取得します。
        /// </summary>
        [JsonProperty("ID")]
        public int StationID { get; private set; }

        /// <summary>
        /// 漢字表記の駅名を取得します。
        /// </summary>
        [JsonProperty("value")]
        public string Name { get; private set; }

        /// <summary>
        /// 平仮名表記の駅名を取得します。
        /// </summary>
        [JsonProperty("hira")]
        public string HiraName { get; private set; }

        /// <summary>
        /// 片仮名表記の駅名を取得します。
        /// </summary>
        [JsonProperty("kata")]
        public string KataName { get; private set; }

        /// <summary>
        /// 英語表記の駅名を取得します。
        /// </summary>
        [JsonProperty("en")]
        public string EnglishName { get; private set; }

        [JsonProperty("position")]
        public int LinePosition { get; set; }

        [JsonProperty("need")]
        public bool IsEnd { get; set; }

        /// <summary>
        /// 2つの駅が同じ駅を表すかどうか判断します。
        /// </summary>
        /// <param name="a">比較元の駅。</param>
        /// <param name="b">比較対象の駅。</param>
        /// <returns></returns>
        public static bool operator ==(StationWithPosition a, StationWithPosition b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;
            else
            {
                if (a.StationID == -1 || a.StationID == -1)
                {
                    if (a.Name == b.Name)
                        return true;
                    else
                        return false;
                }
                else
                    return a.StationID == b.StationID;
            }
        }

        /// <summary>
        /// 2つの駅が異なる駅を表すかどうか判断します。
        /// </summary>
        /// <param name="a">比較元の駅。</param>
        /// <param name="b">比較対象の駅。</param>
        /// <returns></returns>
        public static bool operator !=(StationWithPosition a, StationWithPosition b)
        {
            return !(a == b);
        }

        public int CompareTo(object obj)
        {
            if (obj is Station a)
            {
                return this.StationID.CompareTo(a.StationID);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// このインスタンスを表す文字列を取得します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
