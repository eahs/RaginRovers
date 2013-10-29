using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using RaginRoversLibrary;
using System.IO;

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

        public MapEditor(GameWindow window, ClientNetworking client, CannonManager cannonManager, List<CannonGroups> cannonGroups)
        {
            
            factory = GameObjectFactory.Instance;

            this.window = window;
            this.client = client;
            this.cannonManager = cannonManager;
            this.cannonGroups = cannonGroups;
            
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState msState = Mouse.GetState();

            Vector2 ms = Vector2.Transform(new Vector2(msState.X, msState.Y), Matrix.Invert(camera.GetViewMatrix(Vector2.One)));
            window.Title = "Local Mouse> X: " + msState.X + " Y: " + msState.Y + ", World Mouse> X: " + ms.X + " Y: " + ms.Y;

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
            DetectKeyPress(kb, Keys.Enter);
            DetectKeyPress(kb, Keys.Space);
            DetectKeyPress(kb, Keys.N);
            DetectKeyPress(kb, Keys.J);



            if (KeyDown)
            {
                if (kb.IsKeyUp(Key))
                {
                    switch (Key)
                    {
                        case Keys.Enter:

                            foreach (int i in factory.Objects.Keys)
                            {
                                if (factory.Objects[i].typeid == (int)GameObjectTypes.DOG)
                                    factory.Objects[i].sprite.PhysicsBody.ApplyLinearImpulse(new Vector2(60, 40));
                            }

                            break;
                        ////////////////////////////////////////////////////
                        case Keys.Space:
                            if (!EditMode)
                            {
                                cannonManager.ChangeCannonState(cannonGroups[0]);

                                if (cannonGroups[0].cannonState == CannonState.WAITING)
                                {
                                    client.SendMessage("action=shoot;cannonGroup=0;rotation=" + cannonGroups[0].Rotation + ";power=" + cannonGroups[0].Power);

                                }
                            }
                            break;

                        case Keys.N:
                            if (!EditMode)
                            {
                                cannonManager.ChangeCannonState(cannonGroups[1]);

                                if (cannonGroups[1].cannonState == CannonState.WAITING)
                                {
                                    client.SendMessage("action=shoot;cannonGroup=1;rotation=" + cannonGroups[1].Rotation + ";power=" + cannonGroups[1].Power);
                                }
                            }
                            break;

                        case Keys.OemTilde:
                            EditMode = !EditMode;
                            //camera.Zoom = 1f;
                            
                            window.Title = "Ragin Rovers " + (EditMode ? " | EDITING MODE" : "");
                            break;

                        case Keys.D1:

                            if (EditMode)
                            {
                                int dog = factory.Create((int)GameObjectTypes.DOG, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[dog].sprite.PhysicsBody.Mass = 30;
                                factory.Objects[dog].sprite.PhysicsBody.Restitution = 0.4f;
                            }

                            break;

                        case Keys.D2:

                            if (EditMode)
                            {
                                int cat = factory.Create((int)GameObjectTypes.CAT, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                                factory.Objects[cat].sprite.PhysicsBody.Mass = 30;
                                factory.Objects[cat].sprite.PhysicsBody.Restitution = 0.8f;
                            }

                            break;

                        case Keys.D3:

                            if (EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD1, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D4:

                            if (EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD2, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D5:

                            if (EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD3, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D6:

                            if (EditMode)
                            {
                                int board = factory.Create((int)GameObjectTypes.WOOD4, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D7:

                            if (EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_LEFT, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D8:

                            if (EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_MIDDLE, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;

                        case Keys.D9:

                            if (EditMode)
                            {
                                factory.Create((int)GameObjectTypes.PLATFORM_RIGHT, new Vector2((int)ms.X /*+ camera.Position.X*/ - 95, (int)ms.Y - 80), "spritesheet", new Vector2(0, 0), 0, 0f, 0f);
                            }

                            break;
                        case Keys.D0:

                            if (EditMode)
                            {
                                factory = cannonManager.CreateCannonStuff(factory, new Vector2(ms.X /*+ camera.Position.X*/ - 95, ms.Y - 80), camera, false, ref cannonGroups);

                            }

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

                            using (StreamReader infile = new StreamReader("map.txt"))
                            {
                                string objs = infile.ReadToEnd();
                                string[] lines = objs.Split('\n');

                                for (int i = 0; i < lines.Length; i++)
                                {
                                    if (lines[i].Length > 0)
                                    {
                                        string[] fields = lines[i].Split('\t');

                                        client.SendMessage("action=create;gotype=" + Convert.ToInt32(fields[1]) + ";textureassetname=" + fields[4] + ";location.x=" + (float)Convert.ToDouble(fields[2]) + ";location.y=" + (float)Convert.ToDouble(fields[3]) + ";rotation=" + (float)Convert.ToDouble(fields[5]) + ";upperBounds=" + 0f + ";lowerBounds=" + 0f);

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
    }
}
