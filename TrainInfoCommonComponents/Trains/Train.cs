using System;
using System.Collections.Generic;
using System.Text;
using TrainInfo.Stations;

namespace TrainInfo.Trains
{
    public class Train
    {
        public Train()
        {
            TimeData = new ListDictionary<Station, StationTrainTime>();
        }


        public int Id { get; }

        public TrainData.TrainName TrainName { get; set; }

        public Station Destination { get; set; }

        public ListDictionary<Station, StationTrainTime> TimeData { get; private set; }
    }


    public class StationTrainTime : IComparable<StationTrainTime>
    {
        public StationTrainTime()
        {

        }

        public StationTrainTime(DateTime arrivalTime, DateTime departureTime)
        {
            ArrivalTime = arrivalTime;
            DepartureTime = departureTime;
        }

        public DateTime ArrivalTime { get; set; }

        public DateTime DepartureTime { get; set; }

        public int CompareTo(StationTrainTime other)
        {
            if (ArrivalTime.Year < 2010)
            {
                return 1;
            }
            if (other.DepartureTime.Year < 2010)
            {
                return -1;
            }

            return DepartureTime.CompareTo(other.DepartureTime);
        }
    }
}
