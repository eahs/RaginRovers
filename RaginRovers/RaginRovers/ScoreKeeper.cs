using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RaginRoversLibrary;

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

        public void DrawNumber(SpriteBatch spriteBatch, TextureManager textureManager, Vector2 position, int number)
        {
            string num = number.ToString();

            for (int i = 0; i < num.Length; i++)
            {
                Texture2D numTexture = textureManager.Texture("scoresheet");
                Rectangle sourcerec = SpriteCreators.spriteSourceRectangles["score_" + num[i]];

                spriteBatch.Draw(numTexture, position + new Vector2(0, 10), sourcerec, Color.White);
                position.X += sourcerec.Width + 10;
            }
        }

        public void DrawScore(SpriteBatch spriteBatch, TextureManager textureManager, Vector2 position1, Vector2 position2)
        {
            spriteBatch.Draw(textureManager.Texture("scoresheet"), position1, SpriteCreators.spriteSourceRectangles["score_SCORE"], Color.White);
            DrawNumber(spriteBatch, textureManager, position1 + new Vector2((float)SpriteCreators.spriteSourceRectangles["score_SCORE"].Width + 30f, 0), playerLeftScore);

            spriteBatch.Draw(textureManager.Texture("scoresheet"), position2, SpriteCreators.spriteSourceRectangles["score_SCORE"], Color.White);
            DrawNumber(spriteBatch, textureManager, position2 + new Vector2((float)SpriteCreators.spriteSourceRectangles["score_SCORE"].Width + 30f, 0), playerRightScore);

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
