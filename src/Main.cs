using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Mods;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Enums;
using System;
using System.IO;
using System.Reflection;

namespace ichortower
{
    internal sealed class Nightshade : Mod
    {
        public static string ModId = null;
        public static Effect ColorShader = null;
        public static Effect DofShader = null;

        private static SpriteBatch sb = null;

        private static RenderTarget2D uiScreen = null;
        private static RenderTarget2D sceneScreen = null;
        private static bool usingColorizer = false;
        private static bool usingDepthOfField = false;

        public static ModConfig Config;

        public static Nightshade instance;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            ModId = this.ModManifest.UniqueID;
            try {
                byte[] stream = File.ReadAllBytes(Path.Combine(
                        helper.DirectoryPath, "assets/colorizer.mgfx"));
                Nightshade.ColorShader = new Effect(Game1.graphics.GraphicsDevice, stream);
                stream = File.ReadAllBytes(Path.Combine(
                        helper.DirectoryPath, "assets/depthoffield.mgfx"));
                Nightshade.DofShader = new Effect(Game1.graphics.GraphicsDevice, stream);
            }
            catch(Exception e) {
                Monitor.Log("Could not load a required shader!" +
                        " This mod will be disabled.", LogLevel.Error);
                Monitor.Log(e.ToString(), LogLevel.Error);
                return;
            }
            Nightshade.Config = helper.ReadConfig<ModConfig>();
            ApplyConfig(Nightshade.Config);

            var harmony = new Harmony(this.ModManifest.UniqueID);
            MethodInfo Game1_ShouldDrawOnBuffer = typeof(Game1).GetMethod(
                    "ShouldDrawOnBuffer",
                    BindingFlags.Public | BindingFlags.Instance);
            var post = new HarmonyMethod(typeof(Nightshade),
                    "Game1_ShouldDrawOnBuffer_Postfix");
            harmony.Patch(Game1_ShouldDrawOnBuffer,
                    postfix: post);

            sb = new SpriteBatch(Game1.graphics.GraphicsDevice);
            /*
            MethodInfo Game1__draw = typeof(Game1).GetMethod("_draw",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            var pre = new HarmonyMethod(typeof(InShade), "Game1__draw_Prefix");
            var post = new HarmonyMethod(typeof(InShade), "Game1__draw_Postfix");
            harmony.Patch(Game1__draw, prefix: pre, postfix: post);
            */
            /*
            MethodInfo Game1DrawWorld = typeof(Game1).GetMethod("DrawWorld",
                    BindingFlags.Public | BindingFlags.Instance);
            var pre = new HarmonyMethod(typeof(InShade), "Game1_DrawWorld_Prefix");
            var post = new HarmonyMethod(typeof(InShade), "Game1_DrawWorld_Postfix");
            harmony.Patch(Game1DrawWorld, prefix: pre, postfix: post);
            */
            /*
            MethodInfo target = typeof(SpriteBatch).GetMethod("Begin",
                    BindingFlags.Public | BindingFlags.Instance);
            var Postfix = new HarmonyMethod(typeof(InShade),
                    "SpriteBatch_Begin_Postfix");
            harmony.Patch(target, postfix: Postfix);
            */
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Specialized.LoadStageChanged += this.OnLoadStageChanged;
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Content.AssetReady += this.OnAssetReady;
        }

