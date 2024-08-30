using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace ichortower.ui
{
    public class ShaderMenu : IClickableMenu
    {
        public static int defaultWidth = 420;
        public static int defaultHeight = 720 - 64;
        public static int defaultX = 32;
        public static int defaultY = 32;

        public static Texture2D IconTexture = null;

        private List<Widget> children = new();
        // references to the child widgets that have interop
        private TabBar profileSwitcher = null;
        private IconButton previewButton = null;
        //private Checkbox bySeasonToggle = null;
        //private Checkbox byIndoorsToggle = null;

        private Widget heldChild = null;
        private Widget keyedChild = null;

        private List<NightshadeProfile> ProfileInitialStates = new();
        private List<NightshadeProfile> ProfileActiveStates = new();
        private ColorizerProfile CopyPasteBuffer = null;

        public ShaderMenu()
            : this(defaultX, defaultY)
        {
        }

        public ShaderMenu(int x, int y)
            : base(x, y, defaultWidth, defaultHeight, true)
        {
            LoadIcons();
            AddChildWidgets();
            LoadConfigSettings();
            exitFunction = delegate {
                Nightshade.instance.ApplyConfig(Nightshade.Config);
            };
        }

        public static void LoadIcons()
        {
            IconTexture = Game1.content.Load<Texture2D>($"Mods/{Nightshade.ModId}/Icons");
        }

        private void AddChildWidgets()
        {
            int y = 20;
            int x = 20;

            string[] names = Nightshade.Config.Profiles.Select(
                    (p, i) => $" {i+1} ").ToArray();
            var tbr_profiles = new TabBar(
                    new Rectangle(4, y, defaultWidth-8, 39),
                    names,
                    parent: this);
            profileSwitcher = tbr_profiles;
            y += tbr_profiles.Bounds.Height + 12;

            var txt_conditions = new TextBox(this,
                    new Rectangle(x, y-3, 0, 33),
                    name: "Conditions");
            var lbl_conditions = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.Conditions.Text"),
                    hoverText: TR.Get("menu.Conditions.Hover"),
                    activate: txt_conditions);
            txt_conditions.Bounds.X += lbl_conditions.Bounds.Width + 8;
            txt_conditions.Bounds.Width = defaultWidth - 20 - txt_conditions.Bounds.X;
            y += txt_conditions.Bounds.Height + 4;

            // give labels the same height (27) as checkboxes so they line up
            // vertically (default valign is center)
            var lbl_colorizer = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.ApplyTo.Text"));
            x += lbl_colorizer.Bounds.Width + 8;
            var chk_colorizeWorld = new Checkbox(this, x, y, "ColorizeWorld");
            x += chk_colorizeWorld.Bounds.Width + 8;
            var lbl_colorizeWorld = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.ColorizeWorld.Text"),
                    hoverText: TR.Get("menu.ColorizeWorld.Hover"),
                    activate: chk_colorizeWorld);
            x += lbl_colorizeWorld.Bounds.Width + 8;
            var chk_colorizeUI = new Checkbox(this, x, y, "ColorizeUI");
            x += chk_colorizeUI.Bounds.Width + 8;
            var lbl_colorizeUI = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.ColorizeUI.Text"),
                    hoverText: TR.Get("menu.ColorizeUI.Hover"),
                    activate: chk_colorizeUI);
            x += lbl_colorizeUI.Bounds.Width + 8;
            var chk_colorizeTitle = new Checkbox(this, x, y, "ColorizeTitleScreen");
            x += chk_colorizeTitle.Bounds.Width + 8;
            var lbl_colorizeTitle = new Label(this,
                    new Rectangle(x, y, 0, 27),
                    text: TR.Get("menu.ColorizeTitleScreen.Text"),
                    hoverText: TR.Get("menu.ColorizeTitleScreen.Hover"),
                    activate: chk_colorizeTitle);
            y += lbl_colorizer.Bounds.Height + 16;

            // same as before, give labels the same height (20) as the sliders.
            // this makes the labels render "too high" but it lines up
            var lbl_saturation = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Saturation.Text"));
            var sld_saturation = new Slider(this, 156, y, name: "Saturation");
            y += lbl_saturation.Bounds.Height + 8;
            var lbl_lightness = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Lightness.Text"));
            var sld_lightness = new Slider(this, 156, y, name: "Lightness");
            sld_lightness.Range = new int[]{-80, 80};
            y += lbl_lightness.Bounds.Height + 8;
            var lbl_contrast = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Contrast.Text"));
            var sld_contrast = new Slider(this, 156, y, name: "Contrast");
            sld_contrast.Range = new int[]{-80, 80};
            y += lbl_contrast.Bounds.Height + 16;

            int buttonY = y;

            var colorBalance = TR.Get("menu.ColorBalance.Hover");
            var lbl_cyan = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "C", hoverText: colorBalance);
            var lbl_red = new Label(this, new Rectangle(308, y, 24, 60),
                    text: "R", hoverText: colorBalance);
            var sld_redShadow = new Slider(this, 48, y, name: "ShadowR");
            var sld_redMidtone = new Slider(this, 48, y+20, name: "MidtoneR");
            var sld_redHighlight = new Slider(this, 48, y+40, name: "HighlightR");
            y += 60 + 8;
            var lbl_magenta = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "M", hoverText: colorBalance);
            var lbl_green = new Label(this, new Rectangle(308, y, 24, 60),
                    text: "G", hoverText: colorBalance);
            var sld_greenShadow = new Slider(this, 48, y, name: "ShadowG");
            var sld_greenMidtone = new Slider(this, 48, y+20, name: "MidtoneG");
            var sld_greenHighlight = new Slider(this, 48, y+40, name: "HighlightG");
            y += 60 + 8;
            var lbl_yellow = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "Y", hoverText: colorBalance);
            var lbl_blue = new Label(this, new Rectangle(308, y, 24, 60),
                    text: "B", hoverText: colorBalance);
            var sld_blueShadow = new Slider(this, 48, y, name: "ShadowB");
            var sld_blueMidtone = new Slider(this, 48, y+20, name: "MidtoneB");
            var sld_blueHighlight = new Slider(this, 48, y+40, name: "HighlightB");
            y += 60 + 16;

            // centering based on knowing how tall the buttons are
            buttonY += 7;
            var btn_revert = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 0, hoverText: TR.Get("menu.RevertButton.Hover"),
                    onClick: RevertCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_clear = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 1, hoverText: TR.Get("menu.ClearButton.Hover"),
                    onClick: ClearCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_copy = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 2, hoverText: TR.Get("menu.CopyButton.Hover"),
                    onClick: CopyCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_paste = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 3, hoverText: TR.Get("menu.PasteButton.Hover"),
                    onClick: PasteCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_preview = new IconButton(this, defaultWidth-50, buttonY,
                    iconIndex: 4, hoverText: TR.Get("menu.PreviewButton.Hover"));
            btn_preview.Name = "PreviewButton";
            btn_preview.ActiveIconIndex = 5;
            btn_preview.ReportUpdates = true;
            previewButton = btn_preview;

            var chk_enableDepthOfField = new Checkbox(this, 20, y, "DepthOfField");
            var lbl_enableDepthOfField = new Label(this,
                    new Rectangle(56, y, 0, 27),
                    text: TR.Get("menu.EnableDepthOfField.Text"),
                    hoverText: TR.Get("menu.EnableDepthOfField.Hover"),
                    activate: chk_enableDepthOfField);
            y += chk_enableDepthOfField.Bounds.Height + 16;
            var lbl_field = new Label(this,
                    new Rectangle(20, y, 96, 20),
                    text: TR.Get("menu.Field.Text"),
                    hoverText: TR.Get("menu.Field.Hover"));
            var sld_field = new Slider(this, new Rectangle(126, y, 201, 20),
                    name: "Field", range: new int[]{0, 100});
            sld_field.ValueDelegate = sld_field.FloatRenderer(denom:100f);
            y += lbl_field.Bounds.Height + 8;
            var lbl_intensity = new Label(this,
                    new Rectangle(20, y, 96, 20),
                    text: TR.Get("menu.Intensity.Text"),
                    hoverText: TR.Get("menu.Intensity.Hover"));
            var sld_intensity = new Slider(this, new Rectangle(126, y, 201, 20),
                    name: "Intensity", range: new int[]{0, 100});
            sld_intensity.ValueDelegate = sld_intensity.FloatRenderer(denom:10f);
            y += lbl_intensity.Bounds.Height + 16;

            var btn_save = new TextButton(this, 0, 0,
                    text: TR.Get("menu.Save.Text"), onClick: SaveSettings);
            btn_save.Bounds.X = defaultWidth/2 - btn_save.Bounds.Width/2;
            btn_save.Bounds.Y = defaultHeight - btn_save.Bounds.Height - 8;

            this.children.AddRange(new List<Widget>() {
                tbr_profiles,
                lbl_conditions, txt_conditions,
                lbl_colorizer, lbl_colorizeWorld, chk_colorizeWorld,
                lbl_colorizeUI, chk_colorizeUI,
                lbl_colorizeTitle, chk_colorizeTitle,
                lbl_saturation, sld_saturation,
                lbl_lightness, sld_lightness,
                lbl_contrast, sld_contrast,
                lbl_cyan, lbl_red,
                sld_redShadow, sld_redMidtone, sld_redHighlight,
                lbl_magenta, lbl_green,
                sld_greenShadow, sld_greenMidtone, sld_greenHighlight,
                lbl_yellow, lbl_blue,
                sld_blueShadow, sld_blueMidtone, sld_blueHighlight,
                btn_revert, btn_clear, btn_copy, btn_paste, btn_preview,
                lbl_enableDepthOfField, chk_enableDepthOfField,
                lbl_field, sld_field,
                lbl_intensity, sld_intensity,
                btn_save,
            });
        }

        public void LoadConfigSettings()
        {
            ProfileInitialStates.Clear();
            ProfileActiveStates.Clear();
            foreach (var p in Nightshade.Config.Profiles) {
                ProfileInitialStates.Add(p.Clone());
                ProfileActiveStates.Add(p.Clone());
            }
            profileSwitcher.FocusedIndex = Nightshade.InitialMenuIndex;
            var profile = ProfileInitialStates[Nightshade.InitialMenuIndex];
            LoadToggles(profile);
            LoadColorizerProfile(profile.ColorSettings);
            LoadDepthOfFieldProfile(profile.DepthOfField);
        }

        public void LoadToggles(NightshadeProfile profile)
        {
            foreach (var child in this.children) {
                if (child is Checkbox ch) {
                    switch (ch.Name) {
                    case "ColorizeWorld":
                        ch.Value = profile.ColorizeWorld;
                        break;
                    case "ColorizeUI":
                        ch.Value = profile.ColorizeUI;
                        break;
                    case "ColorizeTitleScreen":
                        ch.Value = profile.ColorizeTitleScreen;
                        break;
                    case "DepthOfField":
                        ch.Value = (profile.EnableToyShader == ToyShader.DepthOfField);
                        break;
                    }
                }
                if (child is TextBox tb) {
                    switch (tb.Name) {
                    case "Conditions":
                        tb.Text = profile.Conditions;
                        break;
                    }
                }
            }
        }

        public void LoadColorizerProfile(ColorizerProfile set)
        {
            foreach (var child in this.children) {
                if (child is Slider ch) {
                    switch (ch.Name) {
                    case "Saturation":
                        ch.Value = (int)(set.Saturation * 100);
                        break;
                    case "Lightness":
                        ch.Value = (int)(set.Lightness * 100);
                        break;
                    case "Contrast":
                        ch.Value = (int)(set.Contrast * 100);
                        break;
                    case "ShadowR":
                        ch.Value = (int)(set.ShadowR * 100);
                        break;
                    case "ShadowG":
                        ch.Value = (int)(set.ShadowG * 100);
                        break;
                    case "ShadowB":
                        ch.Value = (int)(set.ShadowB * 100);
                        break;
                    case "MidtoneR":
                        ch.Value = (int)(set.MidtoneR * 100);
                        break;
                    case "MidtoneG":
                        ch.Value = (int)(set.MidtoneG * 100);
                        break;
                    case "MidtoneB":
                        ch.Value = (int)(set.MidtoneB * 100);
                        break;
                    case "HighlightR":
                        ch.Value = (int)(set.HighlightR * 100);
                        break;
                    case "HighlightG":
                        ch.Value = (int)(set.HighlightG * 100);
                        break;
                    case "HighlightB":
                        ch.Value = (int)(set.HighlightB * 100);
                        break;
                    }
                }
            }
        }

        public void LoadDepthOfFieldProfile(DepthOfFieldProfile set)
        {
            foreach (var child in this.children) {
                if ((child is Slider ch)) {
                    switch (ch.Name) {
                    case "Field":
                        ch.Value = (int)(set.Field * 100);
                        break;
                    case "Intensity":
                        ch.Value = (int)(set.Intensity * 10);
                        break;
                    }
                }
            }
        }

        public void SaveSettings()
        {
            ModConfig built = new();
            built.MenuKeybind = Nightshade.Config.MenuKeybind;
            foreach (var profile in ProfileActiveStates) {
                built.Profiles.Add(profile.Clone());
            }
            Nightshade.Config = built;
            Nightshade.instance.Helper.WriteConfig(Nightshade.Config);
            HUDMessage m = new("Nightshade: config saved.", HUDMessage.newQuest_type);
            m.timeLeft = 2000;
            m.type = $"{Nightshade.ModId}_SaveToast";
            Game1.addHUDMessage(m);
        }

        public override void draw(SpriteBatch b)
        {
            this.drawFrame(b, xPositionOnScreen, yPositionOnScreen,
                    width, height);
            base.draw(b);
            foreach (var child in this.children) {
                child.draw(b);
            }
            // only apply hover states and tooltips if not dragging a slider
            if (this.heldChild is null) {
                int modx = Game1.getMouseX() - this.xPositionOnScreen;
                int mody = Game1.getMouseY() - this.yPositionOnScreen;
                foreach (var child in this.children) {
                    child.InHoverState = child.Bounds.Contains(modx, mody);
                    if (child.InHoverState) {
                        if (child.HoverText?.Length > 0) {
                            var split = child.HoverText.Split("^");
                            string lines = Game1.parseText((split.Length > 1 ? split[1] : split[0]),
                                    Game1.smallFont, 280);
                            string title = (split.Length > 1 ? split[0] : null);
                            IClickableMenu.drawHoverText(b, font: Game1.smallFont,
                                    text: lines, boldTitleText: title);
                        }
                    }
                    else {
                        child.InActiveState = false;
                    }
                }
            }
            base.drawMouse(b);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            int modx = x - this.xPositionOnScreen;
            int mody = y - this.yPositionOnScreen;
            foreach (var child in this.children) {
                if (child.Bounds.Contains(modx, mody)) {
                    child.InActiveState = true;
                    child.click(modx, mody, playSound);
                    if (child is Slider || child is TabBar ||
                            child.Name == "PreviewButton") {
                        this.heldChild = child;
                    }
                    this.keyedChild = child;
                    break;
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            if (this.heldChild != null) {
                int modx = x - this.xPositionOnScreen;
                int mody = y - this.yPositionOnScreen;
                this.heldChild.clickHold(modx, mody);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            foreach (var child in this.children) {
                child.InActiveState = false;
            }
            if (this.heldChild != null) {
                int modx = x - this.xPositionOnScreen;
                int mody = y - this.yPositionOnScreen;
                this.heldChild.clickRelease(modx, mody);
            }
            this.heldChild = null;
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (this.keyedChild != null) {
                this.keyedChild.keyPress(key);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            int modx = Game1.getMouseX() - xPositionOnScreen;
            int mody = Game1.getMouseY() - yPositionOnScreen;
            foreach (var child in children) {
                if (child.Bounds.Contains(modx, mody)) {
                    child.scrollWheel(direction);
                    break;
                }
            }
        }

        public void onChildChange(Widget child)
        {
            NightshadeProfile current = ProfileActiveStates[profileSwitcher.FocusedIndex];
            if (child == profileSwitcher) {
                LoadToggles(current);
                LoadColorizerProfile(current.ColorSettings);
                LoadDepthOfFieldProfile(current.DepthOfField);
            }
            // change the current profile along with the widget
            else if (child is Slider sl) {
                switch (sl.Name) {
                case "Saturation":
                    current.ColorSettings.Saturation = sl.Value / 100f;
                    break;
                case "Lightness":
                    current.ColorSettings.Lightness = sl.Value / 100f;
                    break;
                case "Contrast":
                    current.ColorSettings.Contrast = sl.Value / 100f;
                    break;
                case "ShadowR":
                    current.ColorSettings.ShadowR = sl.Value / 100f;
                    break;
                case "ShadowG":
                    current.ColorSettings.ShadowG = sl.Value / 100f;
                    break;
                case "ShadowB":
                    current.ColorSettings.ShadowB = sl.Value / 100f;
                    break;
                case "MidtoneR":
                    current.ColorSettings.MidtoneR = sl.Value / 100f;
                    break;
                case "MidtoneG":
                    current.ColorSettings.MidtoneG = sl.Value / 100f;
                    break;
                case "MidtoneB":
                    current.ColorSettings.MidtoneB = sl.Value / 100f;
                    break;
                case "HighlightR":
                    current.ColorSettings.HighlightR = sl.Value / 100f;
                    break;
                case "HighlightG":
                    current.ColorSettings.HighlightG = sl.Value / 100f;
                    break;
                case "HighlightB":
                    current.ColorSettings.HighlightB = sl.Value / 100f;
                    break;
                case "Field":
                    current.DepthOfField.Field = sl.Value / 100f;
                    break;
                case "Intensity":
                    current.DepthOfField.Intensity = sl.Value / 10f;
                    break;
                }
            }
            else if (child is Checkbox ch) {
                switch (ch.Name) {
                case "ColorizeWorld":
                    current.ColorizeWorld = ch.Value;
                    break;
                case "ColorizeUI":
                    current.ColorizeUI = ch.Value;
                    break;
                case "ColorizeTitleScreen":
                    current.ColorizeTitleScreen = ch.Value;
                    break;
                case "DepthOfField":
                    current.EnableToyShader = (ch.Value ?
                            ToyShader.DepthOfField : ToyShader.None);
                    break;
                }
            }
            else if (child is TextBox tx) {
                // FIXME check for validity here dear god
                current.Conditions = tx.Text;
            }

            // Build a minimal config to apply in Main, by just sending the
            // current profile (TODO: and optional locked profile?) and
            // nulling out its Conditions so it always applies.
            // Start with current, then swap in color data from initial if
            // preview is on
            ModConfig built = new();
            var p = current.Clone();
            if (previewButton.InActiveState) {
                p.ColorSettings = ProfileInitialStates[profileSwitcher.FocusedIndex]
                        .ColorSettings.Clone();
                p.DepthOfField = ProfileInitialStates[profileSwitcher.FocusedIndex].DepthOfField;
            }
            p.Conditions = null;
            built.Profiles.Add(p);
            Nightshade.instance.ApplyConfig(built);
        }

        public void drawFrame(SpriteBatch b, int x, int y, int w, int h)
        {
            Texture2D tex = Game1.menuTexture;
            Rectangle[] sources = Widget.nineslice(new Rectangle(64, 320, 60, 60), 8, 8);
            Rectangle[] dests = Widget.nineslice(new Rectangle(x, y, w, h), 8, 8);
            for (int i = 0; i < sources.Length; ++i) {
                b.Draw(tex, color: Color.White,
                        sourceRectangle: sources[i],
                        destinationRectangle: dests[i]);
            }
        }

        public void RevertCurrentProfile()
        {
            NightshadeProfile current = ProfileActiveStates[profileSwitcher.FocusedIndex];
            NightshadeProfile initial = ProfileInitialStates[profileSwitcher.FocusedIndex];
            current.ColorSettings = initial.ColorSettings.Clone();
            LoadColorizerProfile(current.ColorSettings);
            onChildChange(null);
        }

        public void ClearCurrentProfile()
        {
            NightshadeProfile current = ProfileActiveStates[profileSwitcher.FocusedIndex];
            current.ColorSettings = new();
            LoadColorizerProfile(current.ColorSettings);
            onChildChange(null);
        }

        public void CopyCurrentProfile()
        {
            NightshadeProfile current = ProfileActiveStates[profileSwitcher.FocusedIndex];
            CopyPasteBuffer = current.ColorSettings.Clone();
        }

        public void PasteCurrentProfile()
        {
            if (CopyPasteBuffer is null) {
                return;
            }
            NightshadeProfile current = ProfileActiveStates[profileSwitcher.FocusedIndex];
            current.ColorSettings = CopyPasteBuffer.Clone();
            LoadColorizerProfile(current.ColorSettings);
            onChildChange(null);
        }

    }
}
