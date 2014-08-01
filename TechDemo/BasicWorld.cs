using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharp2D;

namespace TechDemo
{
    public class BasicWorld : GenericWorld
    {
        public override string Name
        {
            get { return "worlds/world.json"; }
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();

            if (!Sprites.Contains(Program.player1))
                AddSprite(Program.player1);
            if (!Sprites.Contains(Program.player2))
                AddSprite(Program.player2);

            Program.player1.X = 8f * 16f;
            Program.player2.X = 26f * 16f;

            Program.player1.Y = 0f * 16f;
            Program.player2.Y = 0f * 16f;
        }
    }
}
