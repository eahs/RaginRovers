using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using RaginRoversLibrary;
using System.IO;
using FarseerPhysics.Dynamics;

namespace RaginRovers
{
    class MapEditor
    {
        public bool EditMode = false;
        bool KeyDown = false, MouseDown = false;
        Keys Key = Keys.None;
        int DragSprite = -1; // Which sprite are we dragging around
        Vector2 DragOffset = Vector2.Zero;

        GameWindow window;
        ClientNetworking client;
        GameObjectFactory factory;
        CannonManager cannonManager;
        List<CannonGroups> cannonGroups;
        int screenconfiguration = 0;

        public MapEditor(GameWindow window, ClientNetworking client, CannonManager cannonManager, List<CannonGroups> cannonGroups, int screenconfiguration)
        {
            
            factory = GameObjectFactory.Instance;

            this.window = window;
            this.client = client;
            this.cannonManager = cannonManager;
            this.cannonGroups = cannonGroups;
            this.screenconfiguration = screenconfiguration;
            
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState msState = Mouse.GetState();

            Vector2 ms = Vector2.Transform(new Vector2(msState.X, msState.Y), Matrix.Invert(camera.GetViewMatrix(Vector2.One)));
            window.Title = "Local Mouse> X: " + msState.X + " Y: " + msState.Y + ", World Mouse> X: " + ms.X + ", Y: " + ms.Y + ", Zoom: " + camera.Zoom;

            DetectKeyPress(kb, Keys.OemTilde);
            DetectKeyPress(kb, Keys.D1);  // Record if this key is pressed
            DetectKeyPress(kb, Keys.D2);  // Record if this key is pressed
            DetectKeyPress(kb, Keys.D3);
            DetectKeyPress(kb, Keys.D4);
            DetectKeyPress(kb, Keys.D5);
            DetectKeyPress(kb, Keys.D6);
            DetectKeyPress(kb, Keys.D7);
            DetectKeyPress(kb, Keys.D8);
            DetectKeyPress(kb, Keys.D9);
            DetectKeyPress(kb, Keys.D0);
            DetectKeyPress(kb, Keys.P);
            DetectKeyPress(kb, Keys.R);
            DetectKeyPress(kb, Keys.B);
            DetectKeyPress(kb, Keys.Delete);
            DetectKeyPress(kb, Keys.M);
            DetectKeyPress(kb, Keys.L);
            DetectKeyPress(kb, Keys.U);
            DetectKeyPress(kb, Keys.Enter);
            DetectKeyPress(kb, Keys.Space);
            DetectKeyPress(kb, Keys.N);
            DetectKeyPress(kb, Keys.J);
            DetectKeyPress(kb, Keys.Q);


            if (KeyDown)
            {
                if (kb.IsKeyUp(Key))
                {
                    switch (Key)
                    {
                        ////////////////////////////////////////////////////
                        case Keys.Space:
                            if (!EditMode)
                            { 
                                //probably will get annoying when working on project and testing bugs
                                if (screenconfiguration == 1 || screenconfiguration == 2) //get rid of =2 part after testing
                                {
                                    cannonManager.ChangeCannonState(cannonGroups[1]);

                                    if (cannonGroups[1].cannonState == CannonState.WAITING)
                                    {
                                        client.SendMessage("action=shoot;cannonGroup=1;rotation=" + cannonGroups[1].Rotation + ";power=" + cannonGroups[1].Power + ";Screen=" + Game1.ScreenConfiguration);
                                        SunManager.Instance.Mood = SunMood.OPENSMILE;
                                    }
                                }
                                if (screenconfiguration == 3)
                                {
                                    cannonManager.ChangeCannonState(cannonGroups[0]);

                                    if (cannonGroups[0].cannonState == CannonState.WAITING)
                                    {
                                        client.SendMessage("action=shoot;cannonGroup=0;rotation=" + cannonGroups[0].Rotation + ";power=" + cannonGroups[0].Power + ";Screen=" + Game1.ScreenConfiguration);
                                        SunManager.Instance.Mood = SunMood.OPENSMILE;

                                    }
                                }
                                
                            }
                            break;

                        case Keys.N:
                            if (!EditMode)
                            {
                                
                            }
                            break;

                        case Keys.OemTilde:
                            //if (screenconfiguration == 2)
                                EditMode = !EditMode;
                            //camera.Zoom = 1f;
                            
                            window.Title = "Ragin Rovers " + (EditMode ? " | EDITING MODE" : "");
                            break;

                        case Keys.Q:

                            if (EditMode)
                            {
                                int puff = factory.Create((int)GameObjectTypes.PUFF, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[puff].sprite.PhysicsBody.Mass = 30;
                                factory.Objects[puff].sprite.PhysicsBody.Restitution = 0.4f;
                                factory.Objects[puff].saveable = false;
                            }

                            break;
                        
                        case Keys.D1:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                int dog = factory.Create((int)GameObjectTypes.DOG, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[dog].sprite.PhysicsBody.Mass = 30;
                                factory.Objects[dog].sprite.PhysicsBody.Restitution = 0.4f;
                            }
                            if (screenconfiguration == 2)
                                LoadMap(1);

                            break;

                        case Keys.D2:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                int cat = factory.Create((int)GameObjectTypes.CAT, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[cat].sprite.PhysicsBody.Mass = 30;
                                factory.Objects[cat].sprite.PhysicsBody.Restitution = 0.8f;
                            }
                            if (screenconfiguration == 2)
                                LoadMap(2);

                            break;

                        case Keys.D3:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD1, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);

                                factory.Objects[board].sprite.OnCollision += new OnCollisionEventHandler(CollisionEvents.wood_OnCollision);
                            }
                            if (screenconfiguration == 2)
                                LoadMap(3);

                            break;

                        case Keys.D4:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD2, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[board].sprite.OnCollision += new OnCollisionEventHandler(CollisionEvents.wood_OnCollision);
                            }
                            if (screenconfiguration == 2)
                                LoadMap(4);

                            break;

                        case Keys.D5:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD3, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[board].sprite.OnCollision += new OnCollisionEventHandler(CollisionEvents.wood_OnCollision);
                            }
                            if (screenconfiguration == 2)
                                LoadMap(5);

                            break;

                        case Keys.D6:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD4, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[board].sprite.OnCollision += new OnCollisionEventHandler(CollisionEvents.wood_OnCollision);
                            }
                            if (screenconfiguration == 2)
                                LoadMap(1);

