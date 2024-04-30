using StardewModdingAPI;

namespace ichortower
{
    public class TR
    {
        public static IModHelper Helper;

        public static string Get(string key) {
            return Helper.Translation.Get(key);
        }
    }
}
