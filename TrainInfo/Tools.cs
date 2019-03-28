using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainInfo.Models;

namespace TrainInfo
{
    /// <summary>
    /// 列車データを処理する拡張メソッドを提供します。
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// <see cref="TrainTypes"/> の日本語名を取得します。
        /// </summary>
        /// <param name="trainTypes">取得対象の列車種別。</param>
        /// <returns>一般的な日本語の列車種別名。</returns>
        public static string GetName(this TrainTypes trainTypes)
        {
            switch (trainTypes)
            {
                case TrainTypes.Local:
                    return "普通";
                case TrainTypes.Semi_Rapid:
                    return "区間快速";
                case TrainTypes.Rapid:
                    return "快速";
                case TrainTypes.Express:
                    return "急行";
                case TrainTypes.Ltd_Exp:
                    return "特急";
                case TrainTypes.Extra:
                    return "臨時";
                case TrainTypes.Other:
                    return "不明";
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// <see cref="TrainConditions"/> の日本語名を取得します。
        /// </summary>
        /// <param name="trainConditions">取得対象の運行状況。</param>
        /// <returns>列車状況を表す文字列。</returns>
        public static string GetName(this TrainConditions trainConditions)
        {
            switch (trainConditions)
            {
                case TrainConditions.OutsideArea:
                    return "表示区間外";
                case TrainConditions.NotDeparted:
                case TrainConditions.NotDepartedArrive:
                    return "出発前";
                case TrainConditions.OnSchedule:
                    return "通常運行";
                case TrainConditions.Delayed:
                    return "列車遅延";
                case TrainConditions.PartiallySuspended:
                    return "部分運休";
                case TrainConditions.Suspended:
                    return "運休";
                default:
                    throw new NotSupportedException();
            }
        }

        public static string GetName(this JrhLine jrhLine)
        {
            switch (jrhLine)
            {
                case JrhLine.Hakodate_Iwamizawa:
                    return "函館本線岩見沢方面";
                case JrhLine.Hakodate_Otaru:
                    return "函館本線小樽方面";
                case JrhLine.Chitose_Tomakomai:
                    return "千歳線苫小牧方面";
                case JrhLine.Sassyo:
                    return "学園都市線";
                case JrhLine.Sekisyo:
                    return "石勝線";
                case JrhLine.Muroran:
                    return "室蘭本線";
                default:
                    throw new NotSupportedException("路線が存在しません");
            }
        }

        public static string GetName(this JrhDestType jrhDistType)
        {
            switch (jrhDistType)
            {
                case JrhDestType.Sapporo_Hakodate:
                    return "函館本線[札幌方面]";
                case JrhDestType.Sapporo_Chitose:
                    return "千歳線[札幌方面]";
                case JrhDestType.Sapporo_Sassyo:
                    return "学園都市線[札幌方面]";
                case JrhDestType.Hakodate_Otaru:
                    return "函館本線[小樽方面]";
                case JrhDestType.Muroran_Iwamizawa:
                    return "室蘭本線[岩見沢方面]";
                case JrhDestType.Hakodate_Iwamizawa:
                    return "函館本線[岩見沢方面]";
                case JrhDestType.Hakodate_Yoichi:
                    return "函館本線[余市方面]";
                case JrhDestType.Hakodate_Takikawa:
                    return "函館本線[滝川・旭川方面]";
                case JrhDestType.Chitose_Chitose:
                    return "千歳線[千歳方面]";
                case JrhDestType.Chitose_AP:
                    return "千歳線[新千歳空港]";
                case JrhDestType.Chitose_Rapid_AP:
                    return "千歳線[（快速）新千歳空港]";
                case JrhDestType.Chitose_LocalRapid_Sapporo:
                    return "千歳線[（普通・快速）札幌方面]";
                case JrhDestType.Chitose_LimExp_Sapporo:
                    return "千歳線[（特急）札幌]";
                case JrhDestType.Muroran_Tomakomai:
                    return "室蘭本線[苫小牧方面]";
                case JrhDestType.chitose_Tomakomai:
                    return "千歳線[苫小牧・東室蘭・函館方面]";
                case JrhDestType.Chitose_Muroran:
                    return "千歳線[室蘭・函館方面]";
                case JrhDestType.Sassyo_IshikariTobetsu:
                    return "学園都市線[石狩当別方面]";
                case JrhDestType.Sassyo_Urausu:
                    return "学園都市線[浦臼・新十津川方面]";
                case JrhDestType.Muroran_Oiwake:
                    return "室蘭本線[追分方面]";
                case JrhDestType.Sekisyo_MinamiChitose_Sapporo:
                    return "石勝線[南千歳・札幌方面]";
                case JrhDestType.Sekisyo_Oiwake_Obihiro:
                    return "石勝線[追分・帯広・釧路方面]";
                case JrhDestType.Sekisyo_Oiwake_Sapporo:
                    return "石勝線[追分・新千歳空港・札幌方面]";
                case JrhDestType.Sekisyo_Shiyubari:
                    return "石勝線[新夕張方面]";
                case JrhDestType.Sekisyo_Yubari:
                    return "石勝線[夕張方面]";
                case JrhDestType.Sekisyo_Obihiro:
                    return "石勝線[帯広方面]";
                case JrhDestType.Nemuro_Takikawa:
                    return "根室本線[滝川方面]";
                case JrhDestType.Nemuro_Hurano:
                    return "根室本線[富良野方面]";
                case JrhDestType.Nemuro_Shintoku:
                    return "根室本線[新得方面]";
                case JrhDestType.Nemuro_Obihiro:
                    return "根室本線[帯広方面]";
                case JrhDestType.Nemuro_Kusiro:
                    return "根室本線[釧路方面]";
                case JrhDestType.Hanasaki_Kusiro:
                    return "花咲線[釧路方面]";
                case JrhDestType.Senmo_Kusiro:
                    return "釧網本線[釧路方面]";
                case JrhDestType.Hanasaki_Nemuro:
                    return "釧網本線[根室方面]";
                case JrhDestType.Senmo_Abashiri:
                    return "釧網本線[網走方面]";
                case JrhDestType.Sekihoku_Asahikawa:
                    return "石北本線[旭川方面]";
                case JrhDestType.Hurano_Asahikawa:
                    return "富良野線[旭川方面]";

                default:
                    throw new NotSupportedException("路線が存在しません");
            }
        }

        /// <summary>
        /// 2つの指定された文字の間にある文字列を返します。
        /// </summary>
        /// <param name="text">検索対象の文字列</param>
        /// <param name="start">始点となる文字。</param>
        /// <param name="end">終点となる文字。</param>
        /// <returns>検索結果。存在しない場合は空の文字列を返します。</returns>
        public static string GetRange(this string text, char start, char end)
        {
            bool Isstarted = false;
            List<char> result = new List<char>();

            foreach (char c in text)
            {
                if (Isstarted)
                {
                    if (c == end)
                    {
                        break;
                    }
                    else
                    {
                        result.Add(c);
                    }
                }
                else if (c == start)
                {
                    Isstarted = true;
                }
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// 指定された文字から最後までの間にある文字列を返します。
        /// </summary>
        /// <param name="text">検索対象の文字列</param>
        /// <param name="delimiter">区切り文字。</param>
        /// <returns>検索結果。存在しない場合は空の文字列を返します。</returns>
        public static string GetRangeWithStart(this string text, char delimiter)
        {
            List<char> result = new List<char>();
            bool isStarted = false;

            foreach (char c in text)
            {
                if (c == delimiter)
                {
                    isStarted = true;
                }
                else if (isStarted)
                {
                    result.Add(c);
                }
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// 文字列の最初から指定された文字までの間にある文字列を返します
        /// </summary>
        /// <param name="text">検索対象の文字列。</param>
        /// <param name="delimiter">区切り文字。</param>
        /// <returns>検索結果。存在しない場合は空の文字列を返します。</returns>
        public static string GetRangeWithEnd(this string text, char delimiter)
        {
            List<char> result = new List<char>();

            foreach (char c in text)
            {
                if (c == delimiter)
                {
                    break;
                }
                else
                {
                    result.Add(c);
                }
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// 文字列内に含まれるすべての全角数字を半角に変換して返します。変換処理は文字コードに依存しています。
        /// </summary>
        /// <param name="text">検索対象の文字列。</param>
        /// <returns>全角数字が半角に変換された文字列。</returns>
        public static string ToHankakuNum(this string text)
        {
            const int start = 65296;
            const int end = 65305;
            const int offset = 65248;

            return new string(text.Select(c => c >= start && c <= end ? (char)(c - offset) : c).ToArray());
        }
    }
}
