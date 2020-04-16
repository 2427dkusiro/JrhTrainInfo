using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TrainInfo.Stations;

namespace TrainInfo.Resources
{
    /// <summary>
    /// 駅の基本情報と各路線にある駅の情報を提供します。このクラスは内部リソースです。
    /// </summary>
    internal static class StationData
    {
        /// <summary>
        /// すべての駅データを取得します
        /// </summary>
        public static Station[] Stations { get; private set; }

        public static Dictionary<string, Station> KanjiDictionary { get; private set; }
        public static Dictionary<string, Station> HiraganaDictionary { get; private set; }
        public static Dictionary<string, Station> KatakanaDictionary { get; private set; }
        public static Dictionary<string, Station> RomanDictionary { get; private set; }

        public static Dictionary<JrhLine, Station[]> LineDataDictionary { get; private set; }


        static StationData()
        {
            SetStationsArray();
            SetNameDictionarys();
            SetLineDataDictionary();
        }

        private static void SetStationsArray()
        {
            Stations = JsonConvert.DeserializeObject<Station[]>(Properties.Resources.stationArray);
        }

        private static void SetNameDictionarys()
        {
            KanjiDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(Properties.Resources.StationNameDic)
                .ToDictionary(kvp => kvp.Key, kvp => Stations[kvp.Value]);
            HiraganaDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(Properties.Resources.StationHiraDic)
                .ToDictionary(kvp => kvp.Key, kvp => Stations[kvp.Value]);
            KatakanaDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(Properties.Resources.StationKanaDic)
                .ToDictionary(kvp => kvp.Key, kvp => Stations[kvp.Value]);
            RomanDictionary = JsonConvert.DeserializeObject<Dictionary<string, int>>(Properties.Resources.StationRomanDic)
                .ToDictionary(kvp => kvp.Key, kvp => Stations[kvp.Value]);
        }

        private static void SetLineDataDictionary()
        {
            LineDataDictionary = JsonConvert.DeserializeObject<Dictionary<JrhLine, int[]>>(Properties.Resources.LineDataDic)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(n => Stations[n]).ToArray());
        }

