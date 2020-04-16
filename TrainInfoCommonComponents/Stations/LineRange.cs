using System.Collections.Generic;

namespace TrainInfo.Stations
{


#pragma warning disable CS0659 // 型は Object.Equals(object o) をオーバーライドしますが、Object.GetHashCode() をオーバーライドしません
#pragma warning disable CS0661 // 型は演算子 == または演算子 != を定義しますが、Object.GetHashCode() をオーバーライドしません
    /// <summary>
    /// 線路上の区間を表します。
    /// </summary>
    public class LineRange
#pragma warning restore CS0661 // 型は演算子 == または演算子 != を定義しますが、Object.GetHashCode() をオーバーライドしません
#pragma warning restore CS0659 // 型は Object.Equals(object o) をオーバーライドしますが、Object.GetHashCode() をオーバーライドしません
    {
        /// <summary>
        /// 区間の開始駅を取得または設定します。
        /// </summary>
        public Station StartPos { get; set; }

        /// <summary>
        /// 区間の終了駅を取得または設定します。
        /// </summary>
        public Station EndPos { get; set; }

        /// <summary>
        /// <see cref="LineRange"/> クラスの新しいインスタンスを開始地点と終了地点で初期化します。
        /// </summary>
        /// <param name="startPos">区間の開始駅。</param>
        /// <param name="endPos">区間の終了駅。</param>
        public LineRange(Station startPos, Station endPos)
        {
            StartPos = startPos;
            EndPos = endPos;
        }

        /// <summary>
        /// <see cref="LineRange"/> クラスの新しいインスタンスを開始地点と終了地点の駅名で初期化します。
        /// </summary>
        /// <param name="startPos">開始地点の駅名。</param>
        /// <param name="endPos">終了地点の駅名。</param>
        public LineRange(string startPos, string endPos)
        {
            StartPos = StationReader.GetOrCreateStationByName(startPos);
            EndPos = StationReader.GetOrCreateStationByName(endPos);
        }

        /// <summary>
        /// 指定の駅からなる路線区間を表すための複数の<see cref="LineRange"/>を取得します。
        /// </summary>
        /// <param name="stations"></param>
        /// <returns></returns>
        public static IEnumerable<LineRange> CreateLineRanges(IEnumerable<Station> stations)
        {
            Station station = default;

            var IsFirst = true;
            foreach (var sta in stations)
            {
                if (IsFirst)
                {
                    station = sta;
                    yield return new LineRange(station, station);
                    IsFirst = false;
                }
                else
                {
                    yield return new LineRange(station, sta);
                    station = sta;
                    yield return new LineRange(station, station);
                }
            }
        }

        /*
        /// <summary>
        /// 現在の列車の位置を表す値を取得します。
        /// </summary>
        /// <param name="jrhLine">位置を取得する対象となる路線</param>
        /// <returns>札幌を起点(0)とする位置情報。指定路線上にない場合は<c>null</c>。</returns>
        public int? GetNowPosition(JrhLine jrhLine)
        {
            if (StartPos.Position.TryGetValue(jrhLine, out int startPos) && EndPos.Position.TryGetValue(jrhLine, out int endPos))
            {
                return startPos - 1 + (endPos - 1);
            }
            else
            {
                return null;
            }
        }
        */

        /// <summary>
        /// 指定した駅がこの区間に含まれているかどうか判断します。
        /// </summary>
        /// <param name="station">判定対象の駅。</param>
        /// <returns></returns>
        public bool Contains(Station station)
        {
            var pos_a = StartPos.Position;
            var pos_b = EndPos.Position;

            JrhLine? jrhLine = null;
            var start = 0;
            var end = 0;

            foreach (var pos in pos_a)
            {
                if (pos_b.ContainsKey(pos.Key))
                {
                    jrhLine = pos.Key;
                    start = pos.Value;
                    end = pos_b[pos.Key];
                }
            }

            if (jrhLine is JrhLine nouNull)
            {
                if (station.Position.TryGetValue(nouNull, out var pos))
                {
                    if (start < pos && pos < end)
                    {
                        return true;
                    }
                    else if (start > pos && pos > end)
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
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// この区間がある駅を表すかどうかを判定します。
        /// </summary>
        public bool IsStation => StartPos == EndPos;

        /// <summary>
        /// この路線区間の指定した路線上での順序を表す、0　始まりの整数を取得します。
        /// 列車が位置提供範囲外の場合、-1を返します。
        /// </summary>
        /// <returns></returns>
        public int GetPositionCode(JrhLine jrhLine)
        {
            if (StartPos.Position == null || EndPos.Position == null)
            {
                return -1;
            }

            if (StartPos.Position.TryGetValue(jrhLine, out var start) && EndPos.Position.TryGetValue(jrhLine, out var end))
            {
                return (start - 1) + (end - 1);
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// 2つの区間が等しい区間を示すかどうかを判断します。
        /// </summary>
        /// <param name="a">比較元の区間。</param>
        /// <param name="b">比較対象の区間。</param>
        /// <returns>2つの区間が等しいかどうかを示す値。</returns>
        public static bool operator ==(LineRange a, LineRange b)
        {
            if (a is null || b is null)
            {
                if (a is null && b is null)
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
                if (a.StartPos == b.StartPos && a.EndPos == b.EndPos)
                {
                    return true;
                }

                if (a.StartPos == b.EndPos && a.EndPos == b.StartPos)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 2つの区間が異なる区間を示すかどうかを判断します。
        /// </summary>
        /// <param name="a">比較元の区間。</param>
        /// <param name="b">比較対象の区間。</param>
        /// <returns>2つの区間が異なるかどうかを示す値。</returns>
        public static bool operator !=(LineRange a, LineRange b)
        {
            return !(a == b);
        }

        /// <summary>
        /// この区間を表す文字列を取得します。
        /// </summary>
        /// <returns>この区間を表す文字列。</returns>
        public override string ToString()
        {
            if (StartPos == EndPos)
            {
                return StartPos.ToString();
            }
            else
            {
                return $"{StartPos.ToString()}-{EndPos.ToString()}";
            }
        }

        /// <summary>
        /// この区間が比較対象と等しい区間を示すかどうかを判断します。
        /// </summary>
        /// <param name="obj">比較対象の区間</param>
        /// <returns>2つの区間が等しいかどうか</returns>
        public override bool Equals(object obj)
        {
            if (obj is LineRange range)
            {
                return this == range;
            }
            else
            {
                return false;
            }
        }
    }
}
