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
    public class Prop : Entity
    {
        public string Texture = "";

        public bool Tiled = false;

        public Color Blending = Color.White;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Texture);
            writer.Write(Tiled);

            writer.Write(new byte[] { Blending.R, Blending.G, Blending.B });
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            Texture = reader.ReadString();
            Tiled = reader.ReadBoolean();

            Blending = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        public override void Preload()
        {
            ContentRegister.Texture(Texture);
        }

        public override void Update(GameTime gameTime)
        {


            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                if (Tiled)
                {
                    Rectangle renderRectangle = GetRenderRectangle();
                    Vector2 pos = new Vector2(renderRectangle.X, renderRectangle.Y);
                    renderRectangle.X = 0;
                    renderRectangle.Y = 0;
                    spriteBatch.Draw(ContentRegister.Texture(Texture), pos, renderRectangle, Blending, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
                else
                    spriteBatch.Draw(ContentRegister.Texture(Texture), GetRenderRectangle(), Blending);
            }

            base.Render(spriteBatch);
        }
    }
}
