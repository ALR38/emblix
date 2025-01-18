using System.Text;

namespace Emblix.Session
{
    public static class Transliteration
    {
        private static readonly Dictionary<string, string> CyrillicToLatinMap = new Dictionary<string, string>
        {
            {"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"},
            {"е", "e"}, {"ё", "yo"}, {"ж", "zh"}, {"з", "z"}, {"и", "i"},
            {"й", "y"}, {"к", "k"}, {"л", "l"}, {"м", "m"}, {"н", "n"},
            {"о", "o"}, {"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"},
            {"у", "u"}, {"ф", "f"}, {"х", "h"}, {"ц", "ts"}, {"ч", "ch"},
            {"ш", "sh"}, {"щ", "sch"}, {"ъ", ""}, {"ы", "y"}, {"ь", ""},
            {"э", "e"}, {"ю", "yu"}, {"я", "ya"}
        };

        public static string CyrillicToLatin(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            text = text.ToLower();
            var result = new StringBuilder();

            foreach (var c in text)
            {
                var letter = c.ToString();
                if (CyrillicToLatinMap.ContainsKey(letter))
                    result.Append(CyrillicToLatinMap[letter]);
                else
                    result.Append(letter);
            }

            return result.ToString();
        }
    }
}