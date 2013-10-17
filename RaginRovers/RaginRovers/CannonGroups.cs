using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaginRovers
{
    public class CannonGroups
    {
        public int cannonKey;
        public int wheelKey;
        public int barKey;
        public int tabKey;

        public CannonGroups(int cannonKey,
                            int wheelKey,
                            int barKey,
                            int tabKey)
        {
            this.cannonKey = cannonKey;
            this.wheelKey = wheelKey;
            this.barKey = barKey;
            this.tabKey = tabKey;
        }
    }
}
