using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GammaDraconis.Types
{
    /// <summary>
    /// Represents a position / rotation coordinate in space-time.  This can 
    /// be an object's position, velocity, or acceleration.
    /// </summary>
    class Coords
    {
        // Coordinates
        public Matrix T;

        // Rotations
        public Quaternion R;

        /// <summary>
        /// Build a Coords object with the given properties.
        /// </summary>
        /// <param name="x">X axis position</param>
        /// <param name="y">Y axis position</param>
        /// <param name="z">Z axis position</param>
        /// <param name="a">Alpha rotation (X axis / Pitch)</param>
        /// <param name="b">Beta rotation (Y axis / Yaw)</param>
        /// <param name="g">Gamma rotation (Z axis / Roll)</param>
        public Coords(float x, float y, float z, float a, float b, float g)
        {
            T = Matrix.CreateTranslation(x, y, z);
            R = Quaternion.CreateFromYawPitchRoll(b, a, g);
        }
        public Coords(float x, float y, float z) : this(x, y, z, 0, 0, 0) { }
        public Coords() : this(0, 0, 0, 0, 0, 0) { }

        /// <summary>
        /// Get a Vector3 object containing this object's rotation.
        /// </summary>
        /// <returns>Positional Vector3</returns>
        public Vector3 pos()
        {
            return T.Translation;
        }

        public Vector3 rot()
        {
            return Vector3.Up;
        }

        /// <summary>
        /// Get a Matrix that contains both the translation and rotation of this object.
        /// </summary>
        /// <returns>Combined translation and rotation matrix</returns>
        public Matrix matrix()
        {
            return Matrix.CreateFromQuaternion(R) * T;
        }
    }
}
