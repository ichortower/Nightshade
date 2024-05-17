using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ichortower
{
    public sealed class ModConfig
    {
        public KeybindList MenuKeybind = new(SButton.H);
        public bool ColorizeWorld = true;
        public bool ColorizeUI = true;
        public bool ColorizeBySeason = true;
        public bool ColorizeIndoors = true;
        public int ColorizerActiveProfile = 0;
        public ColorizerProfile[] ColorizerProfiles = new ColorizerProfile[5] {
            new(), new(), new(), new(), new(),
        };

        public bool DepthOfFieldEnabled = false;
        public DepthOfFieldProfile DepthOfFieldSettings = new();

        public static ModConfig ApplyMigrations(ModConfig conf)
        {
            if (conf.ColorizerProfiles.Length < 5) {
                ColorizerProfile[] arr = new ColorizerProfile[5];
                for (int i = 0; i < 5; ++i) {
                    if (i < conf.ColorizerProfiles.Length) {
                        arr[i] = conf.ColorizerProfiles[i].Clone();
                    }
                    else {
                        arr[i] = new();
                    }
                }
                conf.ColorizerProfiles = arr;
            }
            return conf;
        }
    }

    public sealed class ColorizerProfile
    {
        public float Saturation = 0f;
        public float Lightness = 0f;
        public float Contrast = 0f;

        public LumaType Luma = LumaType.BT709;

        public float ShadowR = 0f;
        public float ShadowG = 0f;
        public float ShadowB = 0f;
        public float MidtoneR = 0f;
        public float MidtoneG = 0f;
        public float MidtoneB = 0f;
        public float HighlightR = 0f;
        public float HighlightG = 0f;
        public float HighlightB = 0f;

        public ColorizerProfile Clone() {
            return (ColorizerProfile) this.MemberwiseClone();
        }
    }

    public sealed class DepthOfFieldProfile
    {
        public float Field = 0.6f;
        public float Intensity = 6.0f;
    }

    public enum LumaType {
        BT709 = 0,
        BT601 = 1,
    }
}
