using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GammaDraconis.Video.GUI
{
    class InterfaceComponent : DrawableGameComponent
    {
        public Vector2 RelativePosition;
        public Vector2 RelativeScale;
        public float RelativeRotation;

        protected SpriteBatch spriteBatch;

        public InterfaceComponent(GammaDraconis game)
            : base(game)
        {
            game.Components.Add(this);
            RelativePosition = Vector2.Zero;
            RelativeScale = Vector2.One;
            RelativeRotation = 0;
            Visible = false;
            Enabled = false;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        internal virtual void Draw(GameTime gameTime, Vector2 position, Vector2 scale, float rotation)
        {
        }

        protected void CalculateResultingValues(Vector2 position, Vector2 scale, float rotation, out Vector2 outPos, out Vector2 outScale, out float outRotation)
        {
            // Adjust the position based on the scaling of the parent (NOT the scale of this component).
            outPos = (position + RelativePosition) * scale;
            // Adjust the scale and rotation based on the relative scaling and rotation
            outScale = scale * RelativeScale;
            outRotation = rotation + RelativeRotation;
        }
    }
}
