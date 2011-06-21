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
    public class Trigger : Entity
    {
        public List<Target> Targets;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Targets.Count);
            foreach (Target target in Targets)
                target.Serialize(writer);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            int targetCount = reader.ReadInt32();
            for (int i = 0; i < targetCount; i++)
            {
                Target newTarget = new Target(Engine);
                newTarget.Deserialize(reader);
                Targets.Add(newTarget);
            }
        }
    }
}
