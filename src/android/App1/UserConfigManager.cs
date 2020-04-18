using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;


namespace JrhTrainInfoAndroid
{
    internal static class UserConfigManager
    {
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "UserConfig.txt";
        private static XmlSerializer xmlSerializer;

        private static UserConfigData userConfigData;

        public static event EventHandler ValueChanged;

        static UserConfigManager()
        {
            xmlSerializer = new XmlSerializer(typeof(UserConfigData));
            try
            {
                using (var streamReader = new StreamReader(path, new UTF8Encoding(false)))
                {
                    //var str = streamReader.ReadToEnd();
                    userConfigData = (UserConfigData)xmlSerializer.Deserialize(streamReader);
                }
            }
            catch (Exception ex)
            {
                userConfigData = new UserConfigData();
            }
        }

        public static IEnumerable<Station> GetFavoriteStations()
        {
            return userConfigData.GetFavoriteStations();
        }

        public static IEnumerable<JrhLine> GetFavoriteJehLines()
        {
            return userConfigData.GetFavoriteJrhLines();
        }

        public static bool IsFavoriteStation(Station station)
        {
            return userConfigData.FavoriteStationString.Contains(station.Name);
        }

        public static bool IsFavoriteLine(JrhLine jrhLine)
        {
            return userConfigData.FavoriteLineString.Contains(jrhLine.GetName());
        }

        public static bool AddfavoriteStation(Station station)
        {
            if (!userConfigData.FavoriteStationString.Any(sta => sta == station.Name))
            {
                userConfigData.FavoriteStationString.Add(station.Name);
                SaveUserConfig();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AddfavoriteLine(JrhLine jrhLine)
        {
            if (!userConfigData.FavoriteLineString.Any(line => line == jrhLine.GetName()))
            {
                userConfigData.FavoriteLineString.Add(jrhLine.GetName());
                SaveUserConfig();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DeleteFavoriteStation(Station station)
        {
            if (userConfigData.FavoriteStationString.Any(sta => sta == station.Name))
            {
                userConfigData.FavoriteStationString.Remove(station.Name);
                SaveUserConfig();
            }
        }

        public static void DeletefavoriteLine(JrhLine jrhLine)
        {
            if (userConfigData.FavoriteLineString.Any(line => line == jrhLine.GetName()))
            {
                userConfigData.FavoriteLineString.Remove(jrhLine.GetName());
                SaveUserConfig();
            }
        }

        private static void SaveUserConfig()
        {
            using (var streamWriter = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                xmlSerializer.Serialize(streamWriter, userConfigData);
            }
            ValueChanged?.Invoke(userConfigData, new EventArgs());
        }
    }

    [Serializable]
    public class UserConfigData
    {
        public UserConfigData()
        {
            FavoriteStationString = new List<string>();
            FavoriteLineString = new List<string>();
        }

        public UserConfigData(List<string> favoriteStationString, List<string> favoriteLineString)
        {
            FavoriteStationString = favoriteStationString ?? new List<string>();
            FavoriteLineString = favoriteLineString ?? new List<string>();
        }

        public Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        public List<string> FavoriteStationString { get; }

        public IEnumerable<Station> GetFavoriteStations()
        {
            return FavoriteStationString.Select(str => StationReader.GetStationByName(str));
        }

        public List<string> FavoriteLineString { get; }

        public IEnumerable<JrhLine> GetFavoriteJrhLines()
        {
            return FavoriteLineString.Select(str => JrhLineCreater.FromString(str));
        }
    }
}