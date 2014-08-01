using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Sharp2D;

namespace TechDemo
{
    class Program
    {
        public static BasicWorld world;
        public static Player player1;
        public static Player player2;
        public static void Main(string[] args)
        {
            Screen.DisplayScreenAsync();

            SetupPlayers();

            world = new BasicWorld();

            world.Load();

            world.Display();

            Random random = new Random();

            Light light = world.AddLight(player1.X, player1.Y, 0.7f, 150f, System.Drawing.Color.Tomato, LightType.DynamicPointLight);
            Light light2 = world.AddLight(player2.X, player2.Y, 0.7f, 150f, System.Drawing.Color.Aquamarine, LightType.DynamicPointLight);

            player1.Attach(light);
            player2.Attach(light2);

            world.AmbientBrightness = 0.3f;

            Screen.Camera.Follow2d(player1);
            Screen.Camera.Follow2d(player2);
            Screen.Camera.Z = 150f;
        }

        public static void SetupPlayers()
        {
            player1 = new Player();
            player2 = new Player();

            player2.player2 = true;
            player2.lastDirection = -1;
            player1.lastDirection = 1;
        }
    }
}
