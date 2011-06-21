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
    public class Teleport : Entity
    {
        public Target Target;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            if (Target != null)
                Target.Serialize(writer);
            else
                new Target(Engine).Serialize(writer);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            Target = new Target(Engine);
            Target.Deserialize(reader);
        }
    }
}
