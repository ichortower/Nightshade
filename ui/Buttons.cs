using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ichortower.ui
{
    /*
    public class TextButton : Widget
    {
        public TextButton(IClickableMenu parent, int x, int y, IClickableMenu parent = null, bool Value = false)
            : base(parent)
        {
            this.Bounds = new Rectangle(x, y, 27, 27);
            this.Value = Value;
        }
    }
    */

    public class IconButton : Widget
    {
        public static int defaultWidth = 30;
        public static int defaultHeight = 30;
        public static int iconXY = 22;
        public static string Sound = "drumkit6";
        public static Texture2D IconTexture = null;

        public int IconIndex = 0;
        public Action ClickDelegate = null;

        public IconButton(IClickableMenu parent, int x, int y,
                int iconIndex, string hoverText = null, Action onClick = null)
            : this(parent, new Rectangle(x, y, defaultWidth, defaultHeight),
                    iconIndex, hoverText, onClick)
        {
        }

        public IconButton(IClickableMenu parent, Rectangle bounds,
                int iconIndex, string hoverText = null, Action onClick = null)
            : base(parent, bounds)
        {
            IconIndex = iconIndex;
            HoverText = hoverText;
            ClickDelegate = onClick;
            IconTexture = ShaderMenu.IconTexture;
        }

        public override void click(int x, int y, bool playSound = true)
        {
            if (playSound) {
                Game1.playSound(Checkbox.Sound);
            }
            ClickDelegate?.Invoke();
        }

        public override void draw(SpriteBatch b)
        {
            Rectangle screenb = new(
                    (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X,
                    (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y,
                    this.Bounds.Width, this.Bounds.Height);
            drawFrame(b, screenb.X, screenb.Y, screenb.Width, screenb.Height);
            int offset = (screenb.Width - iconXY) / 2;
            b.Draw(IconTexture, color: Game1.textColor,
                    sourceRectangle: new Rectangle(IconIndex*iconXY, 0, iconXY, iconXY),
                    destinationRectangle: new Rectangle(screenb.X+offset, screenb.Y+offset, iconXY, iconXY));
        }

        public void drawFrame(SpriteBatch b, int x, int y, int w, int h)
        {
            int boxX = InHoverState && !InActiveState ? 267 : 256;
            Rectangle[] sources = Widget.nineslice(new Rectangle(boxX, 256, 10, 10), 2, 2);
            Rectangle[] dests = Widget.nineslice(new Rectangle(x, y, w, h), 4, 4);
            for (int i = 0; i < sources.Length; ++i) {
                b.Draw(Game1.mouseCursors, color: Color.White,
                        sourceRectangle: sources[i],
                        destinationRectangle: dests[i]);
            }
        }
    }
}
