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
    public enum RoutingType : int { Additive, Subtractive, Absolute }

    public class GravityRouter : Entity
    {
        public RoutingType Type;
        public Vector2 AddVector;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)Type);
            writer.Write(AddVector.X);
            writer.Write(AddVector.Y);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            Type = (RoutingType)reader.ReadInt32();
            AddVector = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public override void Triggered(Entity sender)
        {
            switch (Type)
            {
                case RoutingType.Additive: Engine.World.Gravity += AddVector; break;
                case RoutingType.Subtractive: Engine.World.Gravity -= AddVector; break;
                case RoutingType.Absolute: Engine.World.Gravity = new Vector2(AddVector.X, AddVector.Y); break;
            }

            base.Triggered(sender);
        }
    }
}
