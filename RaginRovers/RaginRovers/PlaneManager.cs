using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaginRoversLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaginRovers
{
    enum PlaneState
    {
        NORMAL,
        DIVEBOMB,
        BOMB
    }
    class PlaneManager
    {

        private static PlaneManager instance;
        private GameObjectFactory factory;
        private int plane;
        public PlaneState planeState = PlaneState.NORMAL;
        public int direction = 1;
        public int planeSpeed = 4;
        public int currentPlane = 0;

        public PlaneManager()
        {
            factory = GameObjectFactory.Instance;

            plane = factory.Create((int)RaginRovers.GameObjectTypes.PLANE,
                Vector2.Zero,
                "plane_with_banner",
                Vector2.Zero,
                0f,
                0f,
                0f);
            factory.Objects[plane].sprite.Location = new Vector2(-638 - factory.Objects[plane].sprite.BoundingBoxRect.Width, -650);
            factory.Objects[plane].sprite.Scale = 2;
            factory.Objects[plane].saveable = false;
        }
        public void Update(GameTime gametime)
        {
            factory.Objects[plane].sprite.Location += new Vector2(planeSpeed * direction, 0);
            if (factory.Objects[plane].sprite.Location.X > 5700 + 638)
            {
                direction = -1;
                currentPlane = (currentPlane + 1) % 4;
                //factory.Objects[plane].sprite.flipType = Sprite.FlipType.HORIZONTAL;
            }
            if (factory.Objects[plane].sprite.Location.X < -638 - factory.Objects[plane].sprite.BoundingBoxRect.Width)
            {
                direction = 1;
                currentPlane = (currentPlane + 1) % 4;
                //factory.Objects[plane].sprite.flipType = Sprite.FlipType.NONE;
            }

            factory.Objects[plane].sprite.Frame = currentPlane;
        }
        public static PlaneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlaneManager();
                }
                return instance;
            }
        }
    }
}
