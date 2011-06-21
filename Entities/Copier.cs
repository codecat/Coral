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
    public class Copier : Entity
    {
        public string Template;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Template);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            Template = reader.ReadString();
        }

        public override void Triggered(Entity sender)
        {
            Entity template = Engine.World.FindEntitiy(Template);
            if (template != null)
            {
                Entity newEntity = (Entity)template.Clone();
                newEntity.Position = new Vector2(Position.X, Position.Y);
                newEntity.Template = false;
                newEntity.Initialize(Engine.World);
            }

            base.Triggered(sender);
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            if (Engine.RenderDebug)
                spriteBatch.Draw(EngineContent.Textures.Pixel, GetRenderRectangle(), new Color(123, 123, 64, 90));

            base.Render(spriteBatch);
        }
    }
}
