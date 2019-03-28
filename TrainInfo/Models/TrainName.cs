using System;
using System.Collections.Generic;
using System.Text;
using TrainInfo;
using TrainInfo.Stations;

namespace TrainInfo.Models
{
    /// <summary>
    /// 列車名に関わる情報を表します。
    /// </summary>
    public class TrainName
    {
        /// <summary>
        /// 列車の種別を取得します。
        /// </summary>
        public TrainTypes TrainType { get; private set; }

        /// <summary>
        /// 列車名を取得します。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 列車番号を取得します。
        /// </summary>
        public int? Number { get; private set; }

        /// <summary>
        /// 区間により複数の種別を持つ場合のサブ種別を取得します。
        /// </summary>
        public TrainTypes? SubTrainType { get; private set; }

        /// <summary>
        /// サブ種別が適用される区間を取得します。
        /// </summary>
        public LineRange SubTrainTypeRange { get; private set; }

        /// <summary>
        /// <see cref="TrainName"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="trainType">列車種別。</param>
        /// <param name="name">列車名。</param>
        /// <param name="number">列車番号。</param>
        /// <param name="subTrainType">サブ列車種別。</param>
        /// <param name="subTrainTypeRange">サブ列車種別が適用される区間。</param>
        public TrainName(TrainTypes trainType, string name = null, int? number = null, TrainTypes? subTrainType = null, LineRange subTrainTypeRange = null)
        {
            TrainType = trainType;
            Name = name;
            Number = number;
            SubTrainType = subTrainType;
            SubTrainTypeRange = subTrainTypeRange;
        }

        public TrainTypes GetTypesByStation(Station station)
        {
            if (SubTrainTypeRange == null)
            {
                return TrainType;
            }
            else
            {
                if (SubTrainTypeRange.Contains(station))
                {
                    return (TrainTypes)SubTrainType;
                }
                else
                {
                    return TrainType;
                }
            }
        }

        /// <summary>
        /// このインスタンスを表す文字列を取得します。
        /// </summary>
        /// <returns>列車名を表す文字列。</returns>
        public override string ToString()
        {
            if (TrainType == TrainTypes.Local)
            {
                return TrainType.GetName() + (Number == null ? "" : Number.ToString() + "号");
            }
            else
            {
                return TrainType.GetName() + Name + (Number == null ? "" : Number.ToString() + "号");
            }
        }
    }
}
