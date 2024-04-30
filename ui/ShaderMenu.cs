using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace ichortower.ui
{
    public class ShaderMenu : IClickableMenu
    {
        public static int defaultWidth = 400;
        public static int defaultHeight = 720 - 64;
        public static int defaultX = 32;
        public static int defaultY = 32;

        public static Texture2D IconTexture = null;

        private List<Widget> children = new();

        private Widget heldChild = null;
        private Widget keyedChild = null;

        public ShaderMenu()
            : this(defaultX, defaultY)
        {
        }

        public ShaderMenu(int x, int y)
            : base(x, y, defaultWidth, defaultHeight, true)
        {
            LoadIcons();
            AddChildWidgets();
        }

        public static void LoadIcons()
        {
            IconTexture = Game1.content.Load<Texture2D>($"Mods/{Nightshade.ModId}/Icons");
        }

        private void AddChildWidgets()
        {
            int y = 20;
            // give labels the same height (27) as checkboxes so they line up
            // vertically (default valign is center)
            var chk_enableColorizer = new Checkbox(20, y, this, false);
            var lbl_enableColorizer = new Label(this,
                    new Rectangle(56, y, 0, 27),
                    text: TR.Get("menu.EnableColorizer.Text"),
                    activate: chk_enableColorizer);
            y += chk_enableColorizer.Bounds.Height + 8;
            var chk_colorBySeason = new Checkbox(20, y, this, false);
            var lbl_colorBySeason = new Label(this,
                    new Rectangle(56, y, 0, 27),
                    text: TR.Get("menu.ColorizeBySeason.Text"),
                    hoverText: TR.Get("menu.ColorizeBySeason.Hover"),
                    activate: chk_colorBySeason);
            y += chk_colorBySeason.Bounds.Height + 16;
            var tbr_profiles = new TabBar(
                    new Rectangle(4, y, defaultWidth-8, 39),
                    new string[] {"Spring", "Summer", "Fall", "Winter"},
                    parent: this);
            y += tbr_profiles.Bounds.Height + 16;
            // same as before, give labels the same height (20) as the sliders.
            // this makes the labels render "too high" but it lines up
            var lbl_saturation = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Saturation.Text"));
            var sld_saturation = new Slider(this, 156, y, 0);
            y += lbl_saturation.Bounds.Height + 8;
            var lbl_lightness = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Lightness.Text"));
            var sld_lightness = new Slider(this, 156, y, 0);
            y += lbl_lightness.Bounds.Height + 8;
            var lbl_contrast = new Label(this,
                    new Rectangle(20, y, 128, 20),
                    text: TR.Get("menu.Contrast.Text"));
            var sld_contrast = new Slider(this, 156, y, 0);
            y += lbl_contrast.Bounds.Height + 16;

            int buttonY = y;

            var colorBalance = TR.Get("menu.ColorBalance.Hover");
            var lbl_cyan = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "C", hoverText: colorBalance);
            var lbl_red = new Label(this, new Rectangle(296, y, 24, 60),
                    text: "R", hoverText: colorBalance);
            var sld_redShadow = new Slider(this, 48, y, 0);
            var sld_redMidtone = new Slider(this, 48, y+20, 0);
            var sld_redHighlight = new Slider(this, 48, y+40, 0);
            y += 60 + 8;
            var lbl_magenta = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "M", hoverText: colorBalance);
            var lbl_green = new Label(this, new Rectangle(296, y, 24, 60),
                    text: "G", hoverText: colorBalance);
            var sld_greenShadow = new Slider(this, 48, y, 0);
            var sld_greenMidtone = new Slider(this, 48, y+20, 0);
            var sld_greenHighlight = new Slider(this, 48, y+40, 0);
            y += 60 + 8;
            var lbl_yellow = new Label(this, new Rectangle(20, y, 24, 60),
                    text: "Y", hoverText: colorBalance);
            var lbl_blue = new Label(this, new Rectangle(296, y, 24, 60),
                    text: "B", hoverText: colorBalance);
            var sld_blueShadow = new Slider(this, 48, y, 0);
            var sld_blueMidtone = new Slider(this, 48, y+20, 0);
            var sld_blueHighlight = new Slider(this, 48, y+40, 0);
            y += 60 + 16;
            var tbr_separator = new TabBar(
                    new Rectangle(4, y, defaultWidth-8, 2),
                    new string[] {}, parent: this);
            y += 2 + 16;

            var btn_revert = new IconButton(this, defaultWidth-47, buttonY,
                    iconIndex: 0, hoverText: TR.Get("menu.RevertButton.Hover"),
                    onClick: RevertCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_clear = new IconButton(this, defaultWidth-47, buttonY,
                    iconIndex: 1, hoverText: TR.Get("menu.ClearButton.Hover"),
                    onClick: ClearCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_copy = new IconButton(this, defaultWidth-47, buttonY,
                    iconIndex: 2, hoverText: TR.Get("menu.CopyButton.Hover"),
                    onClick: CopyCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;
            var btn_paste = new IconButton(this, defaultWidth-47, buttonY,
                    iconIndex: 3, hoverText: TR.Get("menu.PasteButton.Hover"),
                    onClick: PasteCurrentProfile);
            buttonY += IconButton.defaultHeight + 8;

            var chk_enableDepthOfField = new Checkbox(20, y, this, false);
            var lbl_enableDepthOfField = new Label(this,
                    new Rectangle(56, y, 0, 27),
                    text: TR.Get("menu.EnableDepthOfField.Text"),
                    activate: chk_enableDepthOfField);
            y += chk_enableDepthOfField.Bounds.Height + 16;
            var lbl_field = new Label(this,
                    new Rectangle(20, y, 96, 20),
                    text: TR.Get("menu.Field.Text"),
                    hoverText: TR.Get("menu.Field.Hover"));
            var sld_field = new Slider(this,
                    new Rectangle(126, y, 201, 20), 50, new int[]{0, 100});
            sld_field.ValueDelegate = sld_field.RenderAsFloat;
            y += lbl_field.Bounds.Height + 8;
            var lbl_ramp = new Label(this,
                    new Rectangle(20, y, 96, 20),
                    text: TR.Get("menu.Ramp.Text"),
                    hoverText: TR.Get("menu.Ramp.Hover"));
            var sld_ramp = new Slider(this,
                    new Rectangle(126, y, 201, 20), 20, new int[]{0, 50});
            sld_ramp.ValueDelegate = sld_ramp.RenderAsFloat;
            y += lbl_ramp.Bounds.Height + 8;
            var lbl_intensity = new Label(this,
                    new Rectangle(20, y, 96, 20),
                    text: TR.Get("menu.Intensity.Text"),
                    hoverText: TR.Get("menu.Intensity.Hover"));
            var sld_intensity = new Slider(this,
                    new Rectangle(126, y, 201, 20), 60, new int[]{0, 100});
            sld_intensity.ValueDelegate = sld_intensity.RenderAsFloat;
            y += lbl_intensity.Bounds.Height + 16;

            this.children.AddRange(new List<Widget>() {
                lbl_enableColorizer, chk_enableColorizer,
                lbl_colorBySeason, chk_colorBySeason,
                tbr_profiles,
                lbl_saturation, sld_saturation,
                lbl_lightness, sld_lightness,
                lbl_contrast, sld_contrast,
                lbl_cyan, lbl_red,
                sld_redShadow, sld_redMidtone, sld_redHighlight,
                lbl_magenta, lbl_green,
                sld_greenShadow, sld_greenMidtone, sld_greenHighlight,
                lbl_yellow, lbl_blue,
                sld_blueShadow, sld_blueMidtone, sld_blueHighlight,
                btn_revert, btn_clear, btn_copy, btn_paste,
                tbr_separator,
                lbl_enableDepthOfField, chk_enableDepthOfField,
                lbl_field, sld_field,
                lbl_ramp, sld_ramp,
                lbl_intensity, sld_intensity,
            });
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
                    if (child is Slider) {
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
            this.heldChild = null;
            foreach (var child in this.children) {
                child.InActiveState = false;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (this.keyedChild != null) {
                this.keyedChild.keyPress(key);
            }
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
        }

        public void ClearCurrentProfile()
        {
        }

        public void CopyCurrentProfile()
        {
        }

        public void PasteCurrentProfile()
        {
        }

    }
}
