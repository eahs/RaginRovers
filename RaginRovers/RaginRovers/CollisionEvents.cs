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

            if (otherObject.typeid == (int)GameObjectTypes.DOG)
            {
                AudioManager.Instance.SoundEffect("meat_hit").Play();
                AudioManager.Instance.SoundEffect("cat1").Play();

                cat.sprite.HitPoints -= (int)otherObject.sprite.PhysicsBody.LinearVelocity.Length(); 

            }
            if (otherObject.typeid == (int)GameObjectTypes.WOOD1 || otherObject.typeid == (int)GameObjectTypes.WOOD2 || otherObject.typeid == (int)GameObjectTypes.WOOD3 || otherObject.typeid == (int)GameObjectTypes.WOOD4)
            {
                //if (otherObject.sprite.PhysicsBody
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
                    SunManager.Instance.Mood = SunMood.MAD;

                //SpriteHelper.Instance.RemoveAfter(dog.id, 8000);
                if (dog.alive)
                {
                    dog.alive = false;

                    SpriteHelper.Instance.TriggerAfter(delegate()
                        {
                            SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.PUFF, dog.sprite.Center-new Vector2(128,128), "spritesheet", 15);
                            SpriteHelper.Instance.TriggerAfter(delegate()
                            {
                                GameObjectFactory.Instance.Remove(dog.id);
                            }, 100);
                        }, 6000);
                }
            }
            else if (otherObject.typeid == (int)GameObjectTypes.WOOD1 ||
                     otherObject.typeid == (int)GameObjectTypes.WOOD2 ||
                     otherObject.typeid == (int)GameObjectTypes.WOOD3 ||
                     otherObject.typeid == (int)GameObjectTypes.WOOD4
                    )
            {
                SunManager.Instance.Mood = SunMood.TOOTHYSMILE;

                if (dog.collisioncount < 2) 
                    SpriteHelper.Instance.TriggerAnimation(GameObjectTypes.EXPLOSION1, collidePoint - new Vector2(175, 197), "explosion1", 15);

            }
            else if (otherObject.typeid == (int)GameObjectTypes.PLANE)
            {
                PlaneManager.Instance.planeState = PlaneState.BOMB;
                //do some bombing
            }


            return true;
        }

        public static bool wood_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            GameObject wood = (GameObject)fixtureA.Body.UserData;
            GameObject otherObject = (GameObject)fixtureB.Body.UserData;

            if (otherObject.typeid == (int)GameObjectTypes.WOOD1 || otherObject.typeid == (int)GameObjectTypes.WOOD2 || otherObject.typeid == (int)GameObjectTypes.WOOD3 || otherObject.typeid == (int)GameObjectTypes.WOOD4)
            {
                //trying to play the wood clanking sound but can't figure out

                AudioManager.Instance.SoundEffect("wood_hitting").Play();
            }
            return true;
        }

    }
}
