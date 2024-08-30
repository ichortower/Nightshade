using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Linq;

namespace ichortower.ui
{
    public class TabBar : Widget
    {
        public static string IndoorLabel = ":I:";
        public static string AddLabel = "+";
        public static int GetTextWidth(string text) {
            if (text.Equals(AddLabel)) {
                return 24;
            }
            return (int)Game1.smallFont.MeasureString(text).X;
        }
        public static int TabWidth(string text) {
            return GetTextWidth(text) + 7;
        }

        public string[] Labels;

        private int _focusedIndex;
        public int FocusedIndex {
            get {
                return _focusedIndex;
            }
            set {
                _focusedIndex = Math.Max(0, Math.Min(value, Labels.Length-1));
            }
        }

        private int _draggingIndex;
        public int DraggingIndex {
            get {
                return _draggingIndex;
            }
            set {
                _draggingIndex = Math.Max(-1, Math.Min(value, Labels.Length-1));
            }
        }
        public int DraggingOffset;

        private int _aboutToDragIndex;
        private int _aboutToDragX;


        public TabBar(Rectangle bounds, string[] labels, IClickableMenu parent)
            : base(parent, bounds)
        {
            this.Labels = labels;
            this.FocusedIndex = 0;
            this.DraggingIndex = -1;
        }

        public override void draw(SpriteBatch b)
        {
            int x = (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X;
            int y = (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y;
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(12, 264, 36, 4),
                    destinationRectangle: new Rectangle(x, y+this.Bounds.Height-3, 21, 2));
            int xoff = 20;
            int[] widths = Labels.Select(l => TabWidth(l)).ToArray();
            int dragposition = 0;
            int dragwidth = 0;
            if (DraggingIndex >= 0) {
                dragposition = Game1.getMouseX() - DraggingOffset -
                        (this.parent?.xPositionOnScreen ?? 0);
                dragwidth = widths[DraggingIndex];
            }
            for (int i = 0; i < Labels.Length; ++i) {
                if (i == DraggingIndex) {
                    continue;
                }
                int drawpos = xoff;
                if (DraggingIndex >= 0 && xoff + widths[i] > dragposition + dragwidth/2) {
                    drawpos += dragwidth;
                }
                drawTab(b, Labels[i], drawpos, i == FocusedIndex);
                xoff += widths[i];
            }
            if (DraggingIndex >= 0) {
                drawTab(b, Labels[DraggingIndex], dragposition, DraggingIndex == FocusedIndex);
                xoff += dragwidth;
            }
            xoff += drawUnfocusedTab(b, AddLabel, xoff);
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(12, 264, 36, 4),
                    destinationRectangle: new Rectangle(x+xoff-1, y+this.Bounds.Height-3, this.Bounds.Width-xoff+1, 2));
        }

        public int drawTab(SpriteBatch b, string text, int xoff, bool focused)
        {
            if (focused) {
                return drawFocusedTab(b, text, xoff);
            }
            return drawUnfocusedTab(b, text, xoff);
        }

        public int drawFocusedTab(SpriteBatch b, string text, int xoff)
        {
            bool useIcon = text.Equals(TabBar.IndoorLabel);
            int textWidth = GetTextWidth(text);
            int x = (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X + xoff;
            int y = (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y;
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(48, 268, 4, 36),
                    destinationRectangle: new Rectangle(x, y+1, 2, this.Bounds.Height-3));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(12, 264, 36, 4),
                    destinationRectangle: new Rectangle(x+1, y, textWidth+5, 2));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(48, 268, 4, 36),
                    destinationRectangle: new Rectangle(x+textWidth+5, y+1, 2, this.Bounds.Height-3));
            if (useIcon) {
                b.Draw(ShaderMenu.IconTexture, color: Game1.textColor,
                        sourceRectangle: new Rectangle(110, 0, 22, 22),
                        destinationRectangle: new Rectangle(x+10, y+6, 22, 22));
            }
            else {
                Utility.drawTextWithShadow(b, text, Game1.smallFont,
                        new Vector2(x+4, y+2), Game1.textColor);
            }
            return textWidth + 7;
        }

