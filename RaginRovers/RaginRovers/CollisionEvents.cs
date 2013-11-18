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
    public static class CollisionEvents
    {
        public static bool cat_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            GameObject cat = (GameObject)fixtureA.Body.UserData;
            GameObject otherObject = (GameObject)fixtureB.Body.UserData;

            cat.collisioncount++;

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
            }
            //might have to do negatives
            if (cat.sprite.PhysicsBody.LinearVelocity.X >= 5 || cat.sprite.PhysicsBody.LinearVelocity.Y >= 5)
            {
                cat.sprite.HitPoints = 0;
                AudioManager.Instance.SoundEffect("cat_aaagh").Play();
            }

            if (cat.sprite.HitPoints <= 0)
            {
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

                    Vector2 dogCenter = dog.sprite.Center;

                    SpriteHelper.Instance.TriggerAfter(delegate()
                        {
                            
                            SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.PUFF, dogCenter-new Vector2(128,128), "spritesheet", 15);
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
                    if (dog.sprite.PlayerNumber == 1)
                    {
                        ScoreKeeper.Instance.PlayerLeftScore += ScoreKeeper.HittingCat;
                        SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS250, collidePoint, "scoresheet");
                    }
                    if (dog.sprite.PlayerNumber == 2)
                    {
                        ScoreKeeper.Instance.PlayerRightScore += ScoreKeeper.HittingCat;
                    }
                }
            }
            else if (otherObject.typeid == (int)GameObjectTypes.DOG)
            {
                Vector2 otherCenter = otherObject.sprite.Center;
                SpriteHelper.Instance.TriggerAfter(delegate()
                {
                    SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.PUFF, otherCenter - new Vector2(128, 128), "spritesheet", 15);
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

                if (dog.collisioncount == 1)
                {

                    SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.DUSTSPLODE, otherObject.sprite.Center - new Vector2(180, 180), "dustsplode", 15);
                    SpriteHelper.Instance.TriggerAfter(delegate()
                    {
                        GameObjectFactory.Instance.Remove(otherObject.id);
                    }, 100);

                    AudioManager.Instance.SoundEffect("dog_impact").Play();

                    SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS50, collidePoint, "scoresheet");

                    if (dog.sprite.PlayerNumber == 1)
                    {
                        ScoreKeeper.Instance.PlayerLeftScore += ScoreKeeper.HittingWood;
                    }
                    if (dog.sprite.PlayerNumber == 2)
                    {
                        ScoreKeeper.Instance.PlayerRightScore += ScoreKeeper.HittingWood;
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
                PlaneManager.Instance.planeState = PlaneState.BOMB;
                //do some bombing
                if (dog.sprite.PlayerNumber == 1)
                {
                    ScoreKeeper.Instance.PlayerLeftScore += ScoreKeeper.HittingPlane;
                }
                if (dog.sprite.PlayerNumber == 2)
                {
                    ScoreKeeper.Instance.PlayerRightScore += ScoreKeeper.HittingPlane;
                }

                SpriteHelper.Instance.TriggerFadeUp(GameObjectTypes.SCOREPLUS100, collidePoint, "scoresheet");

                AudioManager.Instance.SoundEffect("dog_bark").Play(0.5f, 0, 0);
            }


            return true;
        }

        public static bool wood_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            GameObject wood = (GameObject)fixtureA.Body.UserData;
            GameObject otherObject = (GameObject)fixtureB.Body.UserData;

            if (otherObject.typeid == (int)GameObjectTypes.WOOD1 || otherObject.typeid == (int)GameObjectTypes.WOOD2 || otherObject.typeid == (int)GameObjectTypes.WOOD3 || otherObject.typeid == (int)GameObjectTypes.WOOD4)
            {

                AudioManager.Instance.SoundEffect("wood_hitting").Play(0.01f, 0f, 0f);
            }
            return true;
        }

    }
}
