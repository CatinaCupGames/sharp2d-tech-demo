using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Sharp2D;
using Sharp2D.Core.Interfaces;

namespace TechDemo
{
    public class SlowMoBar : Sprite, ILogical
    {
        private Player owner;
        private int lastTime;
        public SlowMoBar(Player owner)
        {
            this.owner = owner;
        }

        protected override void BeforeDraw()
        {
        }

        protected override void OnLoad()
        {
            Texture = Texture.NewTexture("poo");
            Texture.BlankTexture(64, 8);

            using (Graphics g = Graphics.FromImage(Texture.Bitmap))
            {
                using (Brush b = new SolidBrush(Color.White))
                {
                    g.FillRectangle(b, new Rectangle(0, 0, 64, 8));
                }
            }

            Width = Texture.TextureWidth;
            Height = Texture.TextureHeight;

            lastTime = owner.SlowDownTime;
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

        public void Update()
        {
            if (lastTime != owner.SlowDownTime)
            {
                Width = ((float)((float)owner.SlowDownTime / (float)Player.TOTAL_SLOWDOWN_TIME) * 64f);
                lastTime = owner.SlowDownTime;
            }
        }
    }
}
