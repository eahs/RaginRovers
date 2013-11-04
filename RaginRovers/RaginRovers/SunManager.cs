using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaginRoversLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RaginRovers
{
    enum SunMood
    {
        GRIN = 1,
        OPENSMILE = 0,
        TOOTHYSMILE = 2,
        OHFACE = 4,
        MAD = 3
    }

    class SunManager
    {
        private static SunManager instance;
        private int sun;
        private double suncount = 0;
        private float animationTimer = 0;
        private float moodTimer = 0;
        private GameObjectFactory factory;
        private Vector2 defaultLocation;
        private Random rand;

        private SunMood mood = SunMood.GRIN;

        private SunManager()
        {
            factory = GameObjectFactory.Instance;

            rand = new Random(System.Environment.TickCount);
            defaultLocation = new Vector2(GameWorld.WorldWidth / 2, -800);

            this.sun = factory.Create((int)GameObjectTypes.SUN, defaultLocation, "sun", Vector2.Zero, 0f, 0f, 0f, 256);
            factory.Objects[sun].saveable = false;
        }

        public SunMood Mood
        {
            get { return this.mood; }
            set
            {
                if (moodTimer > 750f)
                {
                    moodTimer = 0f;
                    mood = value;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            Sprite sunsprite = factory.Objects[sun].sprite;

            sunsprite.Location = defaultLocation;
            animationTimer += (float)gameTime.ElapsedGameTime.Milliseconds;
            moodTimer += (float)gameTime.ElapsedGameTime.Milliseconds;

            if (mood == SunMood.GRIN || mood == SunMood.OPENSMILE || mood == SunMood.TOOTHYSMILE)
            {
                
                if (animationTimer > 5f)
                {
                    animationTimer = 0f;

                    double rotamt = 1;
                    if (mood == SunMood.OPENSMILE)
                        rotamt = 2;
                    else if (mood == SunMood.TOOTHYSMILE)
                        rotamt = 3;

                    suncount += (Math.PI / 180) * rotamt;
                    sunsprite.Rotation = (float)((Math.PI / 10) * Math.Sin(suncount));
                }
            }
            else if (mood == SunMood.OHFACE)
            {
                sunsprite.Rotation = 0;
            }
            else if (mood == SunMood.MAD)
            {
                sunsprite.Rotation = 0;

                if (animationTimer > 30f)
                {
                    animationTimer = 0f;
                    sunsprite.Location = defaultLocation + new Vector2(rand.Next(-5, 5), rand.Next(-5, 5));
                }
            }

            sunsprite.Frame = (int)mood;
            
        }

        // Guarantee only one instance
        public static SunManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SunManager();
                }
                return instance;
            }
        }
    }
}
