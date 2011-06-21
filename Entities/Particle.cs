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
    public class Particle : Entity
    {
        public Color Color = Color.White;
        public Texture2D Texture = null;

        public Vector2 Velocity = Vector2.Zero;

        public Color FadeColor = Color.Transparent;
        public int FadeStartTime;
        public int FadeTime;

        public int Rotation;
        public int RotationSpeed;
    }
}
