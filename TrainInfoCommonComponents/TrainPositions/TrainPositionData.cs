using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrainInfo.Stations;

namespace TrainInfo.TrainPositions
{
    /// <summary>
    /// 列車の位置ごとに分類された情報を表します。
    /// </summary>
    public class TrainPositionData : IReadOnlyList<(LineRange, IReadOnlyList<TrainData>)>, IReadOnlyDictionary<LineRange, IReadOnlyList<TrainData>>
    {
        /// <summary>
        /// このクラスが表す列車位置の対象路線を取得します。
        /// </summary>
        public JrhLine JrhLine { get; private set; }

        private readonly List<LineRange> lineRanges;

        /// <summary>
        /// 位置ごとに分類された列車情報を取得します。
        /// </summary>
        private readonly List<List<TrainData>> TrainDatas;

        /// <summary>
        /// <see cref="TrainPositionData"/>クラスの新しいインスタンスを、列車をマッピングして初期化します。
        /// </summary>
        /// <param name="jrhLine"></param>
        /// <param name="trainDatas"></param>
        public TrainPositionData(JrhLine jrhLine, params IEnumerable<(TrainData td, int position)>[] trainDatas)
        {
            JrhLine = jrhLine;
            lineRanges = LineRange.CreateLineRanges(LineDataReader.GetStations(jrhLine)).ToList();
            TrainDatas = lineRanges.Select(x => new List<TrainData>()).ToList();

            foreach (var tds in trainDatas)
            {
                foreach (var tuple in tds)
                {
                    TrainDatas[tuple.position].Add(tuple.td);
                }
            }
        }

        /// <summary>
        /// 駅を指定して走行中の列車を取得します。
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public IReadOnlyList<TrainData> GetTrainDatasByStation(Station station)
        {
            var lineRange = new LineRange(station, station);
            return this[lineRange];
        }

        /// <summary>
        /// 路線区間の総数を取得します。
        /// </summary>
        public int Count => TrainDatas.Count;

        /// <summary>
        /// この位置情報を構成する<see cref="LineRange"/>を列挙します。
        /// </summary>
        public IEnumerable<LineRange> Keys => lineRanges;

        /// <summary>
        /// この位置情報に含まれる、位置別にまとめられた列車データを列挙します。
        /// </summary>
        public IEnumerable<IReadOnlyList<TrainData>> Values => TrainDatas;

        /// <summary>
        /// 指定した路線区間上に存在する列車を取得します。列車がない場合は空の<see cref="IReadOnlyList{T}"/>を返します。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IReadOnlyList<TrainData> this[LineRange key] => TrainDatas[lineRanges.IndexOf(key)];

        /// <summary>
        /// 指定した路線区間番号上に存在する列車を取得します。列車がない場合は空の<see cref="IReadOnlyList{T}"/>を返します。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public (LineRange, IReadOnlyList<TrainData>) this[int index] => (lineRanges[index], TrainDatas[index]);


        /// <summary>
        /// 列車データを列挙するための列挙子を取得します。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<(LineRange, IReadOnlyList<TrainData>)> GetEnumerator()
        {
            return GetKeyValuesTuple().GetEnumerator();
        }

        /// <summary>
        /// 列挙子を取得します。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return TrainDatas.GetEnumerator();
        }

        /// <summary>
        /// この位置情報に指定の路線区間が含まれるかどうか調べます。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(LineRange key)
        {
            return lineRanges.Contains(key);
        }

        /// <summary>
        /// 指定の<see cref="LineRange"/>上に存在する列車のデータの取得を試みます。
        /// 列車がない場合は空の<see cref="IReadOnlyList{T}"/>を返します。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">路線区間上の列車データ。このパラメータは初期化せずに渡されます。</param>
        /// <returns>この位置情報に指定の路線区間が存在するかどうかを表す値。</returns>
        public bool TryGetValue(LineRange key, out IReadOnlyList<TrainData> value)
        {
            var index = lineRanges.IndexOf(key);
            if (index == -1)
            {
                value = null;
                return false;
            }
            else
            {
                value = TrainDatas[index];
                return true;
            }
        }

        IEnumerator<KeyValuePair<LineRange, IReadOnlyList<TrainData>>> IEnumerable<KeyValuePair<LineRange, IReadOnlyList<TrainData>>>.GetEnumerator()
        {
            return GetKeyValues().GetEnumerator();
        }

        private IEnumerable<KeyValuePair<LineRange, IReadOnlyList<TrainData>>> GetKeyValues()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return new KeyValuePair<LineRange, IReadOnlyList<TrainData>>(lineRanges[i], TrainDatas[i]);
            }
        }

        private IEnumerable<(LineRange, IReadOnlyList<TrainData>)> GetKeyValuesTuple()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return (lineRanges[i], TrainDatas[i]);
            }
        }
    }
}
