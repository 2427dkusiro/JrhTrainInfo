using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using TrainInfo.Stations;

namespace TrainInfo
{
    /// <summary>
    /// 解析済みの列車データセットを表します。
    /// </summary>
    public class TrainDataFile
    {
        internal TrainDataFile(Station station, DateTime getedDateTime)
        {
            Station = station;
            GetedDateTime = getedDateTime;
        }

        /// <summary>
        /// <see cref="TrainDataFile"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="station">取得対象の駅</param>
        /// <param name="getedDateTime">取得日時</param>
        /// <param name="departureTrainDatas">出発列車の方面別の列車データを表す<see cref="Dictionary{TKey, TValue}"/></param>
        /// <param name="arrivalTrainDatas">到着列車の方面別の列車データを表す<see cref="Dictionary{TKey, TValue}"/></param>
        public TrainDataFile(Station station, DateTime getedDateTime, Dictionary<JrhDestType, IReadOnlyList<TrainData>> departureTrainDatas, Dictionary<JrhDestType, IReadOnlyList<TrainData>> arrivalTrainDatas)
        {
            Station = station;
            GetedDateTime = getedDateTime;
            DepartureTrainDatas = new Dictionary<JrhDestType, IReadOnlyList<TrainData>>(departureTrainDatas);
            ArrivalTrainDatas = new Dictionary<JrhDestType, IReadOnlyList<TrainData>>(arrivalTrainDatas);
        }

        /// <summary>
        /// データの取得対象の駅を取得します。
        /// </summary>
        public Station Station { get; private set; }

        /// <summary>
        /// 取得日時を取得します。
        /// </summary>
        public DateTime GetedDateTime { get; private set; }

        /// <summary>
        /// 出発列車の情報を取得します。
        /// </summary>
        public IReadOnlyDictionary<JrhDestType, IReadOnlyList<TrainData>> DepartureTrainDatas { get; private set; }

        /// <summary>
        /// 到着列車の情報を取得します。
        /// </summary>
        public IReadOnlyDictionary<JrhDestType, IReadOnlyList<TrainData>> ArrivalTrainDatas { get; private set; }

        public static TrainDataFile MargeTrainDataFile(params TrainDataFile[] source)
        {
            if (source.Length < 2)
            {
                throw new ArgumentException("結合する要素は2個以上必要です");
            }

            var station = source.First().Station;

            if (source.Any(tdf => tdf.Station != station))
            {
                throw new ArgumentException("結合対象に異なる駅が含まれています");
            }

            var newTime = DateTime.MinValue;
            var depDict = new Dictionary<JrhDestType, List<TrainData>>();
            var arrDict = new Dictionary<JrhDestType, List<TrainData>>();

            foreach (var trainDataFile in source)
            {
                if (newTime < trainDataFile.GetedDateTime)
                {
                    newTime = trainDataFile.GetedDateTime;
                }

                foreach (var kvp in trainDataFile.DepartureTrainDatas)
                {
                    if (depDict.TryGetValue(kvp.Key, out var list))
                    {
                        //要素追加(ただし重複回避)、nullに警戒
                        if (list is null)
                        {
                            list = new List<TrainData>(kvp.Value);
                        }
                        else
                        {
                            foreach (var item in kvp.Value)
                            {
                                if (!list.Any(td => td == item))
                                {
                                    list.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        depDict.Add(kvp.Key, new List<TrainData>(kvp.Value));
                    }
                }
                foreach (var kvp in trainDataFile.ArrivalTrainDatas)
                {
                    if (arrDict.TryGetValue(kvp.Key, out var list))
                    {
                        //要素追加(ただし重複回避)、nullに警戒
                        if (list is null)
                        {
                            list = new List<TrainData>(kvp.Value);
                        }
                        else
                        {
                            foreach (var item in kvp.Value)
                            {
                                if (!list.Any(td => td == item))
                                {
                                    list.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        arrDict.Add(kvp.Key, new List<TrainData>(kvp.Value));
                    }
                }
            }

            return new TrainDataFile(station, newTime, ToReadOnlyListDict(depDict), ToReadOnlyListDict(arrDict));
        }

        private static Dictionary<Tkey, IReadOnlyList<Tlist>> ToReadOnlyListDict<Tkey, Tlist>(Dictionary<Tkey, List<Tlist>> dict)
        {
            var result = new Dictionary<Tkey, IReadOnlyList<Tlist>>();
            foreach (var kvp in dict)
            {
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }
    }
}
