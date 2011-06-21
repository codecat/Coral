using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CoralEngine
{
    public class EngineContent
    {
        public class Textures
        {
            public static Texture2D Pixel;
            public static Texture2D Light;
            public static Texture2D NotificationBar;
        }

        public static class Sounds
        {
            public static SoundEffect Default;
        }

        public static class Fonts
        {
            public static SpriteFont Calibri12;
            public static SpriteFont Calibri12Bold;
        }

        public static void LoadContent(ContentManager content)
        {
            Textures.Pixel = content.Load<Texture2D>("Images/Pixel");
            Textures.Light = content.Load<Texture2D>("Images/Light");
            Textures.NotificationBar = content.Load<Texture2D>("Images/NotificationBar");

            Sounds.Default = content.Load<SoundEffect>("Sounds/Default");

            Fonts.Calibri12 = content.Load<SpriteFont>("Fonts/Calibri12");
            Fonts.Calibri12Bold = content.Load<SpriteFont>("Fonts/Calibri12Bold");
        }
    }
}
