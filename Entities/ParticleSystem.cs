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
    public class ParticleSystem : Entity
    {
        public Particle ParticleTemplate = new Particle();

        public string Texture;

        public Vector2 InitialVelocity;
        public Vector2 InitialRandom;

        public int Time;
        public int Amount;

        public bool StartEnabled;
        public bool Enabled;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(new byte[] { ParticleTemplate.Color.R, ParticleTemplate.Color.G, ParticleTemplate.Color.B, ParticleTemplate.Color.A });
            writer.Write(Texture);
            
            writer.Write(ParticleTemplate.Velocity.X);
            writer.Write(ParticleTemplate.Velocity.Y);

            writer.Write(new byte[] { ParticleTemplate.FadeColor.R, ParticleTemplate.FadeColor.G, ParticleTemplate.FadeColor.B, ParticleTemplate.FadeColor.A });
            writer.Write(ParticleTemplate.FadeStartTime);
            writer.Write(ParticleTemplate.FadeTime);

            writer.Write(ParticleTemplate.Rotation);
            writer.Write(ParticleTemplate.RotationSpeed);

            writer.Write(InitialVelocity.X);
            writer.Write(InitialVelocity.Y);
            writer.Write(InitialRandom.X);
            writer.Write(InitialRandom.Y);

            writer.Write(Time);
            writer.Write(Amount);

            writer.Write(StartEnabled);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            ParticleTemplate = new Particle();
            ParticleTemplate.Template = true;

            ParticleTemplate.Color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            ParticleTemplate.Texture = Texture2D.FromStream(Engine.graphicsDevice, System.IO.File.OpenRead(reader.ReadString()));

            ParticleTemplate.Velocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            ParticleTemplate.FadeColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            ParticleTemplate.FadeStartTime = reader.ReadInt32();
            ParticleTemplate.FadeTime = reader.ReadInt32();

            ParticleTemplate.Rotation = reader.ReadInt32();
            ParticleTemplate.RotationSpeed = reader.ReadInt32();

            InitialVelocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            InitialRandom = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            Time = reader.ReadInt32();
            Amount = reader.ReadInt32();

            StartEnabled = reader.ReadBoolean();
        }
    }
}
