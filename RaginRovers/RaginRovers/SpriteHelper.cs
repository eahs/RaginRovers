using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using RaginRoversLibrary;

namespace RaginRovers
{
    delegate void Callback ();

    class TimedItem
    {
        //public int SpriteKey;
        public Callback cb;
        public float TimerElapsed;
        public float Time;

        public TimedItem(Callback callback, float time)
        {
            //this.SpriteKey = key;
            this.cb = callback;
            this.Time = time;
            this.TimerElapsed = 0;
        }
    }

    class SpriteHelper
    {
        private static SpriteHelper instance;
        private List<TimedItem> items;
        private GameObjectFactory factory;

        private SpriteHelper ()
        {
            items = new List<TimedItem>();
            factory = GameObjectFactory.Instance;
        }

        public void TriggerAnimation(GameObjectTypes type, Vector2 location, string textureassetname)
        {
            TriggerAnimation(type, location, textureassetname, 64);
        }

        public void TriggerAnimation(GameObjectTypes type, Vector2 location, string textureassetname, int depth)
        {
            int key = factory.Create((int)type, location, textureassetname, Vector2.Zero, 0f, 0f, 0f, depth);

            float animationTime = 1000 * factory.Objects[key].sprite.FrameTime * factory.Objects[key].sprite.FrameCount;

            RemoveAfter(key, animationTime);
            
        }

        public void TriggerAfter(Callback cb, float TriggerTime)
        {
            items.Add(new TimedItem(cb, TriggerTime));
        }

        public void RemoveAfter(int SpriteKey, float RemoveTime)
        {
            items.Add(new TimedItem(
                        delegate() 
                        {
                            GameObjectFactory.Instance.Remove(SpriteKey); 
                        }, 
                        RemoveTime));
        }

        public void Update(GameTime gameTime)
        {
            
            for (int i = items.Count - 1; i >= 0; i--)
            {
                items[i].TimerElapsed += gameTime.ElapsedGameTime.Milliseconds;

                if (items[i].TimerElapsed > items[i].Time)
                {
                    items[i].cb();
                    items.RemoveAt(i);
                }
            }
        }

        public static SpriteHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpriteHelper();
                }
                return instance;
            }
        }        
    }
}
