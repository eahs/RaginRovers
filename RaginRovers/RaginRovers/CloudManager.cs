using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaginRoversLibrary;
using Microsoft.Xna.Framework;

namespace RaginRovers
{
    class CloudManager
    {
        GameObjectFactory factory;
        List<int> clouds;
        Random rand;

        public CloudManager()
        {
            rand = new Random(10); // We have to make sure all systems follow the same random pattern
            factory = GameObjectFactory.Instance;
            clouds = new List<int>();

            for (int i = 0; i < 15; i++)
            {
                int cloud = factory.Create(
                         (int)RaginRovers.GameObjectTypes.CLOUD1 + rand.Next(0, 5),
                         new Vector2(
                             rand.Next(-1024, GameWorld.WorldWidth + 1024),
                             rand.Next(-800, 0)),
                         "clouds",
                         new Vector2(5 + rand.Next(0, 15), 0),
                         0,
                         0f,
                         0f, 100);

                clouds.Add(
                                cloud
                    );

                factory.Objects[cloud].saveable = false;
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < clouds.Count; i++)
            {
                Sprite cloud = factory.Objects[clouds[i]].sprite;

                cloud.Update(gameTime);
                if (cloud.Location.X + cloud.BoundingBoxRect.Width < -1024 || cloud.Location.X > GameWorld.WorldWidth + 1024)
                {
                    cloud.Location = new Vector2(-1024, MathHelper.Clamp(cloud.Location.Y + rand.Next(-50, 50), 0, 300 ));
                   
                    
                }
            }
        }

    }
}
