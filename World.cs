using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;

namespace CoralEngine
{
    public class World
    {
        public Engine Engine;

        public string Filename;

        public string Name = "";
        public string Author = "";
        public string Description = "";

        public Color Background = Color.Black;

        public List<Entity> Entities = new List<Entity>();

        public Vector2 Gravity;

        public static World Empty(Engine engine)
        {
            World world = new World();

            world.Filename = "";
            world.Engine = engine;

            world.Name = "";
            world.Author = "";
            world.Description = "";

            world.Background = Color.Black;

            world.Gravity = new Vector2(0, 30);

            return world;
        }

        public static void FromFile(Engine engine, string Filename)
        {
            Console.WriteLine("Loading world from " + Filename + "...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            World ret = new World();

            ret.Filename = Filename;
            ret.Engine = engine;

            engine.World = ret;

            BinaryReader reader = new BinaryReader(File.OpenRead(Filename));

            ret.Name = reader.ReadString();
            ret.Author = reader.ReadString();
            ret.Description = reader.ReadString();

            ret.Background = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

            ret.Gravity = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            int EntityCount = reader.ReadInt32();
            for (int i = 0; i < EntityCount; i++)
            {
                string EntityType = reader.ReadString();
                Entity ent = (Entity)System.Activator.CreateInstance(Type.GetType("CoralEngine." + EntityType));
                ent.Initialize(ret);
                ent.Deserialize(reader);
            }

            sw.Stop();
            Console.WriteLine("World loaded in " + sw.ElapsedMilliseconds.ToString() + "ms");
        }

        public void Save(string filename = "")
        {
            if (filename == "")
                filename = this.Filename;

            Console.WriteLine("Writing " + filename + "...");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (File.Exists(filename))
                File.WriteAllBytes(filename, new byte[0]);

            BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename));

            writer.Write(Name);
            writer.Write(Author);
            writer.Write(Description);

            writer.Write(new byte[] { Background.R, Background.G, Background.B });

            writer.Write(Gravity.X);
            writer.Write(Gravity.Y);

            writer.Write(Entities.Count);

            Console.WriteLine("Level header size: " + writer.BaseStream.Position.ToString() + " bytes");

            bool NoNamesFound = false;
            foreach (Entity ent in Entities)
            {
                long firstLength = writer.BaseStream.Position;

                writer.Write(ent.GetType().Name);
                ent.Serialize(writer);

                if (ent.Name == "")
                {
                    NoNamesFound = true;

                    ConsoleColor originalColor = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("*** ");
                    Console.ForegroundColor = originalColor;
                }
                Console.WriteLine(ent.GetType().Name + (ent.Name != "" ? " (" + ent.Name + ")" : "") + ": " + (writer.BaseStream.Position - firstLength).ToString() + " bytes");
            }

            Console.WriteLine("Total filesize: " + writer.BaseStream.Position.ToString() + " bytes\n");

            if (NoNamesFound)
            {
                ConsoleColor originalColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("***");
                Console.ForegroundColor = originalColor;

                Console.WriteLine(" You have entities without names! It might be harder to organize the world.");
            }

            writer.Close();

            sw.Stop();
            Console.WriteLine("World saved in " + sw.ElapsedMilliseconds.ToString() + "ms");
        }

        public Entity CollidesWithSolid(Entity sender, Rectangle collision)
        {
            Entity[] solidEntities = Engine.World.FindSolidEntities();
            foreach (Entity ent in solidEntities)
            {
                if (ent == sender)
                    continue;

                if (ent.GetCollision().Intersects(collision))
                    return ent;
            }
            return null;
        }

        public Entity FindEntitiy(string Name)
        {
            foreach (Entity ent in Entities)
            {
                if (ent.Name == Name)
                    return ent;
            }
            return null;
        }

        public Entity[] FindEntitiesByType(Type ofType)
        {
            List<Entity> ret = new List<Entity>();
            foreach (Entity ent in Entities)
            {
                if (ofType == null || ent.GetType() == ofType)
                    ret.Add(ent);
            }
            return ret.ToArray();
        }

        public Entity[] FindSolidEntities<T>()
        {
            List<Entity> ret = new List<Entity>();
            Entity[] entitiesByType = FindEntitiesByType(typeof(T));
            foreach (Entity ent in entitiesByType)
            {
                if (ent.Solid)
                    ret.Add(ent);
            }
            return ret.ToArray();
        }

        public Entity[] FindSolidEntities()
        {
            List<Entity> ret = new List<Entity>();
            foreach (Entity ent in Entities)
            {
                if (ent.Solid)
                    ret.Add(ent);
            }
            return ret.ToArray();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (!Entities[i].Template)
                    Entities[i].Update(gameTime);
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            Engine.graphicsDevice.Clear(Background);

            Entity[] orderedEntities = Entities.OrderBy(ent => ent.Z).ToArray<Entity>();

            for (int i = 0; i < orderedEntities.Length; i++)
            {
                if (orderedEntities[i].GetType() != typeof(Light) && !orderedEntities[i].Template)
                    orderedEntities[i].Render(spriteBatch);
            }
        }
    }
}
