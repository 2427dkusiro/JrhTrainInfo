using System;
using System.Data.Common;
using System.Linq;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace TrainInfo
{
    /// <summary>
    /// 列車データを表します。
    /// </summary>
    public partial class TrainData
    {
        /// <summary>
        /// <see cref="TrainData"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="arrivalType"></param>
        /// <param name="departureStation"></param>
        /// <param name="destination"></param>
        /// <param name="condition"></param>
        /// <param name="nowPosition"></param>
        public TrainData(TrainName name, DateTime time, Station departureStation, ArrivalTypes arrivalType, Station destination, TrainCondition condition, LineRange nowPosition)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Time = time;
            DepartureStation = departureStation; // ?? throw new ArgumentNullException(nameof(departureStation));
            ArrivalType = arrivalType;
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            NowPosition = nowPosition; //nullable
        }

        /// <summary>
        /// 列車名に関わる情報を取得します。
        /// </summary>
        public TrainName Name { get; private set; }

        /// <summary>
        /// 列車の出発または到着の時刻を取得します。
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// 列車の出発駅または到着駅を取得します。
        /// </summary>
        public Station DepartureStation { get; private set; }

        /// <summary>
        /// 列車の到着種別を取得します。
        /// </summary>
        public ArrivalTypes ArrivalType { get; private set; }

        /// <summary>
        /// 列車の行先を取得します。
        /// </summary>
        public Station Destination { get; private set; }

        /// <summary>
        /// 列車の運行状況を取得します。
        /// </summary>
        public TrainCondition Condition { get; private set; }

        /// <summary>
        /// 列車の現在位置を取得します。
        /// </summary>
        public LineRange NowPosition { get; private set; }

        /// <summary>
        /// 駅発車情報として表示する為の列車種別情報を取得します。
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public TrainTypes GetDisplayTrainTypes(Station station)
        {
            if (Name.Name == "いしかりライナー")
            {
                var destId = Destination.StationID;
                var targetId = station.StationID;
                var rapidId = new[] { Name.SubTrainTypeRange.StartPos.StationID, Name.SubTrainTypeRange.EndPos.StationID };

                if (destId > rapidId.Max())
                {
                    if (targetId >= rapidId.Max())
                    {
                        return TrainTypes.Local;
                    }
                    else if (targetId >= rapidId.Min())
                    {
                        return TrainTypes.Semi_Rapid;
                    }
                    else
                    {
                        return TrainTypes.Become_Semi_Rapid;
                    }
                }
                else
                {
                    if (targetId <= rapidId.Min())
                    {
                        return TrainTypes.Local;
                    }
                    else if (targetId <= rapidId.Max())
                    {
                        return TrainTypes.Semi_Rapid;
                    }
                    else
                    {
                        return TrainTypes.Become_Semi_Rapid;
                    }
                }
            }
            else
            {
                return Name.TrainType;
            }
        }

        /// <summary>
        /// このインスタンスを表す文字列を取得します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (NowPosition == null)
            {
                return $"{Name.ToString()} {Destination.Name}行 出発前";
            }
            else
            {
                return $"{Name.ToString()} {Destination.Name}行 {NowPosition.ToString()}走行中";
            }
        }

        public static bool operator ==(TrainData left, TrainData right)
        {
            if (left is null)
            {
                if (right is null)
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
                return left.Time == right.Time && left.Name == right.Name && left.Destination == right.Destination
                    && left.ArrivalType == right.ArrivalType && left.DepartureStation == right.DepartureStation;
            }
        }

        public static bool operator !=(TrainData left, TrainData right)
        {
            return !(left == right);
        }
    }
}
