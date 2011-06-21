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
    public enum Direction : int { Left, Right, Up, Down }

    public class Target
    {
        public Engine Engine;
        public string Entity = "";

        public Target(Engine engine)
        {
            this.Engine = engine;
        }

        public void Trigger(Entity Sender)
        {
            Entity ent = Engine.World.FindEntitiy(Entity);
            if (ent != null)
                ent.Triggered(Sender);
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Entity);
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            Entity = reader.ReadString();
        }
    }

    public class Entity : ICloneable
    {
        public Engine Engine;

        public String Name = "";

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Size;

        public int Z;

        public Direction Direction;

        private string _parent = "";
        public string Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                if (Engine != null)
                {
                    Entity ent = Engine.World.FindEntitiy(value);
                    if (ent == null)
                    {
                        _parent = "";
                        ParentOffset = new Vector2(0);
                    }
                    else
                        ParentOffset = ent.Position - Position;
                }
            }
        }
        public Vector2 ParentOffset = new Vector2(0);

        public bool Solid;
        public bool Dynamic;
        public bool Template;
        public bool Visible = true;

        public float Friction = 0.15f;

        public bool MustSerialize = true;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public virtual void Triggered(Entity sender) { }

        public virtual void Initialize(World world)
        {
            this.Engine = world.Engine;
            world.Entities.Add(this);

            Entity ent = Engine.World.FindEntitiy(Parent);
            if (ent == null)
            {
                Parent = "";
                ParentOffset = new Vector2(0);
            }
            else
                ParentOffset = ent.Position - Position;
        }

        public virtual Rectangle GetCollision()
        {
            return new Rectangle((int)Position.X - (int)Size.X / 2, (int)Position.Y - (int)Size.Y / 2, (int)Size.X, (int)Size.Y);
        }

        public virtual Rectangle GetRenderRectangle()
        {
            return GetCollision();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Dynamic)
            {
                Velocity += Engine.World.Gravity / 100;
                Velocity.X *= 0.99f;

                Rectangle Future;
                Entity CollideWith;

                Future = GetCollision();
                Future.X += (int)Velocity.X;
                CollideWith = Engine.World.CollidesWithSolid(this, Future);
                if (CollideWith == null)
                    Position.X += (int)Velocity.X;
                else
                {
                    if (Position.X < CollideWith.Position.X - CollideWith.Size.X / 2)
                        Position.X = CollideWith.Position.X - CollideWith.Size.X;
                    else
                        Position.X = CollideWith.Position.X - CollideWith.Size.X;
                    Velocity.X = 0;
                }

                Future = GetCollision();
                Future.Y += (int)Velocity.Y;
                CollideWith = Engine.World.CollidesWithSolid(this, Future);
                if (CollideWith == null)
                    Position.Y += (int)Velocity.Y;
                else
                {
                    if (Position.Y < CollideWith.Position.Y - CollideWith.Size.Y / 2)
                    {
                        Position.Y = CollideWith.Position.Y - CollideWith.Size.Y;
                        Velocity.X *= 1f - CollideWith.Friction;
                    }
                    else
                        Position.Y = CollideWith.Position.Y + CollideWith.Size.Y;
                    Velocity.Y = 0;
                }
            }

            if (Parent != "")
            {
                Entity parent = Engine.World.FindEntitiy(Parent);
                Position = parent.Position - ParentOffset;
            }
        }

        public virtual void Render(SpriteBatch spriteBatch)
        {
            if (Engine.RenderDebug)
            {
                Rectangle rect = GetCollision();

                Color renderColor = Visible ? Color.Red : Color.Blue;

                // Cross
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X + rect.Width / 2, rect.Y, 1, rect.Height), renderColor);
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, 1), renderColor);

                // Top left
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X, rect.Y, 16, 1), renderColor);
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X, rect.Y, 1, 16), renderColor);

                // Top right
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X + rect.Width - 16, rect.Y, 16, 1), renderColor);
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, 16), renderColor);

                // Bottom left
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X, rect.Y + rect.Height - 1, 16, 1), renderColor);
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X, rect.Y + rect.Height - 16, 1, 16), renderColor);

                // Bottom right
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X + rect.Width - 16, rect.Y + rect.Height - 1, 16, 1), renderColor);
                spriteBatch.Draw(EngineContent.Textures.Pixel, new Rectangle(rect.X + rect.Width - 1, rect.Y + rect.Height - 16, 1, 16), renderColor);

                // Name
                spriteBatch.DrawString(EngineContent.Fonts.Calibri12, Name, Position + new Vector2(6), renderColor);
            }
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            if (MustSerialize)
            {
                writer.Write(Name);

                writer.Write(Position.X);
                writer.Write(Position.Y);
                writer.Write(Velocity.X);
                writer.Write(Velocity.Y);
                writer.Write(Size.X);
                writer.Write(Size.Y);

                writer.Write(Z);

                writer.Write((int)Direction);

                writer.Write(Parent);

                writer.Write(Solid);
                writer.Write(Dynamic);
                writer.Write(Template);
                writer.Write(Visible);

                writer.Write(Friction);
            }
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            if (MustSerialize)
            {
                Name = reader.ReadString();

                Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                Velocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                Size = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                Z = reader.ReadInt32();

                Direction = (Direction)reader.ReadInt32();

                Parent = reader.ReadString();

                Solid = reader.ReadBoolean();
                Dynamic = reader.ReadBoolean();
                Template = reader.ReadBoolean();
                Visible = reader.ReadBoolean();

                Friction = reader.ReadSingle();
            }
        }

        public virtual void Preload() { }
    }
}
