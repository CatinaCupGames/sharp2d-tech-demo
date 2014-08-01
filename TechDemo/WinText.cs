using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Sharp2D;

namespace TechDemo
{
    public class WinText : Sprite
    {
        private string text;
        public WinText(string text)
        {
            this.text = text;
        }
        protected override void BeforeDraw()
        {
        }

        protected override void OnLoad()
        {
            Texture = Texture.NewTexture("win");
            Texture.BlankTexture(128, 128);
            Width = Texture.TextureWidth;
            Height = Texture.TextureHeight;

            X = -Screen.Camera.X;
            Y = Screen.Camera.Y;

            Layer = 0.5f;

            using (Graphics g = Graphics.FromImage(Texture.Bitmap))
            {
                using (Brush b = new SolidBrush(Color.White))
                {

                    StringFormat format = new StringFormat();
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                    g.DrawString(text, new Font("NBP Informa FiveSix", 24f, FontStyle.Regular, GraphicsUnit.Point), b, new RectangleF(0, 0, 128, 128), format);
                }
            }
        }

        protected override void OnUnload()
        {
        }

        protected override void OnDispose()
        {
        }

        protected override void OnDisplay()
        {
        }
    }
}
