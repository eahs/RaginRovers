using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using RaginRoversLibrary;
using Microsoft.Xna.Framework;

namespace RaginRovers
{
    public class CollisionEvents
    {
        private static CollisionEvents instance;
        public int NewestCollision = 0;
        private ClientNetworking client;

        public void TransferClientInfo(ClientNetworking client)
        {
            this.client = client;
        }


        public static bool cat_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            GameObject cat = (GameObject)fixtureA.Body.UserData;
            GameObject otherObject = (GameObject)fixtureB.Body.UserData;

            cat.collisioncount++;

            Vector2 normal;
            FixedArray2<Vector2> points;

            contact.GetWorldManifold(out normal, out points);
            Vector2 collidePoint = ConvertUnits.ToDisplayUnits(points[0]);

            if (otherObject.typeid == (int)GameObjectTypes.DOG)
            {

                //cat.sprite.HitPoints -= (int)otherObject.sprite.PhysicsBody.LinearVelocity.Length();
                cat.sprite.HitPoints = 0;
                AudioManager.Instance.SoundEffect("cat1").Play();
            }
            if (otherObject.typeid == (int)GameObjectTypes.WOOD1 || otherObject.typeid == (int)GameObjectTypes.WOOD2 || otherObject.typeid == (int)GameObjectTypes.WOOD3 || otherObject.typeid == (int)GameObjectTypes.WOOD4)
            {
                if (otherObject.sprite.PhysicsBody.LinearVelocity.Length() > 2)
                {
                    cat.sprite.HitPoints = 0;
                    AudioManager.Instance.SoundEffect("cat_moan").Play();

                    
                }
                //dont know what that is
                /*else
                {
                    if (otherObject.sprite.PlayerNumber == 1)
                    {
                        ScoreKeeper.Instance.PlayerLeftScore += ScoreKeeper.HittingWood;
                        SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS50, collidePoint, "scoresheet");
                    }
                    else if (otherObject.sprite.PlayerNumber == 2)
                    {
                        ScoreKeeper.Instance.PlayerRightScore += ScoreKeeper.HittingWood;
                        SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS50, collidePoint, "scoresheet");
                    }
                    otherObject.sprite.PlayerNumber = 0;
                }*/
            }
            //might have to do negatives
            if (cat.sprite.PhysicsBody.LinearVelocity.X >= 5 || cat.sprite.PhysicsBody.LinearVelocity.Y >= 5)
            {
                cat.sprite.HitPoints = 0;
                AudioManager.Instance.SoundEffect("cat_aaagh").Play();
            }

            if (cat.sprite.HitPoints <= 0)
            {
                //if cat dies, points awarded to person to last hit tower
                if (Game1.ScreenConfiguration == 2)
                {
                    if (Instance.NewestCollision == 1 && !cat.sprite.AlreadyGavePoints)
                    {
                        ScoreKeeper.Instance.PlayerLeftScore += ScoreKeeper.HittingCat;
                        SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS250, collidePoint, "scoresheet");
                        cat.sprite.AlreadyGavePoints = true;
                        Instance.client.SendMessage("action=sendpoints;playernumber=" + Instance.NewestCollision + ";score=" + 250);
                    }
                    if (Instance.NewestCollision == 2 && !cat.sprite.AlreadyGavePoints)
                    {
                        ScoreKeeper.Instance.PlayerRightScore += ScoreKeeper.HittingCat;
                        SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS250, collidePoint, "scoresheet");
                        cat.sprite.AlreadyGavePoints = true;
                        Instance.client.SendMessage("action=sendpoints;playernumber=" + Instance.NewestCollision + ";score=" + 250);
                    }
                }

                Vector2 catCenter = cat.sprite.Center;

