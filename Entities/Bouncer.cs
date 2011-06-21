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
    public enum BouncerMode : int { Additive, Subtractive, Absolute }

    public class Bouncer : Entity
    {
        public List<Entity> TouchedEntities = new List<Entity>();

        public Vector2 AddVelocity;

        public BouncerMode Mode;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(AddVelocity.X);
            writer.Write(AddVelocity.Y);

            writer.Write((int)Mode);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            AddVelocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            Mode = (BouncerMode)reader.ReadInt32();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Entity ent in Engine.World.Entities)
            {
                if (ent.GetCollision().Intersects(GetCollision()))
                {
                    if (ent.Dynamic && !TouchedEntities.Contains(ent))
                    {
                        TouchedEntities.Add(ent);
                        switch (Mode)
                        {
                            case BouncerMode.Additive: ent.Velocity += AddVelocity; break;
                            case BouncerMode.Subtractive: ent.Velocity -= AddVelocity; break;
                            case BouncerMode.Absolute: ent.Velocity = new Vector2(AddVelocity.X, AddVelocity.Y); break;
                        }
                    }
                }
                else if (TouchedEntities.Contains(ent))
                    TouchedEntities.Remove(ent);
            }

            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            if (Engine.RenderDebug)
                spriteBatch.Draw(EngineContent.Textures.Pixel, GetRenderRectangle(), new Color(255, 0, 128, 90));

            base.Render(spriteBatch);
        }
    }
}
