using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GammaDraconis.Video.Shaders
{
    class PostProcessShader : DrawableGameComponent
    {
        public List<string> effects;
        public List<int> divisions;
        protected List<Effect> _effects;

        public SpriteBatch spriteBatch;
        public RenderTarget2D source;

        private PresentationParameters pp;
        private SurfaceFormat format;

        private GammaDraconis game;

        public PostProcessShader(GammaDraconis game) : base(game)
        {
            game.Components.Add(this);
            this.game = game;

            effects = new List<string>();
            divisions = new List<int>();
        }

        public virtual void reset()
        {
            pp = game.GraphicsDevice.PresentationParameters;
            format = pp.BackBufferFormat;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            if (source.Width != width || source.Height != height)
            {
                source.Dispose();
                source = new RenderTarget2D(game.GraphicsDevice, width, height, 1, format);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            _effects = new List<Effect>();

            for (int i = 0; i < effects.Count; i++)
            {
                _effects.Add(game.Content.Load<Effect>(effects[i]));
            }

            pp = game.GraphicsDevice.PresentationParameters;
            format = pp.BackBufferFormat;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            source = new RenderTarget2D(game.GraphicsDevice, width, height, 1, format);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            source.Dispose();

            base.UnloadContent();
        }

        public void Render()
        {
            SurfaceFormat format = pp.BackBufferFormat;

            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;

            game.GraphicsDevice.SetRenderTarget(1, source);

            ResolveTexture2D resolveTexture = new ResolveTexture2D(game.GraphicsDevice, width, height, 1, format);
            game.GraphicsDevice.ResolveBackBuffer(resolveTexture);

            game.GraphicsDevice.SetRenderTarget(1, null);

            Texture2D texture = (Texture2D)resolveTexture;
            
            for (int i = 0; i < effects.Count; i++)
            {
                Effect effect = _effects[i];

                width = pp.BackBufferWidth / divisions[i];
                height = pp.BackBufferHeight / divisions[i];

                if (source.Width != width || source.Height != height)
                {
                    source.Dispose();
                    source = new RenderTarget2D(game.GraphicsDevice, width, height, 1, format);
                }
                
                DrawFullscreenQuad(texture, source, effect);
                texture.Dispose();
                texture = source.GetTexture();
            }
        }

        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
                                Effect effect)
        {
            game.GraphicsDevice.SetRenderTarget(2, renderTarget);
            game.GraphicsDevice.Clear(Color.Black);

            DrawFullscreenQuad(texture,
                               renderTarget.Width, renderTarget.Height,
                               effect);

            game.GraphicsDevice.SetRenderTarget(2, null);
        }

        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, int width, int height,
                                Effect effect)
        {
            spriteBatch.Begin(SpriteBlendMode.None,
                              SpriteSortMode.Immediate,
                              SaveStateMode.None);

            // Begin the custom effect, if it is currently enabled. If the user
            // has selected one of the show intermediate buffer options, we still
            // draw the quad to make sure the image will end up on the screen,
            // but might need to skip applying the custom pixel shader.
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();
            
            // Draw the quad.
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();

            // End the custom effect.
            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }

    }
}
