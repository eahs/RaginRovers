using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaginRoversLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

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
        private int plane = -1;
        public PlaneState planeState = PlaneState.NORMAL;
        public int direction = 1;
        public int planeSpeed = 5;
        public int currentPlane = 0;
        private bool isCreated = false;
        public int LengthAddedfromZoom = 1080;

        private SoundEffectInstance soundBuzz;

        public PlaneManager()
        {
            factory = GameObjectFactory.Instance;
        }

        public void CreatePlane()
        {
            // Already created plane?
            if (plane != -1)
            {
                factory.Objects[plane].sprite.Location = new Vector2(-LengthAddedfromZoom - factory.Objects[plane].sprite.BoundingBoxRect.Width, -650);
                return;
            }

            plane = factory.Create((int)RaginRovers.GameObjectTypes.PLANE,
                Vector2.Zero,
                "plane_with_banner",
                Vector2.Zero,
                0f,
                0f,
                0f);
            factory.Objects[plane].sprite.Location = new Vector2(-LengthAddedfromZoom - factory.Objects[plane].sprite.BoundingBoxRect.Width, -650);
            factory.Objects[plane].sprite.Scale = 2;
            factory.Objects[plane].saveable = false;

            soundBuzz = AudioManager.Instance.GetSoundEffectLooped("airplane");
            soundBuzz.Volume = 0.2f;
            soundBuzz.Play();

            isCreated = true;
        }

        public void Update(GameTime gametime)
        {
            if (!isCreated) return;

            soundBuzz.Pan = MathHelper.Clamp(factory.Objects[plane].sprite.Location.X, 0, (float)GameWorld.WorldWidth) / (float)GameWorld.WorldWidth;

            factory.Objects[plane].sprite.Location += new Vector2(planeSpeed * direction, 0);
            if (factory.Objects[plane].sprite.Location.X > 12234 - LengthAddedfromZoom)
            {
                direction = -1;
                currentPlane = (currentPlane + 1) % 4;
                //audioManager.SoundEffect("airplane").Play(0.2f, 0, 0);
                //factory.Objects[plane].sprite.flipType = Sprite.FlipType.HORIZONTAL;
            }
            if (factory.Objects[plane].sprite.Location.X < -LengthAddedfromZoom - factory.Objects[plane].sprite.BoundingBoxRect.Width)
            {
                direction = 1;
                currentPlane = (currentPlane + 1) % 4;
                //audioManager.SoundEffect("airplane").Play(0.2f, 0, 0);
                
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
