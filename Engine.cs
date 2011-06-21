using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace CoralEngine
{
    public class Engine
    {
        public String Version = "0.1";

        public World World;

        public GraphicsDevice graphicsDevice;

        public RenderTarget2D LightingTarget;

        public bool RenderDebug = false;
        public bool Fullbright = true;

        public Color ShadowColor = new Color(50, 50, 50);

        public Engine(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            LightingTarget = new RenderTarget2D(this.graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            Console.WriteLine(String.Format(@"
///////////////////////////////////////////////////
// Coral Engine {0}
// (c) MrAG.nl 2011
///////////////////////////////////////////////////
", Version));
        }

        public void LoadWorld(string Filename)
        {
            World.FromFile(this, Filename);
        }

        public void LoadEmptyWorld()
        {
            Console.WriteLine("Creating empty world.");
            World = World.Empty(this);
        }

        public void SaveWorld()
        {
            World.Save();
        }

        public void PreloadWorldContent()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (Entity ent in World.Entities)
                ent.Preload();

            sw.Stop();
            Console.WriteLine("Content preloaded in " + sw.ElapsedMilliseconds.ToString() + "ms");
        }

        public void Initialize()
        {
            ContentRegister.Engine = this;
        }

        public void LoadContent(ContentManager content)
        {
            EngineContent.LoadContent(content);
        }

        public void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            // Draw lighting to LightingTarget
            SetRenderTarget(true);
            graphicsDevice.Clear(ShadowColor);
            foreach (Entity ent in World.Entities)
            {
                if (ent.GetType() == typeof(Light))
                    (ent as Light).Render(spriteBatch);
            }

            // Main graphics
            SetRenderTarget(false);
            World.Render(spriteBatch);

            // Overlay lighting
            if (!Fullbright)
            {
                BlendState bs = new BlendState();
                bs.AlphaSourceBlend = Blend.DestinationAlpha;
                bs.ColorSourceBlend = Blend.DestinationColor;
                bs.AlphaDestinationBlend = Blend.Zero;
                bs.ColorDestinationBlend = Blend.Zero;
                graphicsDevice.BlendState = bs;
                spriteBatch.Draw((Texture2D)LightingTarget, new Rectangle(0, 0, LightingTarget.Width, LightingTarget.Height), Color.White);
            }
        }

        public void SetRenderTarget(bool Lighting)
        {
            if (!Lighting)
            {
                graphicsDevice.SetRenderTarget(null);
                BlendState bs = new BlendState();
                bs.AlphaSourceBlend = Blend.SourceAlpha;
                bs.ColorSourceBlend = Blend.SourceAlpha;
                bs.AlphaDestinationBlend = Blend.InverseSourceAlpha;
                bs.ColorDestinationBlend = Blend.InverseSourceAlpha;
                graphicsDevice.BlendState = bs;
            }
            else
                graphicsDevice.SetRenderTarget(LightingTarget);
        }
    }
}
