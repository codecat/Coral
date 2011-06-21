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
    public class SoundEmitter : Entity
    {
        public string Filename;

        public override void Triggered(Entity sender)
        {
            base.Triggered(sender);

            ContentRegister.Sound(Filename).Play();
        }

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Filename);
        }

        public override void Deserialize(System.IO.BinaryReader reader)
        {
            base.Deserialize(reader);

            Filename = reader.ReadString();
        }

        public override void Preload()
        {
            ContentRegister.Sound(Filename);
        }
    }
}
