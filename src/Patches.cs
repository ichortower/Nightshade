using HarmonyLib;
using StardewValley;
using System.Reflection;

namespace ichortower
{
    internal class Patches
    {
        /*
         * The game checks the current zoom level to determine whether to draw
         * the world on a framebuffer. To be able to re-render the world layer,
         * we need to enable this at all times, not just when zoom is not 100%.
         * So, force it to return true (during gameplay).
         */
        public static void Game1_ShouldDrawOnBuffer_Postfix(
                ref bool __result)
        {
            if (Game1.gameMode == Game1.playingGameMode) {
                __result = true;
            }
        }

        /*
         * There are SMAPI events for returning to the title screen
         * (GameLoop.ReturnedToTitle, LoadStageChanged.ReturningToTitle),
         * but they are not raised until after the title screen has rendered;
         * this causes a color flash if you are using a title screen profile.
         * So, run our config update right after the exit call.
         */
        public static void Game1_ExitToTitle_Postfix()
        {
            Nightshade.instance.ApplyConfig(Nightshade.Config);
        }

        public static void Apply()
        {
            var harmony = new Harmony(Nightshade.ModId);
            MethodInfo Game1_ShouldDrawOnBuffer = typeof(Game1).GetMethod(
                    "ShouldDrawOnBuffer", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo Game1_ExitToTitle = typeof(Game1).GetMethod(
                    "ExitToTitle", BindingFlags.Public | BindingFlags.Static);
            harmony.Patch(Game1_ShouldDrawOnBuffer,
                    postfix: new HarmonyMethod(typeof(Patches),
                        "Game1_ShouldDrawOnBuffer_Postfix"));
            harmony.Patch(Game1_ExitToTitle,
                    postfix: new HarmonyMethod(typeof(Patches),
                        "Game1_ExitToTitle_Postfix"));
        }

    }
}