                SpriteHelper.Instance.TriggerAfter(delegate()
                {
                    SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.CATSPLODE, catCenter - new Vector2(32, 32), "catsplode", 15);
                    SpriteHelper.Instance.TriggerAfter(delegate()
                    {
                        GameObjectFactory.Instance.Remove(cat.id);
                    }, 100);
                }, 100);

                //AudioManager.Instance.SoundEffect("dog_oof").Play();
            }

           return true;
        }

        public static bool dog_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            GameObject dog = (GameObject)fixtureA.Body.UserData;
            GameObject otherObject = (GameObject)fixtureB.Body.UserData;

            Vector2 normal;
            FixedArray2<Vector2> points;

            contact.GetWorldManifold(out normal, out points);
            Vector2 collidePoint = ConvertUnits.ToDisplayUnits(points[0]);

            dog.collisioncount++;

            if (otherObject.typeid == (int)GameObjectTypes.GROUND)
            {
                if (dog.collisioncount == 1)
                {
                    SunManager.Instance.Mood = SunMood.MAD;
                    AudioManager.Instance.SoundEffect("dog_oof").Play(0.5f, 0, 0);
                    if (dog.sprite.PlayerNumber == 1)
                    {
                        ScoreKeeper.Instance.PlayerLeftScore -= ScoreKeeper.Missing;
                    }
                    if (dog.sprite.PlayerNumber == 2)
                    {
                        ScoreKeeper.Instance.PlayerRightScore -= ScoreKeeper.Missing;
                    }

                    SpriteHelper.Instance.TriggerAfter(delegate()
                    {
                        AudioManager.Instance.SoundEffect("cat_soclose").Play(0.5f, 0, 0);
                    }, 1000);
                    
                }

                //SpriteHelper.Instance.RemoveAfter(dog.id, 8000);
                if (dog.alive)
                {
                    dog.alive = false;


                    SpriteHelper.Instance.TriggerAfter(delegate()
                        {
                            if (dog.sprite != null)
                            {
                                Vector2 dogCenter = dog.sprite.Center;
                                SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.PUFF, dogCenter - new Vector2(128, 128), "spritesheet", 15);
                            }

                            SpriteHelper.Instance.TriggerAfter(delegate()
                            {
                                GameObjectFactory.Instance.Remove(dog.id);
                            }, 100);
                        }, 6000);

                    return true;
                }
            }
            else if (otherObject.typeid == (int)GameObjectTypes.CAT)
            {
                if (dog.collisioncount == 1)
                {
                    Instance.NewestCollision = dog.sprite.PlayerNumber;

                    
                }
            }
            //something here giving 50 points
            else if (otherObject.typeid == (int)GameObjectTypes.DOG)
            {
                
                SpriteHelper.Instance.TriggerAfter(delegate()
                {
                    if (otherObject.sprite != null)
                    {
                        Vector2 otherCenter = otherObject.sprite.Center;
                        SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.PUFF, otherCenter - new Vector2(128, 128), "spritesheet", 15);
                    }
                    SpriteHelper.Instance.TriggerAfter(delegate()
                    {
                        GameObjectFactory.Instance.Remove(otherObject.id);
                    }, 100);
                }, 1000);

                if (dog.collisioncount == 1)
                {
                    AudioManager.Instance.SoundEffect("dog_impact").Play(0.5f, 0, 0);
                }

            }
            else if (otherObject.typeid == (int)GameObjectTypes.WOOD1 ||
                     otherObject.typeid == (int)GameObjectTypes.WOOD2 ||
                     otherObject.typeid == (int)GameObjectTypes.WOOD3 ||
                     otherObject.typeid == (int)GameObjectTypes.WOOD4
                    )
            {
                SunManager.Instance.Mood = SunMood.TOOTHYSMILE;
                
                //otherObject.collisiontime = System.Environment.TickCount;
                //otherObject.sprite.PlayerNumber = dog.sprite.PlayerNumber;


                if (dog.collisioncount == 1)
                {

                    Instance.NewestCollision = dog.sprite.PlayerNumber;
                    //Instance.SpreadVirus();

                    SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.DUSTSPLODE, otherObject.sprite.Center - new Vector2(180, 180), "dustsplode", 15);
                    SpriteHelper.Instance.TriggerAfter(delegate()
                    {
                        GameObjectFactory.Instance.Remove(otherObject.id);
                    }, 100);

                    AudioManager.Instance.SoundEffect("dog_impact").Play();

                    SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS50, collidePoint, "scoresheet");


                    if (Game1.ScreenConfiguration == 2)
                    {
                        if (Instance.NewestCollision == 1)
                        {
                            ScoreKeeper.Instance.PlayerLeftScore += ScoreKeeper.HittingWood;
                            Instance.client.SendMessage("action=sendpoints;playernumber=" + Instance.NewestCollision + ";score=" + 50);
                        }
                        if (Instance.NewestCollision == 2)
                        {
                            ScoreKeeper.Instance.PlayerRightScore += ScoreKeeper.HittingWood;
                            Instance.client.SendMessage("action=sendpoints;playernumber=" + Instance.NewestCollision + ";score=" + 50);
                        }
                        
                    }


                    return true;
                }

                if (dog.collisioncount < 2)
                {
                    SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.EXPLOSION1, collidePoint - new Vector2(175, 197), "explosion1", 15);
                    //AudioManager.Instance.SoundEffect("dog_impact").Play(0.5f, 0, 0);
                }
            }
            else if (otherObject.typeid == (int)GameObjectTypes.PLANE)
            {
                if (dog.collisioncount == 1)
                {
                    //PlaneManager.Instance.planeState = PlaneState.BOMB;
                    PlaneManager.Instance.Bomb(dog.sprite.PlayerNumber);
                    //do some bombing
                    if (Game1.ScreenConfiguration == 2)
                    {
                        if (dog.sprite.PlayerNumber == 1)
                        {
                            ScoreKeeper.Instance.PlayerLeftScore += ScoreKeeper.HittingPlane;
                            Instance.client.SendMessage("action=sendpoints;playernumber=" + Instance.NewestCollision + ";score=" + 100);
                        }
                        if (dog.sprite.PlayerNumber == 2)
                        {
                            ScoreKeeper.Instance.PlayerRightScore += ScoreKeeper.HittingPlane;
                            Instance.client.SendMessage("action=sendpoints;playernumber=" + Instance.NewestCollision + ";score=" + 100);
                        }
                    }

                    SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS100, collidePoint, "scoresheet");

                    AudioManager.Instance.SoundEffect("dog_bark").Play(0.5f, 0, 0);
                }
            }


            return true;
        }

        public static bool wood_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            GameObject wood = (GameObject)fixtureA.Body.UserData;
            GameObject otherObject = (GameObject)fixtureB.Body.UserData;

            if (otherObject.typeid == (int)GameObjectTypes.WOOD1 || otherObject.typeid == (int)GameObjectTypes.WOOD2 || otherObject.typeid == (int)GameObjectTypes.WOOD3 || otherObject.typeid == (int)GameObjectTypes.WOOD4)
            {

                //otherObject.sprite.PlayerNumber = wood.sprite.PlayerNumber;
                //otherObject.collisiontime = System.Environment.TickCount;
                //Instance.SpreadVirus();


                // AudioManager.Instance.SoundEffect("wood_hitting").Play(0.01f, 0f, 0f);
            }
            return true;
        }

        #region Failed Spread Code
        //WHAT A MESS, but it thoroughly spreads virus through all touching blocks
        /*public void SpreadVirus()
        {
            //goes through the list until everything that can be infected, is
            int count = 0;
            List<Contact> NeitherInfected = new List<Contact>();
            List<Contact> AlreadyCalculatedObjects = new List<Contact>();
            for (;;)
                {
                    foreach (Contact contacts in GameWorld.world.ContactList)
                    {
                        //trying to save some processing? but maybe it won't
                        bool AlreadyDid = false;
                        foreach (Contact contacts2 in AlreadyCalculatedObjects)
                        {
                            if (contacts == contacts2)
                            {
                                AlreadyDid = true;
                                break;
                            }
                        }
                        if (!AlreadyDid)
                        {
                            GameObject Object1 = (GameObject)contacts.FixtureA.Body.UserData;
                            GameObject Object2 = (GameObject)contacts.FixtureB.Body.UserData;
                            //1st null, second exists
                            if (!Object1.sprite.PlayerNumber.Equals(Object1.sprite.PlayerNumber) && Object2.sprite.PlayerNumber.Equals(Object2.sprite.PlayerNumber))
                            {
                                Object1.sprite.PlayerNumber = Object2.sprite.PlayerNumber;
                                AlreadyCalculatedObjects.Add(contacts);
                            }
                            //2st exists, second doesn't
                            else if (Object1.sprite.PlayerNumber.Equals(Object1.sprite.PlayerNumber) && !Object2.sprite.PlayerNumber.Equals(Object2.sprite.PlayerNumber))
                            {
                                Object1.sprite.PlayerNumber = Object2.sprite.PlayerNumber;
                                AlreadyCalculatedObjects.Add(contacts);
                            }

                            //garbage code below, I can't think of what to do

                            if (Instance.NewestCollision == 1)
                            {
                                Object1.sprite.PlayerNumber = 1;
                                Object2.sprite.PlayerNumber = 1;
                                AlreadyCalculatedObjects.Add(contacts);
                            }
                            else if (Instance.NewestCollision == 2)
                            {
                                Object1.sprite.PlayerNumber = 2;
                                Object2.sprite.PlayerNumber = 2;
                                AlreadyCalculatedObjects.Add(contacts);
                            }
                            else
                            {
                                //don't think will work
                                bool GoodtoGo = true;
                                foreach (Contact notinfected in NeitherInfected)
                                {
                                    if (notinfected == contacts)
                                    {
                                        GoodtoGo = false;
                                        break;
                                    }
                                }
                                if (GoodtoGo)
                                {
                                    NeitherInfected.Add(contacts);
                                    count++;
                                }
                            }
                            //end garbage
                        }
                    }
                    if (count == 0)
                    {
                        break;
                    }
                }
        }


        //simple spread
        public void SpreadVirus()
        {
            foreach (Contact contact in GameWorld.world.ContactList)
            {
                GameObject Object1 = (GameObject)contact.FixtureA.Body.UserData;
                GameObject Object2 = (GameObject)contact.FixtureB.Body.UserData;
                //won't work
                //why can you access playernumber without it being null in the above stuff but i can't down here :(
                //Object2.sprite.PlayerNumber = Object1.sprite.PlayerNumber;
            }
        }*/

        #endregion

        public static CollisionEvents Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CollisionEvents();
                }
                return instance;
            }
        }


    }
}
