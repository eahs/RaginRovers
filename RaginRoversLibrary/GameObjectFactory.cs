using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using System.Runtime;

namespace RaginRoversLibrary
{

    public class GameObject : IComparable
    {
        public int id;
        public int typeid;

        public int depth;  // default depth is 64
        public string textureassetname;  // Asset name for the texture this object uses
        public Sprite sprite;  // You may never ever ever ever ever store a reference to this sprite directly
                               // if you want to reference this GameObject do it by id #
        public bool saveable;
        public int collisioncount;
        public int collisiontime; // Time of the last collision

        public bool alive;

        public GameObject()
        {
            saveable = true;
            depth = 64;
            alive = true;
            collisioncount = 0;
            collisiontime = 0;
        }

        public GameObject(int id, int typeid)
        {
            sprite = null;
            this.id = id;
            this.typeid = typeid;
        }

        public int CompareTo(Object other)
        {
            return ((GameObject)other).depth.CompareTo(this.depth);
        }
    }

    public delegate Sprite CreateSprite (Vector2 location,
                                            Texture2D texture,
                                            Vector2 velocity,
                                            float rotation);

    public class GameObjectFactory
    {
        private static GameObjectFactory instance;
        private int lastid;
        private Dictionary<int, GameObject> objects;
        private List<int> sortedobjects;
        private Dictionary<int, CreateSprite> creators;
        private TextureManager textureManager;


        // Making the object constructor private ensures nobody else can create a "NEW" gameobjectfactory
        private GameObjectFactory()
        {
            lastid = 0;
            this.objects = new Dictionary<int, GameObject>();
            this.creators = new Dictionary<int, CreateSprite>();
            this.sortedobjects = new List<int>();
            /*
             *                     // Default creators
            this.creators.Add(int.CAT, SpriteCreators.CreateCat);
            this.creators.Add(int.DOG, SpriteCreators.CreateDog);

             * */

        }

        public void Initialize(TextureManager textureManager)
        {
            this.textureManager = textureManager;
        }

        public void AddCreator(int gotype, CreateSprite cs)
        {
            if (!this.creators.ContainsKey(gotype))
            {
                this.creators.Add(gotype, cs);
            }
        }

        public int Create(int gotype,
                            Vector2 location,
                            string textureassetname,
                            Vector2 velocity,
                            float rotation,
                            float upperBounds,
                            float lowerBounds)
        {
            return this.Create(gotype, location, textureassetname, velocity, rotation, upperBounds, lowerBounds, 64);
        }

        public int Create(  int gotype, 
                            Vector2 location,
                            string textureassetname,
                            Vector2 velocity,
                            float rotation,
                            float upperBounds,
                            float lowerBounds,
                            int depth)
        {
            lastid++;

            GameObject go = new GameObject();
            
            go.id = lastid;
            go.depth = depth;
            go.typeid = gotype;
            go.textureassetname = textureassetname;
            go.sprite = null;

            if (this.creators.ContainsKey(gotype))
            {
                go.sprite = this.creators[gotype](location, textureManager.Texture(textureassetname), velocity, rotation);

                go.sprite.UpperRotationBounds = upperBounds;
                go.sprite.LowerRotationBounds = lowerBounds;
                go.sprite.PhysicsBody.UserData = go;

                this.objects.Add(lastid, go);


            }
            else
                return -1;

            /*
            this.sortedobjects = this.objects.ToList();

            this.sortedobjects.Sort(
                    delegate(KeyValuePair<int, GameObject> firstPair, KeyValuePair<int, GameObject> nextPair)
                    {
                        return firstPair.Value.CompareTo(nextPair.Value);
                    }
                );
            */
            bool inserted = false;
            for (int i = 0; i < this.sortedobjects.Count; i++)
            {
                if (go.depth > this.objects[this.sortedobjects[i]].depth)
                {
                    this.sortedobjects.Insert(i, lastid);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
                this.sortedobjects.Add(lastid);

            return lastid;
        }

        public void UpdateSortedObjectList()
        {
            sortedobjects.Clear();
            foreach (int key in objects.Keys)
            {
                bool inserted = false;
                for (int i = 0; i < this.sortedobjects.Count; i++)
                {
                    if (objects[key].depth > this.objects[this.sortedobjects[i]].depth)
                    {
                        this.sortedobjects.Insert(i, key);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                    this.sortedobjects.Add(key);
            }

            

        }

        public void Remove(int objectid)
        {
            if (this.objects.ContainsKey(objectid))
            {
                if (objects[objectid].sprite != null)
                {
                    /*List<Fixture> FixtureList = new List<Fixture>();
                    foreach (Fixture fixture in objects[objectid].sprite.PhysicsBody.FixtureList)
                        FixtureList.Add(fixture);
                    for (int i = 0; i < FixtureList.Count; i++)
                        objects[objectid].sprite.PhysicsBody.DestroyFixture(FixtureList[i]);*/
                    //objects[objectid].sprite.PhysicsBody.DestroyFixture(objects[objectid].sprite.PhysicsBodyFixture);

                    GameWorld.world.RemoveBody(objects[objectid].sprite.PhysicsBody); //isn't destroying the body we want
                    objects[objectid].sprite.PhysicsBody.Dispose();
                    objects[objectid].sprite.Destroy();
                    objects[objectid].sprite = null;
                    objects[objectid] = null;
                }

                this.objects.Remove(objectid);


                this.sortedobjects.Remove(objectid);
                
               
            }
        }

        public void RemoveAll()
        {
            List<int> Kays = new List<int>();
            foreach (int key in this.objects.Keys)
            {
                Kays.Add(key);
            }
            foreach (int key in Kays)
            {
                if (this.objects.ContainsKey(key))
                {
                    if (objects[key].sprite != null)
                    {
                        objects[key].sprite.Destroy();
                        objects[key].sprite = null;
                        objects[key] = null;
                    }

                    this.objects.Remove(key);

                    this.sortedobjects.Remove(key);


                }
            }
        }

        public string Serialize()
        {
            string lines = "";

            foreach (int key in objects.Keys)
            {
                Tuple<double, double> v = GameWorld.ScreenToVector((double)objects[key].sprite.Location.X, (double)objects[key].sprite.Location.Y);

                if (objects[key].saveable)
                    lines +=    objects[key].id + "\t" +
                                objects[key].typeid + "\t" +
                                v.Item1 + "\t" +
                                v.Item2 + "\t" +
                                objects[key].textureassetname + "\t" +
                                objects[key].sprite.Rotation +
                                "\n"
                             ;
            }

           
            return lines;
        }

        public Dictionary<int, GameObject> Objects
        {
            get
            {
                return this.objects;
            }
        }

        public List<int> SortedObjectsList
        {
            get
            {
                return this.sortedobjects;
            }
        }

        
        // Guarantee only one instance
        public static GameObjectFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObjectFactory();
                }
                return instance;
            }
        }
    }
}
