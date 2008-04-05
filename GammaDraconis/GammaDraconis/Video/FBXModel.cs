using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GammaDraconis.Types;

namespace GammaDraconis.Video
{
    /// <summary>
    /// A model contains all the information needed to represent a 
    /// three-dimensional mesh that can be drawn by the renderer.
    /// A model should also contain its own collision mesh.
    /// </summary>
    class FBXModel : DrawableGameComponent
    {
        // The .fbx model represented by this object
        public Model model;
        public string filename;

        // Shader effect associated with the model
        public Effect _effect;
        public string effect = "Resources/Effects/Basic";

        // Model scaling
        public float scale;

        public FBXModel(string filename) : this(filename, "") { }
        public FBXModel(string filename, string effect) : this(filename, effect, 1f) { }
        public FBXModel(string filename, string effect, float scale) : base(GammaDraconis.GetInstance())
        {
            this.filename = filename;
            if (effect != "")
            {
                this.effect = effect;
            }
            this.scale = scale;
            offset = new Coords();
            GammaDraconis.GetInstance().Components.Add(this);
        }

        // Offset position/rotation, relative to the *game object's* identity matrix
        public Coords offset;

        protected override void LoadContent()
        {
            model = Game.Content.Load<Model>(filename);
            _effect = Game.Content.Load<Effect>(effect);
            _effect.CurrentTechnique = _effect.Techniques[0];
            base.LoadContent();
        }

        public FBXModel clone()
        {
            FBXModel fbxmodel = new FBXModel(filename);
            fbxmodel.scale = scale;
            fbxmodel.effect = effect;
            fbxmodel._effect = _effect;
            fbxmodel.model = model;

            return fbxmodel;
        }
    }
}
