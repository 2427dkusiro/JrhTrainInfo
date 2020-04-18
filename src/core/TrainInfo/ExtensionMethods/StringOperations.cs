using System.Collections.Generic;
using System.Linq;

namespace TrainInfo.ExtensionMethods
{
    internal static class StringOperations
    {
        /// <summary>
        /// 2つの指定された文字の間にある文字列を返します。
        /// </summary>
        /// <param name="text">検索対象の文字列</param>
        /// <param name="start">始点となる文字。</param>
        /// <param name="end">終点となる文字。</param>
        /// <returns>検索結果。存在しない場合は空の文字列を返します。</returns>
        public static string GetRange(this string text, char start, char end)
        {
            var Isstarted = false;
            var result = new List<char>();

            foreach (var c in text)
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
            var result = new List<char>();
            var isStarted = false;

            foreach (var c in text)
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
            var result = new List<char>();

            foreach (var c in text)
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
