using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Reflection;

namespace ichortower
{
    internal class GMCMIntegration
    {
        public static OpenMenuButton omb;

        public static void Setup()
        {
            var gmcmApi = Nightshade.instance.Helper.ModRegistry.GetApi
                    <IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcmApi is null) {
                return;
            }
            gmcmApi.Register(mod: Nightshade.instance.ModManifest,
                reset: () => {},
                save: () => {
                    Nightshade.instance.Helper.WriteConfig(Nightshade.Config);
                });
            gmcmApi.AddKeybindList(
                mod: Nightshade.instance.ModManifest,
                name: () => TR.Get("gmcm.MenuKeybind.name"),
                tooltip: () => TR.Get("gmcm.MenuKeybind.tooltip"),
                getValue: () => Nightshade.Config.MenuKeybind,
                setValue: (value) => {
                    Nightshade.Config.MenuKeybind = value;
                }
            );
            gmcmApi.AddTextOption(
                mod: Nightshade.instance.ModManifest,
                name: () => TR.Get("gmcm.TabBarWheelScroll.name"),
                tooltip: () => TR.Get("gmcm.TabBarWheelScroll.tooltip"),
                allowedValues: Enum.GetNames<ScrollDirection>(),
                getValue: () => Nightshade.Config.TabBarWheelScroll.ToString(),
                setValue: value => {
                    Nightshade.Config.TabBarWheelScroll = 
                            (ScrollDirection)Enum.Parse(typeof(ScrollDirection), value);
                }
            );
            omb = new OpenMenuButton();
            omb.text = TR.Get("gmcm.OpenMenu.buttonText");
            gmcmApi.AddComplexOption(
                mod: Nightshade.instance.ModManifest,
                name: () => TR.Get("gmcm.OpenMenu.name"),
                tooltip: () => TR.Get("gmcm.OpenMenu.tooltip"),
                draw: omb.Draw
            );
        }
    }

    internal class OpenMenuButton
    {
        public bool mouseLastFrame = false;
        public string text = "";
        public string sound = "drumkit6";
        public void Draw(SpriteBatch sb, Vector2 coords)
        {
            coords.Y -= 4;
            bool mouseThisFrame = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed;
            bool justClicked = (mouseThisFrame && !mouseLastFrame);
            mouseLastFrame = mouseThisFrame;
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();
            Vector2 textSize = Game1.dialogueFont.MeasureString(text);
            int Width = (int)textSize.X + 24;
            int Height = (int)textSize.Y + 4;
            Rectangle bounds = new((int)coords.X, (int)coords.Y, Width, Height);
            bool hovering = bounds.Contains(mouseX, mouseY);
            if (hovering && justClicked) {
                ui.ShaderMenu m = new();
                Game1.playSound(sound);
                // when in game, remove GMCM so world is visible.
                // at title menu, leave it open so UI is showing.
                if (Game1.gameMode == Game1.playingGameMode) {
                    Game1.activeClickableMenu = m;
                }
                else {
                    Game1.activeClickableMenu.SetChildMenu(m);
                }
            }
            Rectangle[] dests = ui.Widget.nineslice(bounds, 6, 6);
            ui.ButtonShared.drawFrame(hovering, sb, dests);
            sb.DrawString(Game1.dialogueFont, this.text,
                    new Vector2(coords.X+12, coords.Y+4), Game1.textColor);
        }
    }
}

namespace GenericModConfigMenu
{
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string> tooltip = null, Action beforeMenuOpened = null, Action beforeSave = null, Action afterSave = null, Action beforeReset = null, Action afterReset = null, Action beforeMenuClosed = null, Func<int> height = null, string fieldId = null);
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);
    }
}
