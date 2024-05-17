using StardewModdingAPI;
using System.Collections.Generic;

namespace ichortower
{
    public sealed class PortablePreset
    {
        public string Format = "1.1";
        public string Name = "";
        public List<PortableProfile> Profiles = new();
    }

    public sealed class PortableProfile
    {
        public string Name = "";
        public int TargetIndex = -1;
        public ColorizerProfile ColorSettings = new();
        public ToyShader EnableToyShader = ToyShader.None;
        public DepthOfFieldProfile DepthOfField = new();
    }

    public enum ToyShader {
        None = 0,
        DepthOfField = 1,
    }
}
