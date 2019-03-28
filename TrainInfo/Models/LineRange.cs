using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TrainInfo;
using TrainInfo.Stations;

namespace TrainInfo.Models
{
    /// <summary>
    /// 線路上の区間を表します。
    /// </summary>
    public class LineRange
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
                    return true;
                else
                    return false;
            }
            else
            {
                if (a.StartPos == b.StartPos && a.EndPos == b.EndPos)
                    return true;
                if (a.StartPos == b.EndPos && a.EndPos == b.StartPos)
                    return true;
                else
                    return false;
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
        /// 現在の列車の位置を表す実数を取得します。
        /// </summary>
        /// <param name="lineDestPair">位置を取得する対象となる路線</param>
        /// <returns>札幌を起点(1)とする位置情報。指定路線上にない場合は<c>null</c>。</returns>
        public decimal? GetNowPosition(JrhLine jrhLine)
        {
            if (StartPos == EndPos)
            {
                if (StartPos.Position.TryGetValue(jrhLine, out var result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (StartPos.Position.TryGetValue(jrhLine, out var startPos) && EndPos.Position.TryGetValue(jrhLine, out var endPos))
                {
                    if (startPos > endPos)
                    {
                        return startPos - 0.5m;
                    }
                    else
                    {
                        return startPos + 0.5m;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public bool Contains(Station station)
        {
            var pos_a = StartPos.Position;
            var pos_b = EndPos.Position;

            JrhLine? jrhLine = null;
            int start = 0;
            int end = 0;

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
                if (station.Position.TryGetValue(nouNull, out int pos))
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
                return false;
        }

        /// <summary>
        /// この区間がある駅を表すかどうかを判定します。
        /// </summary>
        public bool IsStation => StartPos == EndPos;

        /// <summary>
        /// この区間を表す文字列を取得します。
        /// </summary>
        /// <returns>この区間を表す文字列。</returns>
        public override string ToString()
        {
            if (StartPos == EndPos)
                return StartPos.ToString();
            else
                return $"{StartPos.ToString()}-{EndPos.ToString()}";
        }

        /// <summary>
        /// この区間が比較対象と等しい区間を示すかどうかを判断します。
        /// </summary>
        /// <param name="obj">比較対象の区間</param>
        /// <returns>2つの区間が等しいかどうか</returns>
        public override bool Equals(object obj)
        {
            if (obj is LineRange range)
                return this == range;
            else
                return false;
        }

    }
}