        public int drawUnfocusedTab(SpriteBatch b, string text, int xoff)
        {
            bool useIcon = text.Equals(TabBar.IndoorLabel);
            int textWidth = GetTextWidth(text);
            int x = (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X + xoff;
            int y = (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y;
            Rectangle hoverbox = new(x, y, textWidth+7, this.Bounds.Height);
            bool hovering = hoverbox.Contains(Game1.getMouseX(), Game1.getMouseY());

            Color textAlpha = Game1.textColor;
            if (!hovering) {
                textAlpha.A /= 2;
            }
            if (useIcon) {
                b.Draw(ShaderMenu.IconTexture, color: textAlpha,
                        sourceRectangle: new Rectangle(110, 0, 22, 22),
                        destinationRectangle: new Rectangle(x+10, y+10, 22, 22));
            }
            else {
                int minioffset = text.Equals(TabBar.AddLabel) ? 8 : 4;
                Utility.drawTextWithShadow(b, text, Game1.smallFont,
                        new Vector2(x+minioffset, y+6), textAlpha);
            }

            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(64, 324, 4, 52),
                    destinationRectangle: new Rectangle(x, y+5, 2, this.Bounds.Height-7));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(68, 376, 52, 4),
                    destinationRectangle: new Rectangle(x+1, y+4, textWidth+5, 2));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(64, 324, 4, 52),
                    destinationRectangle: new Rectangle(x+textWidth+5, y+5, 2, this.Bounds.Height-7));
            b.Draw(Game1.menuTexture, color: Color.White,
                    sourceRectangle: new Rectangle(12, 264, 36, 4),
                    destinationRectangle: new Rectangle(x-1, y+this.Bounds.Height-3, textWidth+9, 2));
            return textWidth + 7;
        }

        public override void click(int x, int y, bool playSound = true)
        {
            _aboutToDragIndex = -1;
            int start = 20;
            int dx = 0;
            for (int i = 0; i < Labels.Length; ++i) {
                dx = TabWidth(Labels[i]);
                if (x > start && x < start + dx) {
                    FocusedIndex = i;
                    _aboutToDragIndex = i;
                    _aboutToDragX = x;
                    DraggingOffset = x - start;
                    if (this.parent is ShaderMenu m) {
                        m.onChildChange(this);
                    }
                    return;
                }
                start += dx;
            }
            dx = TabWidth(AddLabel);
            if (x > start && x < start + dx) {
                Nightshade.instance.Monitor.Log("Adding new profile", LogLevel.Warn);
            }
        }

        public override void clickHold(int x, int y)
        {
            if (DraggingIndex >= 0 || _aboutToDragIndex < 0) {
                return;
            }
            if (Math.Abs(_aboutToDragX - x) > 8) {
                DraggingIndex = _aboutToDragIndex;
            }
        }

        public override void clickRelease(int x, int y, bool playSound = true)
        {
            if (DraggingIndex < 0) {
                return;
            }
            int startIndex = DraggingIndex;
            int dropIndex = startIndex;
            int dropx = x - DraggingOffset - (this.parent?.xPositionOnScreen ?? 0);
            int start = 20;
            int dx = 0;
            for (int i = 0; i < Labels.Length; ++i) {
                dx = TabWidth(Labels[i]);
                if (x < start + dx) {
                    dropIndex = i;
                    break;
                }
                start += dx;
            }
            DraggingIndex = -1;
            if (startIndex == dropIndex) {
                return;
            }
            Nightshade.instance.Monitor.Log($"releasing drag {startIndex} -> {dropIndex}", LogLevel.Warn);
        }

        public override void scrollWheel(int direction)
        {
            int prev = FocusedIndex;
            int dir = (int)Nightshade.Config.TabBarWheelScroll;
            FocusedIndex += Math.Sign(direction) * dir;
            if (prev != FocusedIndex && parent is ShaderMenu m) {
                m.onChildChange(this);
            }
        }

        public override void keyPress(Keys key)
        {
            int prev = this.FocusedIndex;
            if (key == Keys.Right || Game1.options.doesInputListContain(Game1.options.moveRightButton, key)) {
                this.FocusedIndex += 1;
            }
            else if (key == Keys.Left || Game1.options.doesInputListContain(Game1.options.moveLeftButton, key)) {
                this.FocusedIndex -= 1;
            }
            if (prev != this.FocusedIndex && this.parent is ShaderMenu m) {
                m.onChildChange(this);
            }
        }
    }
}
