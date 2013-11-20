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

        public void TriggerFadeUp(GameObjectTypes type, Vector2 location, string textureassetname)
        {
            int key = factory.Create((int)type, location, textureassetname, new Vector2(0, -100), 0f, 0f, 0f, 32);

            factory.Objects[key].sprite.FrameTime = 0.1f;
            factory.Objects[key].sprite.Fade = true;

            items.Add(new TimedItem(
            delegate()
            {
                GameObjectFactory.Instance.Remove(key);
            },
            1000));

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

        public void TriggerEndRound(MapEditor mapEditor, int mapNumber)
        {
            ScoreKeeper.Instance.CalculateWinner();

            int LeftSide = 0;
            int RightSide = 0;

            if (ScoreKeeper.Instance.winningSituation == ScoreKeeper.WinningSituations.LeftWins)
            {
                LeftSide = (int)GameObjectTypes.YOUWIN;
                RightSide = (int)GameObjectTypes.YOULOSE;
            }
            if (ScoreKeeper.Instance.winningSituation == ScoreKeeper.WinningSituations.RightWins)
            {
                LeftSide = (int)GameObjectTypes.YOULOSE;
                RightSide = (int)GameObjectTypes.YOUWIN;
            }
            if (ScoreKeeper.Instance.winningSituation == ScoreKeeper.WinningSituations.Tie)
            {
                LeftSide = (int)GameObjectTypes.YOUWIN;
                RightSide = (int)GameObjectTypes.YOUWIN;
            }

            //left
            int key = factory.Create(LeftSide, Vector2.Zero, "scoresheet", Vector2.Zero, 0f, 0f, 0f, 64);
            factory.Objects[key].sprite.Location = new Vector2(-1080 + ((12234 / 3) / 2) - (factory.Objects[key].sprite.BoundingBoxRect.Width / 2), -1217 + (2700 / 2)/*should be height of screen*/ - (factory.Objects[key].sprite.BoundingBoxRect.Height / 2));
            RemoveAfterGameEnding(key, 10000, mapEditor, mapNumber);
            
            //right
            int key2 = factory.Create(RightSide, Vector2.Zero, "scoresheet", Vector2.Zero, 0f, 0f, 0f, 64);
            factory.Objects[key2].sprite.Location = new Vector2(-1080 + (2 * 12234 / 3) + ((12234 / 3) / 2) - (factory.Objects[key2].sprite.BoundingBoxRect.Width / 2), -1217 + (2700 / 2)/*should be height of screen*/ - (factory.Objects[key2].sprite.BoundingBoxRect.Height / 2));
            RemoveAfter(key2, 10000);
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
        public void RemoveAfterGameEnding(int SpriteKey, float RemoveTime, MapEditor mapEditor, int mapNumber)
        {
            items.Add(new TimedItem(
                        delegate() 
                        {
                            GameObjectFactory.Instance.Remove(SpriteKey);
                            mapEditor.LoadMap(mapNumber);
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
