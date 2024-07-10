using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace ichortower
{
    public sealed class ModConfig
    {
        public static string CurrentFormat = "1.1";
        public string Format = null;
        public KeybindList MenuKeybind = new(SButton.H);

        public List<NightshadeProfile> Profiles = new();

        /*
         * Deprecated fields from format 1.0 (no format).
         * Privatizing the getter prevents them from being serialized.
         * See ApplyMigrations for where they go after loading.
         */
        public bool ColorizeWorld { private get; set; } = true;
        public bool ColorizeUI { private get; set; } = true;
        public bool ColorizeBySeason { private get; set; } = true;
        public bool DepthOfFieldEnabled { private get; set; } = false;
        public List<ColorizerProfile> ColorizerProfiles { private get; set; } = new();
        public int ColorizerActiveProfile { private get; set; } = 0;
        public DepthOfFieldProfile DepthOfFieldSettings { private get; set; } = new();

        public ModConfig()
        {
            Format = CurrentFormat;
        }

        /*
         *
         */
        public static ModConfig ApplyMigrations(ModConfig conf)
        {
            if (conf.Format is null ||
                    new SemanticVersion(conf.Format).IsOlderThan("1.1")) {
                foreach (var profile in conf.ColorizerProfiles) {
                    NightshadeProfile np = new();
                    np.ColorizeWorld = conf.ColorizeWorld;
                    np.ColorizeUI = conf.ColorizeUI;
                    np.ColorSettings = profile.Clone();
                    if (conf.DepthOfFieldEnabled) {
                        np.EnableToyShader = ToyShader.DepthOfField;
                        np.DepthOfField = conf.DepthOfFieldSettings;
                    }
                    conf.Profiles.Add(np);
                }
                if (conf.ColorizeBySeason && conf.Profiles.Count >= 4) {
                    for (int i = 0; i < 4; ++i) {
                        conf.Profiles[i].Conditions = "LOCATION_SEASON Target " +
                                $"{((StardewValley.Season)i).ToString()}";
                    }
                    conf.Profiles[0].ColorizeTitleScreen = true;
                }
            }
            conf.Format = CurrentFormat;
            return conf;
        }
    }

    public sealed class NightshadeProfile
    {
        public string Name = "";
        public string Conditions = "";
        public bool ColorizeTitleScreen = false;
        public bool ColorizeWorld = true;
        public bool ColorizeUI = true;
        public ColorizerProfile ColorSettings = new();
        public ToyShader EnableToyShader = ToyShader.None;
        public DepthOfFieldProfile DepthOfField = new();

        public NightshadeProfile Clone() {
            NightshadeProfile other = (NightshadeProfile) this.MemberwiseClone();
            other.ColorSettings = ColorSettings.Clone();
            other.DepthOfField = DepthOfField.Clone();
            return other;
        }
    }

    public sealed class ColorizerProfile
    {
        public float Saturation = 0f;
        public float Lightness = 0f;
        public float Contrast = 0f;

        public LumaType Luma = LumaType.BT709;

        public float ShadowR = 0f;
        public float MidtoneR = 0f;
        public float HighlightR = 0f;
        public float ShadowG = 0f;
        public float MidtoneG = 0f;
        public float HighlightG = 0f;
        public float ShadowB = 0f;
        public float MidtoneB = 0f;
        public float HighlightB = 0f;

        public ColorizerProfile Clone() {
            return (ColorizerProfile) this.MemberwiseClone();
        }
    }

    public sealed class DepthOfFieldProfile
    {
        public float Field = 0.6f;
        public float Intensity = 6.0f;

        public DepthOfFieldProfile Clone() {
            return (DepthOfFieldProfile) this.MemberwiseClone();
        }
    }

    public enum LumaType {
        BT709 = 0,
        BT601 = 1,
    }

    public enum ToyShader {
        None = 0,
        DepthOfField = 1,
    }
}
