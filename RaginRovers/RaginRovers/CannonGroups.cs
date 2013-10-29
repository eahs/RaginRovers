using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaginRovers;
using RaginRoversLibrary;
using Microsoft.Xna.Framework;

namespace RaginRovers
{
    public enum CannonState
    {
        ROTATE,
        POWER,
        WAITING, /* Fire confirmation has to come from the server */
        SHOOT,
        COOLDOWN
    }

    public class CannonGroups
    {
        public int cannonKey;
        public int wheelKey;
        public int barKey;
        public int tabKey;
        public int boomKey;
        public float boomTime;
        public bool isFlipped;

        public CannonState cannonState;
        private GameObjectFactory factory;

        public CannonGroups(int cannonKey,
                            int wheelKey,
                            int barKey,
                            int tabKey,
                            int boomKey,
                            bool isFlipped)
        {
            this.factory = GameObjectFactory.Instance;
            this.cannonKey = cannonKey;
            this.wheelKey = wheelKey;
            this.barKey = barKey;
            this.tabKey = tabKey;
            this.boomKey = boomKey;
            this.isFlipped = isFlipped;
            cannonState = CannonState.ROTATE;
        }

        public float Rotation
        {
            get {
                return factory.Objects[cannonKey].sprite.Rotation;
            }
            set {
                factory.Objects[cannonKey].sprite.Rotation = value;
            }
        }

        public float Power
        {
            get
            {
                return ((factory.Objects[tabKey].sprite.Location.X - factory.Objects[barKey].sprite.Location.X) / factory.Objects[barKey].sprite.BoundingBoxRect.Width) + 1;
            }
            set
            {
                factory.Objects[tabKey].sprite.Location = new Vector2(
                    factory.Objects[barKey].sprite.Location.X +
                    (value-1) * factory.Objects[barKey].sprite.BoundingBoxRect.Width,
                    factory.Objects[tabKey].sprite.Location.Y
                    );
            }
        }
    }
}
