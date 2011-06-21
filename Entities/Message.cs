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
    public class Message : Entity
    {
        public string Text;
        public Color Color;
        public int DisplayTime;

        private int Displaying;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Text);
            writer.Write(new byte[] { Color.R, Color.G, Color.B, Color.A });
            writer.Write(DisplayTime);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            Text = reader.ReadString();
            Color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            DisplayTime = reader.ReadInt32();
        }

        public override void Triggered(Entity sender)
        {
            base.Triggered(sender);

            Displaying = DisplayTime;
        }

        public override void Update(GameTime gameTime)
        {
            if (Displaying > 0)
                Displaying--;

            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            if (Displaying > 0)
            {
                Vector2 TextSize = EngineContent.Fonts.Calibri12Bold.MeasureString(Text);
                spriteBatch.DrawString(EngineContent.Fonts.Calibri12Bold, Text, new Vector2(Engine.graphicsDevice.Viewport.Width / 2f - (TextSize.X / 2f), (Engine.graphicsDevice.Viewport.Height) / 3f * 2f), Color);
            }

            base.Render(spriteBatch);
        }
    }
}
