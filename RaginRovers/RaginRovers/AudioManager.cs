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

namespace RaginRoversLibrary
{
    public class AudioManager
    {

        public static AudioManager instance;
        Dictionary<string, SoundEffect> soundEffects;
        ContentManager Content;

        private AudioManager()
        {
            soundEffects = new Dictionary<string, SoundEffect>();
        }
            
        public void Initialize (ContentManager Content)
        {
            this.Content = Content;
        }

        public void LoadSoundEffect(string assetname)
        {
            this.LoadSoundEffect(assetname, assetname);
        }

        public void LoadSoundEffect(string assetname, string assetkey)
        {
            if (!soundEffects.ContainsKey(assetname))
            {
                soundEffects.Add(assetkey, Content.Load<SoundEffect>("Audio/" + assetname));
            }
        }

        public SoundEffect SoundEffect(string assetkey)
        {
            return soundEffects.ContainsKey(assetkey) ? soundEffects[assetkey] : null;
        }

        public SoundEffectInstance GetSoundEffectLooped(string assetkey)
        {
            SoundEffect fx = SoundEffect(assetkey);

            if (fx != null)
            {
                SoundEffectInstance instance = fx.CreateInstance();
                instance.IsLooped = true;
                
                return instance;
            }

            return null;
        }

        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioManager();
                }
                return instance;
            }
        }
    }
}
