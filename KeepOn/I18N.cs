using KeepOn.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KeepOn
{
    public static class I18N
    {
        private static Dictionary<string, string> _strings = new Dictionary<string, string>();
        public static string overrideLanguage = "";

        private static void InitDictionary(string res)
        {
            using (var sr = new StringReader(res))
            {
                foreach (var line in sr.NonWhiteSpaceLines())
                {
                    if (line[0] == '#')
                        continue;

                    var pos = line.IndexOf('=');
                    if (pos < 1)
                        continue;
                    _strings[line.Substring(0, pos)] = line.Substring(pos + 1);
                }
            }
        }

        public static void Init()
        {
            if (!string.IsNullOrEmpty(overrideLanguage))
            {
                var languageDef = Resources.ResourceManager.GetString(overrideLanguage);
                if (languageDef != null)
                {
                    InitDictionary(languageDef);
                    return;
                }
            }

            string name = CultureInfo.CurrentCulture.EnglishName;
            if (name.StartsWith("Chinese", StringComparison.OrdinalIgnoreCase))
            {
                //// choose Traditional Chinese only if we get explicit indication
                //Init(name.Contains("Traditional")
                //    ? Resources.zh_TW
                //    : Resources.zh_CN);
                InitDictionary(Resources.zh_CN);
            }
            else
                InitDictionary(Resources.en_US);

        }

        public static string GetString(string key)
        {
            return _strings.ContainsKey(key)
                ? _strings[key]
                : key;
        }


        #region StringEx

        public static bool IsWhiteSpace(this string value)
        {
            foreach (var c in value)
            {
                if (char.IsWhiteSpace(c)) continue;

                return false;
            }
            return true;
        }

        public static IEnumerable<string> NonEmptyLines(this TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "") continue;
                yield return line;
            }
        }

        public static IEnumerable<string> NonWhiteSpaceLines(this TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.IsWhiteSpace()) continue;
                yield return line;
            }
        }

        #endregion StringEx
    }
}
