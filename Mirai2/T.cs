using System.Globalization;
using System.IO;
using NGettext;

namespace Mirai
{
    public class T
    {
        public static Catalog Catalog;
        public T()
        {
            Catalog = 
                new Catalog("Novell", Path.Combine(Directory.GetCurrentDirectory(), "assets/localization"), new CultureInfo(Game.Instance.Config["lang"].ToString()));
        }

        public static string _(string text)
        {
            return Catalog.GetString(text);
        }
    }
}