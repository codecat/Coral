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
    public class Light : Entity
    {
        public Color Color;
        public int Brightness;

        public bool Enabled;

        public override void Initialize(World world)
        {
            base.Initialize(world);

            this.Size = new Vector2(Brightness * 10);
            this.Color.A = (byte)Brightness;
        }

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(new byte[] { Color.R, Color.G, Color.B });
            writer.Write(Brightness);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            Color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            Brightness = reader.ReadInt32();
        }

        public override void Update(GameTime gameTime)
        {
            this.Size = new Vector2(Brightness * 10);
            this.Color.A = (byte)Math.Max(Math.Min(Brightness * 3, 255), 0);

            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(EngineContent.Textures.Light, GetCollision(), Color);

            base.Render(spriteBatch);
        }
    }
}