        /*
        private static void InitializeData()
        {
            hakodate_Iwamizawa = new Station[] { new Station(91, "札幌", "さっぽろ", "サッポロ", "Sapporo", true, JrhLine.Hakodate_Iwamizawa, 1), new Station(92, "苗穂", "なえぼ", "ナエボ", "Naebo", false, JrhLine.Hakodate_Iwamizawa, 2), new Station(93, "白石", "しろいし", "シロイシ", "Shiroishi", false, JrhLine.Hakodate_Iwamizawa, 3), new Station(94, "厚別", "あつべつ", "アツベツ", "Atsubetsu", false, JrhLine.Hakodate_Iwamizawa, 4), new Station(95, "森林公園", "しんりんこうえん", "シンリンコウエン", "Shinrinkoen", false, JrhLine.Hakodate_Iwamizawa, 5), new Station(96, "大麻", "おおあさ", "オオアサ", "Oasa", false, JrhLine.Hakodate_Iwamizawa, 6), new Station(97, "野幌", "のっぽろ", "ノッポロ", "Nopporo", false, JrhLine.Hakodate_Iwamizawa, 7), new Station(98, "高砂", "たかさご", "タカサゴ", "Takasago", false, JrhLine.Hakodate_Iwamizawa, 8), new Station(99, "江別", "えべつ", "エベツ", "Ebetsu", true, JrhLine.Hakodate_Iwamizawa, 9), new Station(100, "豊幌", "とよほろ", "トヨホロ", "Toyohoro", false, JrhLine.Hakodate_Iwamizawa, 10), new Station(101, "幌向", "ほろむい", "ホロムイ", "Horomui", false, JrhLine.Hakodate_Iwamizawa, 11), new Station(102, "上幌向", "かみほろむい", "カミホロムイ", "Kami-Horomui", false, JrhLine.Hakodate_Iwamizawa, 12), new Station(103, "岩見沢", "いわみざわ", "イワミザワ", "Iwamizawa", true, JrhLine.Hakodate_Iwamizawa, 13), };
            hakodate_Otaru = new Station[] { new Station(91, "札幌", "さっぽろ", "サッポロ", "Sapporo", true, JrhLine.Hakodate_Otaru, 1), new Station(90, "桑園", "そうえん", "ソウエン", "Soen", false, JrhLine.Hakodate_Otaru, 2), new Station(89, "琴似", "ことに", "コトニ", "Kotoni", false, JrhLine.Hakodate_Otaru, 3), new Station(88, "発寒中央", "はっさむちゅうおう", "ハッサムチュウオウ", "Hassamuchuo", false, JrhLine.Hakodate_Otaru, 4), new Station(87, "発寒", "はっさむ", "ハッサム", "Hassamu", false, JrhLine.Hakodate_Otaru, 5), new Station(86, "稲積公園", "いなづみこうえん", "イナヅミコウエン", "Inazumikoen", false, JrhLine.Hakodate_Otaru, 6), new Station(85, "手稲", "ていね", "テイネ", "Teine", true, JrhLine.Hakodate_Otaru, 7), new Station(84, "稲穂", "いなほ", "イナホ", "Inaho", false, JrhLine.Hakodate_Otaru, 8), new Station(83, "星置", "ほしおき", "ホシオキ", "Hoshioki", false, JrhLine.Hakodate_Otaru, 9), new Station(82, "ほしみ", "ほしみ", "ホシミ", "Hoshimi", true, JrhLine.Hakodate_Otaru, 10), new Station(81, "銭函", "ぜにばこ", "ゼニバコ", "Zenibako", false, JrhLine.Hakodate_Otaru, 11), new Station(79, "朝里", "あさり", "アサリ", "Asari", false, JrhLine.Hakodate_Otaru, 12), new Station(78, "小樽築港", "おたるちっこう", "オタルチッコウ", "Otaruchikko", false, JrhLine.Hakodate_Otaru, 13), new Station(77, "南小樽", "みなみおたる", "ミナミオタル", "Minami-Otaru", false, JrhLine.Hakodate_Otaru, 14), new Station(76, "小樽", "おたる", "オタル", "Otaru", true, JrhLine.Hakodate_Otaru, 15), };
            chitose_Tomakomai = new Station[] { new Station(91, "札幌", "さっぽろ", "サッポロ", "Sapporo", true, JrhLine.Chitose_Tomakomai, 1), new Station(92, "苗穂", "なえぼ", "ナエボ", "Naebo", false, JrhLine.Chitose_Tomakomai, 2), new Station(93, "白石", "しろいし", "シロイシ", "Shiroishi", false, JrhLine.Chitose_Tomakomai, 3), new Station(234, "平和", "へいわ", "ヘイワ", "Heiwa", false, JrhLine.Chitose_Tomakomai, 4), new Station(235, "新札幌", "しんさっぽろ", "シンサッポロ", "Shin-Sapporo", false, JrhLine.Chitose_Tomakomai, 5), new Station(236, "上野幌", "かみのっぽろ", "カミノッポロ", "Kami-Nopporo", false, JrhLine.Chitose_Tomakomai, 6), new Station(238, "北広島", "きたひろしま", "キタヒロシマ", "Kita-Hiroshima", false, JrhLine.Chitose_Tomakomai, 7), new Station(239, "島松", "しままつ", "シママツ", "Shimamatsu", false, JrhLine.Chitose_Tomakomai, 8), new Station(240, "恵み野", "めぐみの", "メグミノ", "Megumino", false, JrhLine.Chitose_Tomakomai, 9), new Station(241, "恵庭", "えにわ", "エニワ", "Eniwa", false, JrhLine.Chitose_Tomakomai, 10), new Station(242, "サッポロビール庭園", "さっぽろびーるていえん", "サッポロビールテイエン", "Sapporo-Beer-Teien", false, JrhLine.Chitose_Tomakomai, 11), new Station(243, "長都", "おさつ", "オサツ", "Osatsu", false, JrhLine.Chitose_Tomakomai, 12), new Station(244, "千歳", "ちとせ", "チトセ", "Chitose", true, JrhLine.Chitose_Tomakomai, 13), new Station(245, "南千歳", "みなみちとせ", "ミナミチトセ", "Minami-Chitose", true, JrhLine.Chitose_Tomakomai, 14), new Station(248, "植苗", "うえなえ", "ウエナエ", "Uenae", false, JrhLine.Chitose_Tomakomai, 15), new Station(215, "苫小牧", "とまこまい", "トマコマイ", "Tomakomai", true, JrhLine.Chitose_Tomakomai, 16), };
            sassyo = new Station[] { new Station(91, "札幌", "さっぽろ", "サッポロ", "Sapporo", true, JrhLine.Sassyo, 1), new Station(90, "桑園", "そうえん", "ソウエン", "Soen", false, JrhLine.Sassyo, 2), new Station(251, "八軒", "はちけん", "ハチケン", "Hachiken", false, JrhLine.Sassyo, 3), new Station(252, "新川", "しんかわ", "シンカワ", "Shinkawa", false, JrhLine.Sassyo, 4), new Station(253, "新琴似", "しんことに", "シンコトニ", "Shin-Kotoni", false, JrhLine.Sassyo, 5), new Station(254, "太平", "たいへい", "タイヘイ", "Taihei", false, JrhLine.Sassyo, 6), new Station(255, "百合が原", "ゆりがはら", "ユリガハラ", "Yurigahara", false, JrhLine.Sassyo, 7), new Station(256, "篠路", "しのろ", "シノロ", "Shinoro", false, JrhLine.Sassyo, 8), new Station(257, "拓北", "たくほく", "タクホク", "Takuhoku", false, JrhLine.Sassyo, 9), new Station(258, "あいの里教育大", "あいのさときょういくだい", "アイノサトキョウイクダイ", "Ainosato-Kyoikudai", false, JrhLine.Sassyo, 10), new Station(259, "あいの里公園", "あいのさとこうえん", "アイノサトコウエン", "Ainosato-Koen", true, JrhLine.Sassyo, 11), new Station(260, "石狩太美", "いしかりふとみ", "イシカリフトミ", "Ishikari-Futomi", false, JrhLine.Sassyo, 12), new Station(261, "石狩当別", "いしかりとうべつ", "イシカリトウベツ", "Ishikari-Tobetsu", true, JrhLine.Sassyo, 13), new Station(262, "北海道医療大学", "ほっかいどういりょうだいがく", "ホッカイドウイリョウダイガク", "Hokkaido-Iryodaigaku", true, JrhLine.Sassyo, 14), };
            sekisyo = new Station[] { new Station(245, "南千歳", "みなみちとせ", "ミナミチトセ", "Minami-Chitose", false, JrhLine.Sekisyo, 1), new Station(220, "追分", "", "", "", false, JrhLine.Sekisyo, 2), new Station(284, "川端", "", "", "", false, JrhLine.Sekisyo, 3), new Station(286, "滝ノ上", "", "", "", false, JrhLine.Sekisyo, 4), new Station(288, "新夕張", "", "", "", false, JrhLine.Sekisyo, 5), new Station(293, "占冠", "", "", "", false, JrhLine.Sekisyo, 6), new Station(297, "トマム", "", "", "", false, JrhLine.Sekisyo, 7), new Station(325, "新得", "", "", "", false, JrhLine.Sekisyo, 8), };
        }
        */

    }
}
