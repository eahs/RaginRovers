using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace RaginRoversLibrary
{
    class CannonManager
    {

        public float elapsedTime = 0;

        public int Direction = 1;


        public CannonManager()
        {
        }

        public void ChangeCannonState(CannonGroups cannonGroups)
        {
            if (cannonGroups.cannonState == RaginRovers.Game1.CannonState.POWER)
            {
                cannonGroups.cannonState = RaginRovers.Game1.CannonState.SHOOT;
            }
            if (cannonGroups.cannonState == RaginRovers.Game1.CannonState.ROTATE)
                cannonGroups.cannonState = RaginRovers.Game1.CannonState.POWER;
        }

        public GameObjectFactory ManipulateCannons(GameObjectFactory factory, CannonGroups cannonGroups)
        {
            if (cannonGroups.cannonState == RaginRovers.Game1.CannonState.ROTATE)
                {
                            //decide whether to rotate one way or back
                    if (factory.Objects[cannonGroups.cannonKey].sprite.Rotation >= factory.Objects[cannonGroups.cannonKey].sprite.LowerRotationBounds)
                        factory.Objects[cannonGroups.cannonKey].sprite.rotationDirection = -1;
                    if (factory.Objects[cannonGroups.cannonKey].sprite.Rotation <= factory.Objects[cannonGroups.cannonKey].sprite.UpperRotationBounds)
                        factory.Objects[cannonGroups.cannonKey].sprite.rotationDirection = 1;



                    factory.Objects[cannonGroups.cannonKey].sprite.Rotation += ((MathHelper.PiOver4 / 16) * factory.Objects[cannonGroups.cannonKey].sprite.rotationDirection);
                        

                }
            if (cannonGroups.cannonState == RaginRovers.Game1.CannonState.POWER)
                {

                    if (factory.Objects[cannonGroups.barKey].sprite.Location.X > factory.Objects[cannonGroups.tabKey].sprite.Location.X)
                    {
                        Direction = 1;
                    }
                    else if (factory.Objects[cannonGroups.barKey].sprite.Location.X + factory.Objects[cannonGroups.barKey].sprite.BoundingBoxRect.Width - factory.Objects[cannonGroups.tabKey].sprite.BoundingBoxRect.Width < factory.Objects[cannonGroups.tabKey].sprite.Location.X)
                    {
                        Direction = -1;
                    }
                    factory.Objects[cannonGroups.tabKey].sprite.Location += new Vector2(10 * Direction, 0);
                        
                }
            if (cannonGroups.cannonState == RaginRovers.Game1.CannonState.SHOOT)
                {
                    ShootDoggy(factory, cannonGroups);
                    cannonGroups.cannonState = RaginRovers.Game1.CannonState.ROTATE;
                }
            return factory;
        }

        public GameObjectFactory ShootDoggy(GameObjectFactory factory, CannonGroups cannonGroup)
        {
            List<int> temp = new List<int>();
            //figure out which cannon to shoot from
            //replace once just pass which cannon
            //endreplace

                //dog
                int dog = factory.Create(
                    (int)RaginRovers.GameObjectTypes.DOG,
                    factory.Objects[cannonGroup.cannonKey].sprite.Location,
                    "spritesheet",
                    Vector2.Zero,
                    factory.Objects[cannonGroup.cannonKey].sprite.Rotation,
                    0,
                    0);


                factory.Objects[dog].sprite.PhysicsBody.LinearVelocity = new Vector2(
                        10 * (float)Math.Cos((double)factory.Objects[cannonGroup.cannonKey].sprite.Rotation),
                        10 * (float)Math.Sin((double)factory.Objects[cannonGroup.cannonKey].sprite.Rotation));

                //chaning magnitude depending on power bar
                factory.Objects[dog].sprite.PhysicsBody.LinearVelocity *= ((factory.Objects[cannonGroup.tabKey].sprite.Location.X - factory.Objects[cannonGroup.barKey].sprite.Location.X) / factory.Objects[cannonGroup.barKey].sprite.BoundingBoxRect.Width) + 1;
                                  


                factory.Objects[dog].sprite.PhysicsBody.Mass = 30;
                factory.Objects[dog].sprite.PhysicsBody.Restitution = 0.4f;

                //boom
                int boom = factory.Create(
                    (int)RaginRovers.GameObjectTypes.BOOM,
                    factory.Objects[cannonGroup.cannonKey].sprite.Location,
                    "boom",
                    Vector2.Zero,
                    factory.Objects[cannonGroup.cannonKey].sprite.Rotation,
                    0,
                    0);
                //changing location so that origins equal
                factory.Objects[boom].sprite.Location += factory.Objects[cannonGroup.cannonKey].sprite.Origin - factory.Objects[boom].sprite.Origin;

                factory.Objects[boom].sprite.Scale = 0.5f;

            return factory;
        }

        public GameObjectFactory CreateCannonStuff(GameObjectFactory factory, Vector2 location, Camera camera, bool isReversed, ref List<CannonGroups> cannonGroups)
        {
            int icannon;
            if (!isReversed)
            {
                icannon = factory.Create(
                                        (int)RaginRovers.GameObjectTypes.CANNON,
                                        new Vector2((int)location.X  - 95, (int)location.Y - 80),
                                        "spritesheet",
                                        new Vector2(0, 0),
                                        0,
                                        -MathHelper.PiOver2,
                                        0);
            }
            else
            {
                icannon = factory.Create((int)RaginRovers.GameObjectTypes.CANNON, new Vector2(location.X  - 95, location.Y - 80), "spritesheet", new Vector2(0, 0), -MathHelper.Pi, -MathHelper.Pi, -MathHelper.PiOver2);
            }
            int iwheel = factory.Create(
                (int)RaginRovers.GameObjectTypes.CANNONWHEEL,
                new Vector2((int)location.X  - 30, (int)location.Y - 120),
                "spritesheet",
                new Vector2(0, 0),
                0,
                0f,
                0f);
            int ibar = factory.Create(
                 (int)RaginRovers.GameObjectTypes.POWERMETERBAR,
                 new Vector2(
                     factory.Objects[icannon].sprite.Location.X,
                     factory.Objects[icannon].sprite.Location.Y + factory.Objects[icannon].sprite.BoundingBoxRect.Height + 50),
                 "background",
                 new Vector2(0, 0),
                 0,
                 0f,
                 0f);
            int itab = factory.Create(
                 (int)RaginRovers.GameObjectTypes.POWERMETERTAB,
                 new Vector2(
                     factory.Objects[ibar].sprite.Location.X,
                     factory.Objects[ibar].sprite.Location.Y + factory.Objects[ibar].sprite.Origin.Y),
                 "background",
                 new Vector2(0, 0),
                 0,
                 0f,
                 0f);
            //had to put after because cant access origin before sprite is created
            factory.Objects[itab].sprite.Location -= new Vector2(0, factory.Objects[itab].sprite.Origin.Y);

            cannonGroups.Add(new CannonGroups(icannon, iwheel, ibar, itab));

            //putting them all in a group
            //factory.Objects[icannon].sprite.groupNumber = groupNumber;
            //factory.Objects[iwheel].sprite.groupNumber = groupNumber;
            //factory.Objects[ibar].sprite.groupNumber = groupNumber;
            //factory.Objects[itab].sprite.groupNumber = groupNumber;

            //Sprite cannon = factory.Objects[icannon].sprite;
            //Sprite wheel = factory.Objects[iwheel].sprite;


            factory.Objects[icannon].sprite.Origin = new Vector2(120, 103);

            factory.Objects[iwheel].sprite.Location = factory.Objects[icannon].sprite.Location + factory.Objects[icannon].sprite.Origin - factory.Objects[iwheel].sprite.Origin;
            //groupNumber++;
            return factory;
        }

        public void Update(GameTime gameTime)
        {
            //was going to use this to limit boom duration but idk
        }
        /*
        public float ConvertToRealRadians(float Rotation)
        {
            for (; ; )
            {
                if (Rotation < 0)
                {
                    Rotation += MathHelper.TwoPi;
                }
                if (Rotation > MathHelper.TwoPi)
                {
                    Rotation -= MathHelper.TwoPi;
                }
                if (Rotation >= 0 && Rotation <= MathHelper.TwoPi) 
                    break;
            }
            return Rotation;
        }
         */
    }

}
