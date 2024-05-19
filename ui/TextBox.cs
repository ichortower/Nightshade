using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;

namespace ichortower.ui
{
    public class TextBox : Widget
    {
        private string _text = "";
        public string Text {
            get {
                return _text;
            }
            set {
                _text = value;
                CursorIndex = CursorIndex; // use the setter
            }
        }

        private int _cursorIndex = 0;
        public int CursorIndex {
            get {
                return _cursorIndex;
            }
            set {
                _cursorIndex = Math.Max(0, Math.Min(value, _text.Length - 1));
            }
        }

        public TextBox(IClickableMenu parent, Rectangle bounds, string name)
            : base(parent, bounds, name)
        {
        }

        public override void draw(SpriteBatch b)
        {
            Rectangle screenb = new(
                    (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X,
                    (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y,
                    this.Bounds.Width, this.Bounds.Height);
            Rectangle[] sources = Widget.nineslice(
                    new Rectangle(227, 425, 9, 9), 2, 2);
            Rectangle[] targets = Widget.nineslice(screenb, 4, 4);
            for (int i = 0; i < sources.Length; ++i) {
                b.Draw(Game1.mouseCursors, color: Color.White,
                        sourceRectangle: sources[i],
                        destinationRectangle: targets[i]);
            }
        }

        public override void click(int x, int y, bool playSound = true)
        {
        }

        public override void clickHold(int x, int y)
        {
        }

        public override void keyPress(Keys key)
        {
        }
    }
}