                            break;

                        case Keys.D7:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_LEFT, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }
                            if (screenconfiguration == 2)
                                LoadMap(1);

                            break;

                        case Keys.D8:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_MIDDLE, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }
                            if (screenconfiguration == 2)
                                LoadMap(1);

                            break;

                        case Keys.D9:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_RIGHT, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }
                            if (screenconfiguration == 2)
                                LoadMap(1);

                            break;
                        case Keys.D0:

                            if (screenconfiguration != 2 && EditMode)
                            {
                                factory = cannonManager.CreateCannonStuff(factory, new Vector2(ms.X /*+ camera.Position.X*/ - 95, ms.Y - 80), camera, false, ref cannonGroups);

                            }
                            if (screenconfiguration == 2)
                                LoadMap(1);

                            break;


                        case Keys.P:

                            if (EditMode)
                            {
                                factory = cannonManager.CreateCannonStuff(factory, new Vector2(ms.X, ms.Y), camera, true, ref cannonGroups);

                            }

                            break;
                        case Keys.J:

                            cannonManager.CreateCannonStuff(factory, new Vector2(0, 500), camera, true, ref cannonGroups); //need how to figure out location
                            cannonManager.CreateCannonStuff(factory, new Vector2(500, 500), camera, false, ref cannonGroups); //need how to figure out location
                            break;

                        case Keys.B:

                            int boom = factory.Create((int)GameObjectTypes.BOOM, new Vector2((int)ms.X + camera.Position.X - 95, (int)ms.Y - 80), "boom", new Vector2(0, 0), 0, 0f, 0f);

                            break;

                        case Keys.R:

                            if (EditMode && MouseDown && DragSprite != -1)
                            {
                                if (factory.Objects[DragSprite].sprite.Rotation == 0)
                                    factory.Objects[DragSprite].sprite.Rotation = MathHelper.PiOver2;
                                else
                                    factory.Objects[DragSprite].sprite.Rotation = 0;
                            }

                            break;

                        case Keys.Delete:

                            if (EditMode && MouseDown && DragSprite != -1)
                            {
                                factory.Remove(DragSprite);
                                DragSprite = -1;
                            }

                            break;

                        case Keys.L:
                            if (screenconfiguration == 2)
                            {
                                // Plane
                                client.SendMessage("action=plane;monkeys=awesome"); // lol

                                // Clouds
                                Random rand = new Random();
                                for (int i = 0; i < 15; i++)
                                {
                                    Vector2 v = new Vector2(rand.Next(-1024, GameWorld.WorldWidth + 1024), rand.Next(-800, 0));
                                    Vector2 velocity = new Vector2(5 + rand.Next(0, 15), 0);

                                    client.SendMessage("action=createother;gotype=" + (int)(RaginRovers.GameObjectTypes.CLOUD1 + rand.Next(0, 4)) + ";textureassetname=clouds;location.x=" + v.X + ";location.y=" + v.Y + ";rotation=0;upperBounds=0;lowerBounds=0;velocity.x=" + velocity.X + ";velocity.y=" + velocity.Y);
                                }

                            }

                            break;

                        case Keys.M:

                            string objlist = factory.Serialize();

                            using (StreamWriter outfile = new StreamWriter(@"map.txt"))
                            {
                                outfile.Write(objlist);
                            }

                            break;
                    }

                    KeyDown = false;
                    Key = Keys.None;
                }
            }

            if (!EditMode)
            {
                for (int i = 0; i < cannonGroups.Count; i++)
                {
                    factory = cannonManager.ManipulateCannons(factory, cannonGroups[i]);
                }
            }

            cannonManager.Update(gameTime, factory, cannonGroups);

            if (EditMode)
            {
                if (msState.LeftButton == ButtonState.Pressed && !MouseDown)
                {
                    MouseDown = true;

                    foreach (int key in factory.Objects.Keys)
                    {
                        Sprite sprite = factory.Objects[key].sprite;
                        if (sprite.IsBoxColliding(new Rectangle((int)ms.X /*+ (int)camera.Position.X*/, (int)ms.Y, 1, 1)))
                        {
                            DragSprite = key;
                            DragOffset = new Vector2(ms.X, ms.Y) - sprite.Location;
                        }
                    }
                }

                if (MouseDown && DragSprite != -1)
                {
                    factory.Objects[DragSprite].sprite.Location = new Vector2(ms.X, ms.Y) - DragOffset;
                }

                if (msState.LeftButton == ButtonState.Released)
                {
                    MouseDown = false;
                    DragSprite = -1;
                }
            }
        }

        protected void DetectKeyPress(KeyboardState kb, Keys key)
        {
            if (kb.IsKeyDown(key))
            {
                Key = key;
                KeyDown = true;
            }
        }

        public void LoadMap(int MapNumber)
        {
            //clear list of objects
            List<int> NotSaveable = new List<int>();
            foreach (int keys in factory.Objects.Keys)
            {
                if (factory.Objects[keys].saveable)
                {
                    NotSaveable.Add(keys);
                }
            }
            for (int i = 0; i < NotSaveable.Count; i++)
            {
                factory.Objects.Remove(NotSaveable[i]);
            }

            //load in map
            StreamReader infile = new StreamReader("map" + MapNumber + ".txt") ;
            
            using (infile)
            {
                string objs = infile.ReadToEnd();
                string[] lines = objs.Split('\n');

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length > 0)
                    {
                        string[] fields = lines[i].Split('\t');

                        double vx = (float)Convert.ToDouble(fields[2]);
                        double vy = (float)Convert.ToDouble(fields[3]);

                        Tuple<double, double> v = GameWorld.VectorToScreen(vx, vy);

                        client.SendMessage("action=create;gotype=" + Convert.ToInt32(fields[1]) + ";textureassetname=" + fields[4] + ";location.x=" + v.Item1 + ";location.y=" + v.Item2 + ";rotation=" + (float)Convert.ToDouble(fields[5]) + ";upperBounds=" + 0f + ";lowerBounds=" + 0f);

                        /*
                        factory.Create(Convert.ToInt32(fields[1]),
                                       new Vector2((float)Convert.ToDouble(fields[2]), (float)Convert.ToDouble(fields[3])),
                                       fields[4],
                                       Vector2.Zero,
                                       (float)Convert.ToDouble(fields[5]),
                                       0f,
                                       0f);
                        */
                    }
                }
            }
        }
    }
}
