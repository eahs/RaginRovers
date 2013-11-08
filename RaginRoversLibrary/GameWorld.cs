using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

namespace RaginRoversLibrary
{
    public static class GameWorld
    {
        public static World world;  // Must be initialized
        private static int world_ofs_x = 0;
        private static int worldmin = 0;
        public static int worldmax = 0;
        private static int screen_width = 0;
        private static int screen_height = 0;
        private static float proportionGroundtoScreen = 1084f / 1280f;
        private static int heightofGround = 0;

        //9420+720 = 10140

        public static void Initialize(int worldmin, int worldmax, int screen_width, int screen_height, Vector2 gravity)
        {
            GameWorld.worldmin = worldmin;
            GameWorld.worldmax = worldmax;
            GameWorld.screen_height = screen_height;
            GameWorld.screen_width = screen_width;
            if (screen_height == 727)
            {
                heightofGround = 720 - 105; //hardcode workaround
            }
            else if (screen_height == 1080)
            {
                heightofGround = 1080 - 200; //hardcode workaround
            }
            heightofGround = (int)((float)screen_height * proportionGroundtoScreen);

            world = new World(gravity);
        }

        public static int ViewPortXOffset
        {
            get { return GameWorld.world_ofs_x; }
            set { GameWorld.world_ofs_x = value;  }
        }

        public static int WorldWidth
        {
            get
            {
                return worldmax;
            }
        }

        public static Tuple<double, double> ScreenToVector(double x, double y)
        {
            return new Tuple<double, double>(x, heightofGround - y);
        }

        public static Tuple<double, double> VectorToScreen(double x, double y)
        {
            return new Tuple<double, double>(x, heightofGround - y);
        }

        public static int HeightofGround
        {
            get
            {
                return heightofGround;
            }
        }

        public static float ProportionGroundtoScreen
        {
            get
            {
                return proportionGroundtoScreen;
            }
        }

        public static void Update(GameTime gameTime)
        {
            world.Step((float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001));
        }

    }
}
