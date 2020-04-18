using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TrainInfo.Stations;


namespace App1
{
    internal static class UserConfigManager
    {
        private static List<string> favoriteStations;
        private static string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "favoriteConfig.txt";
        private static XmlSerializer xmlSerializer;

        static UserConfigManager()
        {
            xmlSerializer = new XmlSerializer(typeof(List<string>));
            try
            {
                using (var streamReader = new StreamReader(path, new UTF8Encoding(false)))
                {
                    favoriteStations = (List<string>)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch (Exception)
            {
                favoriteStations = new List<string>();
            }
        }

        public static Station[] GetFavoriteStations()
        {
            return favoriteStations.Select(sta => StationReader.GetStationByName(sta)).ToArray();
        }

        public static bool IsFavoriteStation(string station)
        {
            return favoriteStations.Contains(station);
        }

        public static bool IsFavoriteStation(Station station)
        {
            return favoriteStations.Contains(station.Name);
        }

        public static bool AddfavoriteStation(Station station)
        {
            if (!favoriteStations.Any(sta => sta == station.Name))
            {
                favoriteStations.Add(station.Name);
                SaveFavoriteStation();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DeleteFavoriteStation(Station station)
        {
            if (favoriteStations.Any(sta => sta == station.Name))
            {
                favoriteStations.Remove(station.Name);
                SaveFavoriteStation();
            }
        }

        public static void DeleteFavoriteStation(string station)
        {
            if (favoriteStations.Any(sta => sta == station))
            {
                favoriteStations.Remove(station);
                SaveFavoriteStation();
            }
        }

        private static void SaveFavoriteStation()
        {
            using (var streamWriter = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                xmlSerializer.Serialize(streamWriter, favoriteStations);
            }
        }
    }
}