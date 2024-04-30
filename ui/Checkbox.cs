using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ichortower.ui
{
    public class Checkbox : Widget
    {
        public static string Sound = "drumkit6";
        public bool Value;

        public Checkbox(int x, int y, IClickableMenu parent = null, bool Value = false)
            : base(parent)
        {
            this.Bounds = new Rectangle(x, y, 27, 27);
            this.Value = Value;
        }

        public override void draw(SpriteBatch b)
        {
            Texture2D tex = Game1.mouseCursors;
            Rectangle sourceRect = new(this.Value ? 236 : 227, 425, 9, 9);
            Rectangle destRect = new(
                    (this.parent?.xPositionOnScreen ?? 0) + this.Bounds.X,
                    (this.parent?.yPositionOnScreen ?? 0) + this.Bounds.Y,
                    this.Bounds.Width, this.Bounds.Height);
            b.Draw(tex, color: Color.White,
                    sourceRectangle: sourceRect,
                    destinationRectangle: destRect);
        }

        public override void click(int x, int y, bool playSound = true)
        {
            this.Value = !this.Value;
            if (playSound) {
                Game1.playSound(Checkbox.Sound);
            }
        }
    }
}
