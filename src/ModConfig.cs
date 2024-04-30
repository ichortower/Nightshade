namespace ichortower
{
    public sealed class ModConfig
    {
        public bool ColorizerEnabled = true;
        public bool ColorizeBySeason = true;
        public int ColorizerActiveProfile = 0;
        public ColorizerPreset[] ColorizerProfiles = new ColorizerPreset[4] {
            new(), new(), new(), new(),
        };

        public bool DepthOfFieldEnabled = true;
        public DepthOfFieldPreset DepthOfFieldSettings;
    }

    public sealed class ColorizerPreset
    {
        public float Saturation = -0.12f;
        public float Lightness = 0f;
        public float Contrast = 0.06f;

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
    }

    public sealed class DepthOfFieldPreset
    {
        public float Field = 0.6f;
        public float Ramp = 0.16f;
        public float Intensity = 6.0f;
    }


    public enum LumaType {
        BT709 = 0,
        BT601 = 1,
    }
}
