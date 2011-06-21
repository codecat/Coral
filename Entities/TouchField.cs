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
    public class TouchField : Entity
    {
        public List<string> DetectTypes = new List<string>();
        public List<Entity> TouchedEntities = new List<Entity>();

        public Target OnEnterTarget;
        public Target OnExitTarget;

        public override void Initialize(World world)
        {
            base.Initialize(world);
        }

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(DetectTypes.Count);
            foreach (string type in DetectTypes)
                writer.Write(type);

            if (OnEnterTarget != null)
                OnEnterTarget.Serialize(writer);
            else
                new Target(Engine).Serialize(writer);

            if (OnExitTarget != null)
                OnExitTarget.Serialize(writer);
            else
                new Target(Engine).Serialize(writer);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            int DetectTypesCount = reader.ReadInt32();
            for (int i = 0; i < DetectTypesCount; i++)
            {
                string typeName = reader.ReadString();
                DetectTypes.Add(typeName);
            }

            OnEnterTarget = new Target(Engine);
            OnEnterTarget.Deserialize(reader);

            OnExitTarget = new Target(Engine);
            OnExitTarget.Deserialize(reader);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < Engine.World.Entities.Count; i++)
            {
                Entity ent = Engine.World.Entities[i];
                if (DetectTypes.Contains(ent.GetType().FullName))
                {
                    if (ent.GetCollision().Intersects(GetCollision()) && !TouchedEntities.Contains(ent))
                    {
                        TouchedEntities.Add(ent);
                        if (OnEnterTarget != null)
                            OnEnterTarget.Trigger(this);
                    }
                    else if (!ent.GetCollision().Intersects(GetCollision()) && TouchedEntities.Contains(ent))
                    {
                        TouchedEntities.Remove(ent);
                        if (OnExitTarget != null)
                            OnExitTarget.Trigger(this);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            if (Engine.RenderDebug)
                spriteBatch.Draw(EngineContent.Textures.Pixel, GetRenderRectangle(), new Color(255, 255, 0, 90));

            base.Render(spriteBatch);
        }
    }
}