        public void ApplyConfig(ModConfig conf)
        {
            int index = conf.ColorizerActiveProfile;
            if (conf.ColorizeBySeason) {
                index = Game1.seasonIndex;
            }
            ColorizerPreset active = conf.ColorizerProfiles[index];
            ColorShader.Parameters["Saturation"].SetValue(active.Saturation);
            ColorShader.Parameters["Lightness"].SetValue(active.Lightness);
            ColorShader.Parameters["Contrast"].SetValue(active.Contrast);
            ColorShader.Parameters["ShadowRgb"].SetValue(new Vector3(
                    active.ShadowR, active.ShadowG, active.ShadowB));
            ColorShader.Parameters["MidtoneRgb"].SetValue(new Vector3(
                    active.MidtoneR, active.MidtoneG, active.MidtoneB));
            ColorShader.Parameters["HighlightRgb"].SetValue(new Vector3(
                    active.HighlightR, active.HighlightG, active.HighlightB));
            DofShader.Parameters["Field"].SetValue(conf.DepthOfFieldSettings.Field);
            DofShader.Parameters["Ramp"].SetValue(conf.DepthOfFieldSettings.Ramp);
            DofShader.Parameters["Intensity"].SetValue(conf.DepthOfFieldSettings.Intensity);

            usingColorizer = conf.ColorizerEnabled;
            usingDepthOfField = conf.DepthOfFieldEnabled;
        }

        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            GMCMIntegration.Setup();
        }

        public void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.Preloaded) {
                ApplyConfig(Nightshade.Config);
            }
        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo($"Mods/{ModId}/Icons")) {
                e.LoadFromModFile<Texture2D>("assets/icons.png", AssetLoadPriority.Medium);
            }
        }

        public void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            if (e.Name.IsEquivalentTo($"Mods/{ModId}/Icons")) {
                ui.ShaderMenu.LoadIcons();
            }
        }

        public void EnsureBuffers(bool reallocate = false)
        {
            int sw = Game1.game1.screen.Width;
            int sh = Game1.game1.screen.Height;
            if (reallocate || sceneScreen is null || 
                    (sceneScreen.Width != sw || sceneScreen.Height != sh)) {
                sceneScreen?.Dispose();
                sceneScreen = new(Game1.graphics.GraphicsDevice, sw, sh);
            }
            int uw = Game1.game1.uiScreen.Width;
            int uh = Game1.game1.uiScreen.Height;
            if (reallocate || uiScreen is null || 
                    (uiScreen.Width != uw || uiScreen.Height != uh)) {
                uiScreen?.Dispose();
                uiScreen = new(Game1.graphics.GraphicsDevice, uw, uh);
            }
        }

        // attempting to run the shader after other mods do their OnRendered
        // drawing (GMCM, AT, FS...)
        [EventPriority(EventPriority.Low - 10)]
        public void OnRendered(object sender, RenderedEventArgs e)
        {
            if (!usingColorizer) {
                return;
            }
            EnsureBuffers();
            // call End/Begin to flush any pending draws in the spritebatch.
            // otherwise, they won't be drawn until after our shader.
            // the parameters to Begin are known and are the same as the ones
            // SMAPI uses to open the spritebatch when it raises the Rendered
            // event.
            e.SpriteBatch.End();
            e.SpriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp);

            // save current render target for restoration later
            RenderTarget2D savedTarget = null;
            RenderTargetBinding[] rt = Game1.graphics.GraphicsDevice.GetRenderTargets();
            if (rt.Length > 0) {
                savedTarget = rt[0].RenderTarget as RenderTarget2D;
            }

            // apply the shader by rendering the two framebuffers twice each:
            // once to the appropriate back buffer, applying the shader, then
            // again to re-render it (no shader) back to where it was.
            // this is much more performant than trying to move or copy the
            // data in some other way.
            Game1.SetRenderTarget(sceneScreen);
            sb.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    effect: Nightshade.ColorShader);
            sb.Draw(texture: Game1.game1.screen,
                    position: Vector2.Zero,
                    color: Color.White);
            sb.End();
            Game1.SetRenderTarget(Game1.game1.screen);
            sb.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp);
            sb.Draw(texture: sceneScreen,
                    position: Vector2.Zero,
                    color: Color.White);
            sb.End();

            Game1.SetRenderTarget(uiScreen);
            Game1.game1.GraphicsDevice.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    effect: Nightshade.ColorShader);
            sb.Draw(texture: Game1.game1.uiScreen,
                    position: Vector2.Zero,
                    color: Color.White);
            sb.End();
            Game1.SetRenderTarget(Game1.game1.uiScreen);
            Game1.game1.GraphicsDevice.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp);
            sb.Draw(texture: uiScreen,
                    position: Vector2.Zero,
                    color: Color.White);
            sb.End();

            Game1.SetRenderTarget(savedTarget);
        }

        // this is much like OnRendered, but it's for the depth-of-field
        // shader, which applies only to the world layer.
        [EventPriority(EventPriority.Low - 10)]
        public void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!usingDepthOfField) {
                return;
            }
            EnsureBuffers();
            e.SpriteBatch.End();
            e.SpriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp);

            // save current render target for restoration later
            RenderTarget2D savedTarget = null;
            RenderTargetBinding[] rt = Game1.graphics.GraphicsDevice.GetRenderTargets();
            if (rt.Length > 0) {
                savedTarget = rt[0].RenderTarget as RenderTarget2D;
            }

            Nightshade.DofShader.Parameters["PitchX"]?.SetValue(
                    1f / (float)sceneScreen.Width);
            Nightshade.DofShader.Parameters["PitchY"]?.SetValue(
                    1f / (float)sceneScreen.Height);
            float ypos = Game1.player.getLocalPosition(Game1.viewport).Y;
            ypos += Game1.player.GetBoundingBox().Height / 2;
            ypos /= Game1.viewport.Height;
            Nightshade.DofShader.Parameters["Center"].SetValue(ypos);

            DofShader.CurrentTechnique = DofShader.Techniques[0];
            Game1.SetRenderTarget(sceneScreen);
            sb.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    effect: Nightshade.DofShader);
            sb.Draw(texture: Game1.game1.screen,
                    position: Vector2.Zero,
                    color: Color.White);
            sb.End();
            DofShader.CurrentTechnique = DofShader.Techniques[1];
            Game1.SetRenderTarget(Game1.game1.screen);
            sb.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    effect: Nightshade.DofShader);
            sb.Draw(texture: sceneScreen,
                    position: Vector2.Zero,
                    color: Color.White);
            sb.End();

            Game1.SetRenderTarget(savedTarget);
        }

        public static void Game1_ShouldDrawOnBuffer_Postfix(
                ref bool __result)
        {
            if (Game1.gameMode == Game1.playingGameMode) {
                __result = true;
            }
        }

        public void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (Game1.activeClickableMenu != null) {
                return;
            }
            if (Config.MenuKeybind.JustPressed()) {
                ui.ShaderMenu cfg = new();
                Game1.activeClickableMenu = cfg;
            }
        }

    }

}
