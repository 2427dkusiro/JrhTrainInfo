using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TrainInfo.Stations
{
#pragma warning disable CS0661 // 型は演算子 == または演算子 != を定義しますが、Object.GetHashCode() をオーバーライドしません
    /// <summary>
    /// 駅を表します。
    /// </summary>
    public class Station : IComparable
#pragma warning restore CS0661 // 型は演算子 == または演算子 != を定義しますが、Object.GetHashCode() をオーバーライドしません
    {
        /// <summary>
        /// <see cref="Station"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException">引数がサポートされない組み合わせのときにスローされる例外。</exception>
        [Obsolete("このコンストラクタは将来的に廃止される可能性があります。")]
        internal Station(int stationID, string name)
        {
            StationId = stationID;
            Name = name;

            Position = new Dictionary<JrhLine, int>();
        }

        /// <summary>
        /// <see cref="Station"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="name"></param>
        /// <param name="hiraName"></param>
        /// <param name="kataName"></param>
        /// <param name="englishName"></param>
        /// <param name="isEnd"></param>
        /// <param name="jrhLine"></param>
        /// <param name="position"></param>
        public Station(int stationID, string name, string hiraName, string kataName, string englishName, int stationArea, bool isEnd, JrhLine jrhLine, int position)
        {
            StationId = stationID;
            Name = name;
            HiraName = hiraName;
            KataName = kataName;
            EnglishName = englishName;

            StationArea = stationArea;

            Position = new Dictionary<JrhLine, int>
            {
                { jrhLine, position }
            };
            IsEndStation = isEnd;
        }


        /// <summary>
        /// <see cref="Station"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="name"></param>
        /// <param name="hiraName"></param>
        /// <param name="kataName"></param>
        /// <param name="englishName"></param>
        /// <param name="isEnd"></param>
        /// <param name="position"></param>
        [JsonConstructor]
        public Station(int stationID, string name, string hiraName, string kataName, string englishName, bool isEnd, int stationArea, Dictionary<JrhLine, int> position)
        {
            StationId = stationID;
            Name = name;
            HiraName = hiraName;
            KataName = kataName;
            EnglishName = englishName;
            StationArea = stationArea;
            Position = position;
            IsEndStation = isEnd;
        }



        /// <summary>
        /// 情報取得に必要な駅IDを取得します。
        /// </summary>
        [JsonProperty("ID")]
        public int StationId { get; private set; }

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

        /// <summary>
        /// 駅の存在するエリアを表す値を取得します。札幌圏エリアの場合0です。
        /// </summary>
        [JsonProperty("stationArea")]
        public int StationArea { get; private set; }

        /// <summary>
        /// 路線と位置の組を取得または設定します。
        /// </summary>
        [JsonProperty("Position")]
        public Dictionary<JrhLine, int> Position { get; private set; }

        /// <summary>
        /// 終着列車が存在する駅かどうかを取得します。
        /// </summary>
        [JsonProperty("IsEndStation")]
        public bool IsEndStation { get; private set; }

        /// <summary>
        /// 4文字以下の短い駅名を返します
        /// </summary>
        /// <returns></returns>
        public string GetShortName()
        {
            if (Name.Length < 5)
            {
                return Name;
            }
            else
            {
                switch (Name)
                {
                    case "北海道医療大学":
                        return "医療大";
                    case "あいの里公園":
                        return "あ里公園";
                    case "サッポロビール庭園":
                        return "札ビ庭園";
                    case "新千歳空港":
                        return "千歳空港";
                    default:
                        return Name;
                }
            }
        }

        /// <summary>
        /// 2つの駅が同じ駅を表すかどうか判断します。
        /// </summary>
        /// <param name="a">比較元の駅。</param>
        /// <param name="b">比較対象の駅。</param>
        /// <returns></returns>
        public static bool operator ==(Station a, Station b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }
            else
            {
                if (a.StationId == -1 || a.StationId == -1)
                {
                    if (a.Name == b.Name)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return a.StationId == b.StationId;
                }
            }
        }

        /// <summary>
        /// 2つの駅が異なる駅を表すかどうか判断します。
        /// </summary>
        /// <param name="a">比較元の駅。</param>
        /// <param name="b">比較対象の駅。</param>
        /// <returns></returns>
        public static bool operator !=(Station a, Station b)
        {
            return !(a == b);
        }

        /// <summary>
        /// ２つの駅が同じ駅かどうか判断します。
        /// </summary>
        /// <param name="obj">比較対象の駅。</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"><paramref name="obj"/>が<c>Station</c>でない場合にスローされる例外。</exception>
        public int CompareTo(object obj)
        {
            if (obj is Station a)
            {
                return StationId.CompareTo(a.StationId);
            }
            else
            {
                throw new NotSupportedException("比較対象の型がサポートされない型です。");
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
