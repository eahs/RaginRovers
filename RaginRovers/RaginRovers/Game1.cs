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
using RaginRoversLibrary;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;
using System.IO;
using RaginRovers;

namespace RaginRovers
{


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Graphics objects
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;

        // Managers
        GameObjectFactory factory;
        TextureManager textureManager;
        ClientNetworking client;

        // 
        MapEditor mapEditor;
        CannonManager cannonManager;
        List<CannonGroups> cannonGroups;
        CloudManager cloudManager;

        public static int ScreenConfiguration = 1;

        public Game1()
        {
            //with .5 zoom, 830 more on each side, 1660 more total, 
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 727;

            //graphics.PreferredBackBufferHeight = 500;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GameWorld.Initialize(0, 12234 - ( 1080 * 2), this.Window.ClientBounds.Width, this.Window.ClientBounds.Height, new Vector2(0, 9.8f));
            GameWorld.ViewPortXOffset = 0;

            SpriteCreators.Load("Content\\spritesheet.txt");

            // Create the texture manager
            textureManager = new TextureManager(Content);

            //Create the audio manager
            AudioManager.Instance.Initialize(Content);

            // Now load the sprite creator factory helper
            factory = GameObjectFactory.Instance;
            factory.Initialize(textureManager);

            cannonManager = new CannonManager();
            cannonGroups = new List<CannonGroups>();


            client = new ClientNetworking();
            client.Connect();
            client.ActionHandler["shoot"] = new EventHandler(this.HandleNetworkShoot);
            client.ActionHandler["create"] = new EventHandler(this.HandleNetworkCreate);
            client.ActionHandler["plane"] = new EventHandler(this.HandleNetworkPlane);
            client.ActionHandler["createother"] = new EventHandler(this.HandleNetworkCreateOtherStuff);

            // Add a few sprite creators
            factory.AddCreator((int)GameObjectTypes.CAT, SpriteCreators.CreateCat);
            factory.AddCreator((int)GameObjectTypes.DOG, SpriteCreators.CreateDog);
            factory.AddCreator((int)GameObjectTypes.SUN, SpriteCreators.CreateSun);
            factory.AddCreator((int)GameObjectTypes.BOOM, SpriteCreators.CreateBoom);
            factory.AddCreator((int)GameObjectTypes.PUFF, SpriteCreators.CreatePuff);
            factory.AddCreator((int)GameObjectTypes.DINO, SpriteCreators.CreateDino);
            factory.AddCreator((int)GameObjectTypes.CLOUD1, SpriteCreators.CreateCloud1);
            factory.AddCreator((int)GameObjectTypes.CLOUD2, SpriteCreators.CreateCloud2);
            factory.AddCreator((int)GameObjectTypes.CLOUD3, SpriteCreators.CreateCloud3);
            factory.AddCreator((int)GameObjectTypes.CLOUD4, SpriteCreators.CreateCloud4);
            factory.AddCreator((int)GameObjectTypes.CLOUD5, SpriteCreators.CreateCloud5);
            factory.AddCreator((int)GameObjectTypes.CLOUD6, SpriteCreators.CreateCloud6);
            factory.AddCreator((int)GameObjectTypes.WOOD1, SpriteCreators.CreateWood1);
            factory.AddCreator((int)GameObjectTypes.WOOD2, SpriteCreators.CreateWood2);
            factory.AddCreator((int)GameObjectTypes.WOOD3, SpriteCreators.CreateWood3);
            factory.AddCreator((int)GameObjectTypes.WOOD4, SpriteCreators.CreateWood4);
            factory.AddCreator((int)GameObjectTypes.PLATFORM_LEFT, SpriteCreators.CreatePlatformLeft);
            factory.AddCreator((int)GameObjectTypes.PLATFORM_MIDDLE, SpriteCreators.CreatePlatformMiddle);
            factory.AddCreator((int)GameObjectTypes.PLATFORM_RIGHT, SpriteCreators.CreatePlatformRight);
            factory.AddCreator((int)GameObjectTypes.CANNON, SpriteCreators.CreateCannon);
            factory.AddCreator((int)GameObjectTypes.CANNONWHEEL, SpriteCreators.CreateCannonWheel);
            factory.AddCreator((int)GameObjectTypes.POWERMETERBAR, SpriteCreators.CreatePowerMeterBar);
            factory.AddCreator((int)GameObjectTypes.POWERMETERTAB, SpriteCreators.CreatePowerMeterTab);
            factory.AddCreator((int)GameObjectTypes.EXPLOSION1, SpriteCreators.CreateExplosion1);
            factory.AddCreator((int)GameObjectTypes.EXPLOSION1, SpriteCreators.CreateExplosion2);
            factory.AddCreator((int)GameObjectTypes.PLANE, SpriteCreators.CreatePlane);

            mapEditor = new MapEditor(Window, client, cannonManager, cannonGroups);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            camera = new Camera(new Viewport(GameWorld.ViewPortXOffset, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height));
            camera.Origin = new Vector2(camera.ViewPort.Width / 2.0f, camera.ViewPort.Height);

            camera.Zoom = .47f;

            camera.Position = new Vector2((12234/3) * (ScreenConfiguration-1), camera.Position.Y);



            // Load all the textures we're going to need for this game
            textureManager.LoadTexture("background");
            textureManager.LoadTexture("spritesheet");
            textureManager.LoadTexture("boom");
            textureManager.LoadTexture("cursor");
            textureManager.LoadTexture("sun");
            textureManager.LoadTexture("explosion1");
            textureManager.LoadTexture("plane_with_banner");
            textureManager.LoadTexture("clouds");
            AudioManager.Instance.LoadSoundEffect("airplane");
            AudioManager.Instance.LoadSoundEffect("cat1");
            AudioManager.Instance.LoadSoundEffect("cat2");
            AudioManager.Instance.LoadSoundEffect("cat3");
            AudioManager.Instance.LoadSoundEffect("cat4");
            AudioManager.Instance.LoadSoundEffect("dog1");
            AudioManager.Instance.LoadSoundEffect("meat_hit");
            AudioManager.Instance.LoadSoundEffect("wood_hitting");

            /*
            int cat = factory.Create((int)GameObjectTypes.CAT, Vector2.Zero, "spritesheet", Vector2.Zero, 0);
            int dog = factory.Create((int)GameObjectTypes.DOG, new Vector2(500, 50), "spritesheet", new Vector2(30, 0), 0);
            int boom2 = factory.Create((int)GameObjectTypes.BOOM, new Vector2(500, 50), "boom", new Vector2(30, 0), 0);
            

            factory.Objects[dog].sprite.PhysicsBody.Mass = 30;
            factory.Objects[dog].sprite.PhysicsBody.Restitution = 0.4f;

            factory.Objects[cat].sprite.PhysicsBody.Restitution = 0.8f;
            */



            

            Body body = BodyFactory.CreateBody(GameWorld.world);
            body.BodyType = BodyType.Static;
            body.Position = ConvertUnits.ToSimUnits(new Vector2(0, GameWorld.HeightofGround)); //hardcode workaround, can't figure out what to multiply the proportion by
            // * GameWorld.ProportionGroundtoScreen));
            body.UserData = new GameObject(-1, (int)GameObjectTypes.GROUND);

            //FixtureFactory.AttachRectangle((float)GameWorld.WorldWidth, 10, 1, new Vector2(0, ConvertUnits.ToDisplayUnits(this.Window.ClientBounds.Height-30)), body);
            Fixture ground = FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(GameWorld.WorldWidth)*10, ConvertUnits.ToSimUnits(10), 1, Vector2.Zero, body, "ground");
            ground.Restitution = 0f;
            
