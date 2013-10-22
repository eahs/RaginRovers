using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaginRovers;

namespace RaginRovers
{
    public class CannonGroups
    {
        public int cannonKey;
        public int wheelKey;
        public int barKey;
        public int tabKey;
        public int boomKey;
        public float boomTime;
        public bool isFlipped;

        public RaginRovers.Game1.CannonState cannonState = new RaginRovers.Game1.CannonState();
        

        public CannonGroups(int cannonKey,
                            int wheelKey,
                            int barKey,
                            int tabKey,
                            int boomKey,
                            bool isFlipped)
        {
            this.cannonKey = cannonKey;
            this.wheelKey = wheelKey;
            this.barKey = barKey;
            this.tabKey = tabKey;
            this.boomKey = boomKey;
            this.isFlipped = isFlipped;
            cannonState = Game1.CannonState.ROTATE;
        }
    }
}
