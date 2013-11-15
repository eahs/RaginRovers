using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaginRovers
{
    class ScoreKeeper
    {

        private static ScoreKeeper instance;
        private int playerLeftScore = 0;
        private int playerRightScore = 0;

        //scorey scoring
        /*public const int HittingWood = 100;
        public const int HittingCat = 300;
        public const int HittingPlane = 200;
        public const int Missing = 50;*/
        //etc.

        //football scoring
        public const int HittingWood = 1;
        public const int HittingCat = 6;
        public const int HittingPlane = 2;
        public const int Missing = 0;


        public int PlayerLeftScore
        {
            get
            {
                return playerLeftScore;
            }
            set
            {
                playerLeftScore = value;
            }
        }
        public int PlayerRightScore
        {
            get
            {
                return playerRightScore;
            }
            set
            {
                playerRightScore = value;
            }
        }

        // Guarantee only one instance
        public static ScoreKeeper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScoreKeeper();
                }
                return instance;
            }
        }
    }
}
