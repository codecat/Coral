using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace CoralEngine
{
    public class ContentRegister
    {
        public static Engine Engine;

        private static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, SoundEffect> Sounds = new Dictionary<string, SoundEffect>();

        public static Texture2D Texture(string Filename)
        {
            if (Textures.ContainsKey(Filename))
                return Textures[Filename];
            else
            {
                if (File.Exists(Filename))
                {
                    Texture2D ret = Texture2D.FromStream(Engine.graphicsDevice, File.OpenRead(Filename));
                    Textures.Add(Filename, ret);
                    return ret;
                }
                else
                    return EngineContent.Textures.Pixel;
            }
        }

        public static SoundEffect Sound(string Filename)
        {
            if (Sounds.ContainsKey(Filename))
                return Sounds[Filename];
            else
            {
                if (File.Exists(Filename))
                {
                    SoundEffect ret = SoundEffect.FromStream(File.OpenRead(Filename));
                    Sounds.Add(Filename, ret);
                    return ret;
                }
                else
                    return EngineContent.Sounds.Default;
            }
        }
    }
}