            SetupLevel();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (!mapEditor.EditMode)
                GameWorld.Update(gameTime);

            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Right))
            {
                if (camera.Position.X < GameWorld.WorldWidth - this.Window.ClientBounds.Width)
                {
                    camera.Position = new Vector2(camera.Position.X + 15, camera.Position.Y);
                }
                camera.OriginalPosition = new Vector2(camera.Position.X, camera.OriginalPosition.Y);                 
            }
            if (kb.IsKeyDown(Keys.Left))
            {
                camera.Position = new Vector2(camera.Position.X - 15, camera.Position.Y);
                if (camera.Position.X < 0)
                    camera.Position = Vector2.Zero;

                camera.OriginalPosition = new Vector2(camera.Position.X, camera.OriginalPosition.Y);
            }
            if (kb.IsKeyDown(Keys.Z))
            {
                if (camera.Zoom < 1)
                    camera.Zoom += 0.005f;
            }

            if (kb.IsKeyDown(Keys.X))
            {
                if (camera.Zoom > 0.3)
                    camera.Zoom -= 0.005f;
            }
            if (kb.IsKeyDown(Keys.P))
                camera.Zoom = .5f;

            foreach (int key in factory.Objects.Keys)
            {
                factory.Objects[key].sprite.Update(gameTime);
            }

            SunManager.Instance.Update(gameTime);
            SpriteHelper.Instance.Update(gameTime);
            PlaneManager.Instance.Update(gameTime);
            cloudManager.Update(gameTime);

            mapEditor.Update(gameTime, camera);
            client.Update(gameTime);
            camera.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            client.Disconnect();

            base.OnExiting(sender, args);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(104, 179, 255, 255));

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetViewMatrix(Vector2.One));

            for (int x = -6; x < (GameWorld.WorldWidth / this.Window.ClientBounds.Width)+6; x++)
            {
                spriteBatch.Draw(textureManager.Texture("background"), new Rectangle(x * this.Window.ClientBounds.Width, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), new Rectangle(0, 0, textureManager.Texture("background").Width, textureManager.Texture("background").Height), Color.White);
            }

            foreach (int key in factory.SortedObjectsList)
            {
                factory.Objects[key].sprite.Draw(spriteBatch);
            }

            spriteBatch.End();

            
            if (mapEditor.EditMode)
            {
                spriteBatch.Begin();
                MouseState ms = Mouse.GetState();
                spriteBatch.Draw(textureManager.Texture("cursor"), new Vector2(ms.X, ms.Y), Color.White);
                spriteBatch.End();
            }
            

            base.Draw(gameTime);
        }


        public void SetupLevel()
        {
            //create cannons
            cannonManager.CreateCannonStuff(factory, new Vector2(GameWorld.WorldWidth-250, GameWorld.HeightofGround), camera, true, ref cannonGroups); //need how to figure out location
            cannonManager.CreateCannonStuff(factory, new Vector2(0, GameWorld.HeightofGround), camera, false, ref cannonGroups); //need how to figure out location

            // Create some dinos
            int dino = factory.Create((int)GameObjectTypes.DINO, new Vector2(1000, this.Window.ClientBounds.Height-100), "spritesheet", Vector2.Zero, 0f, 0f, 0f);
            factory.Objects[dino].saveable = false;

            dino = factory.Create((int)GameObjectTypes.DINO, new Vector2(GameWorld.WorldWidth-1000, this.Window.ClientBounds.Height - 100), "spritesheet", Vector2.Zero, 0f, 0f, 0f);
            factory.Objects[dino].saveable = false;

            // Sun
            SunManager.Instance.Mood = SunMood.GRIN;

            // Clouds
            cloudManager = new CloudManager();

        }


        /* Callbacks */

        public void HandleNetworkShoot(object incoming, EventArgs args)
        {
            Dictionary<string, string> data = (Dictionary<string, string>)incoming;

            int cannonGroup = Convert.ToInt32(data["cannonGroup"]);
            double cannonRotation = Convert.ToDouble(data["rotation"]);
            double cannonPower = Convert.ToDouble(data["power"]);

            cannonGroups[cannonGroup].cannonState = CannonState.WAITING;

            cannonGroups[cannonGroup].Power = (float)cannonPower;
            cannonGroups[cannonGroup].Rotation = (float)cannonRotation;
            cannonManager.ChangeCannonState(cannonGroups[cannonGroup]);
        }

        public void HandleNetworkCreate(object incoming, EventArgs args)
        {
            Dictionary<string, string> data = (Dictionary<string, string>)incoming;
            int XOffsetduetoScreenSizeChange = 0;
            if (graphics.PreferredBackBufferWidth == 1920)
                XOffsetduetoScreenSizeChange = 700;
            int item = factory.Create(Convert.ToInt32(data["gotype"]),
                            new Vector2((float)Convert.ToDouble(data["location.x"]) + XOffsetduetoScreenSizeChange, (float)Convert.ToDouble(data["location.y"])),
                            data["textureassetname"],
                            Vector2.Zero,
                            (float)Convert.ToDouble(data["rotation"]),
                            (float)Convert.ToDouble(data["upperBounds"]),
                            (float)Convert.ToDouble(data["lowerBounds"]));

            if (factory.Objects[item].typeid == (int)GameObjectTypes.CAT)
            {
                factory.Objects[item].sprite.PhysicsBody.Friction = 15f;
                factory.Objects[item].sprite.PhysicsBody.Restitution = 0f;
                factory.Objects[item].sprite.PhysicsBody.Mass = 100f;
                factory.Objects[item].sprite.PhysicsBody.AngularDamping = 1f;
                factory.Objects[item].sprite.PhysicsBody.LinearDamping = 1f;
                factory.Objects[item].sprite.OnCollision += new OnCollisionEventHandler(CollisionEvents.cat_OnCollision);
            }
            if (factory.Objects[item].typeid == (int)GameObjectTypes.DOG)
            {
                factory.Objects[item].sprite.OnCollision += new OnCollisionEventHandler(CollisionEvents.dog_OnCollision);
            }
            if (factory.Objects[item].typeid == (int)GameObjectTypes.WOOD1 || factory.Objects[item].typeid == (int)GameObjectTypes.WOOD2 || factory.Objects[item].typeid == (int)GameObjectTypes.WOOD3 || factory.Objects[item].typeid == (int)GameObjectTypes.WOOD4)
            {
                factory.Objects[item].sprite.PhysicsBody.Friction = 15f;
                factory.Objects[item].sprite.PhysicsBody.Restitution = 0f;
                factory.Objects[item].sprite.PhysicsBody.Mass = 100f;
                factory.Objects[item].sprite.PhysicsBody.AngularDamping = 1f;
                factory.Objects[item].sprite.PhysicsBody.LinearDamping = 1f;
                factory.Objects[item].sprite.OnCollision += new OnCollisionEventHandler(CollisionEvents.wood_OnCollision);
            }

        }

        public void HandleNetworkCreateOtherStuff(object incoming, EventArgs args)
        {
            Dictionary<string, string> data = (Dictionary<string, string>)incoming;

            int cloud = factory.Create(Convert.ToInt32(data["gotype"]),
                            new Vector2((float)Convert.ToDouble(data["location.x"]), (float)Convert.ToDouble(data["location.y"])),
                            data["textureassetname"],
                            new Vector2((float)Convert.ToDouble(data["velocity.x"]), (float)Convert.ToDouble(data["velocity.y"])),
                            (float)Convert.ToDouble(data["rotation"]),
                            (float)Convert.ToDouble(data["upperBounds"]),
                            (float)Convert.ToDouble(data["lowerBounds"]));

            cloudManager.AddCloud(cloud);
        }

        public void HandleNetworkPlane(object incoming, EventArgs args)
        {
            PlaneManager.Instance.CreatePlane();
        }
    }
}
