using StardewModdingAPI;

namespace ichortower
{
    public class TR
    {
        public static IModHelper Helper = Nightshade.instance.Helper;

        public static string Get(string key) {
            return Helper.Translation.Get(key);
        }
    }
}
