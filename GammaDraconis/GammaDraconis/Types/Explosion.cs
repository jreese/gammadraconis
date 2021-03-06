using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GammaDraconis.Core;
using GammaDraconis.Video;

namespace GammaDraconis.Types
{
    class Explosion
    {
        public List<Bullet> bullets;
        public int particles = 50;
        public float size = 1f;
        public float power = 1f;

        private Random rand;

        /// <summary>
        /// Create a new explosion.
        /// </summary>
        public Explosion()
        {
            bullets = new List<Bullet>();
            rand = new Random();
        }

        /// <summary>
        /// Clone an Explosion object.
        /// </summary>
        /// <returns>The cloned Explosion object.</returns>
        public Explosion clone()
        {
            Explosion e = new Explosion();

            e.bullets.AddRange( bullets );
            e.particles = particles;
            e.size = size;

            return e;
        }

        /// <summary>
        /// Explode an explosion.
        /// </summary>
        /// <param name="spot">The starting point for the explosion.</param>
        public void explode(Coords spot) { explode(spot, new Coords()); }

        /// <summary>
        /// Explode an explosion.
        /// </summary>
        /// <param name="spot">The starting point for the explosion.</param>
        /// <param name="vel">The starting velocity for the explosion.</param>
        public void explode(Coords spot, Coords vel)
        {
            Coords velocity = new Coords(vel.pos());
            Coords position = new Coords(spot.pos());
            Bullet bullet;
            FBXModel model;
            
            if (bullets.Count < 1)
            {
                Console.WriteLine("Loading Explosion Bullets.");
                bullet = new Bullet();
                bullet.damage = 100 * power;
                bullet.rateL = 35 * size;
                bullet.dragL = 0.3f;
                bullet.dragR = 0.2f;
                bullet.size = 2;
                bullet.timeToLive = 5;
                model = new FBXModel("Resources/Models/Ember1", "", 1.0f);
                model.lighted = false;
                bullet.models.Add(model);
                bullets.Add(bullet);

                bullet = bullet.clone();
                bullet.damage = 100 * power;
                bullet.rateL = 35 * size;
                bullet.dragL = 0.3f;
                bullet.dragR = 0.2f;
                bullet.size = 2;
                bullet.timeToLive = 5;
                model = new FBXModel("Resources/Models/Ember1", "", 1.0f);
                model.lighted = false;
                bullet.models.Add(model);
                bullets.Add(bullet);

                bullet = new Bullet();
                bullet.damage = 150 * power;
                bullet.rateL = 20 * size;
                bullet.dragL = 0.5f;
                bullet.dragR = 0.3f;
                bullet.size = 2;
                bullet.timeToLive = 5;
                model = new FBXModel("Resources/Models/Ember1", "", 2.0f);
                model.lighted = false;
                bullet.models.Add(model);
                bullets.Add(bullet);

                bullet = new Bullet();
                bullet.damage = 80 * power;
                bullet.rateL = 30 * size;
                bullet.dragL = 0.5f;
                bullet.dragR = 0.3f;
                bullet.size = 2;
                bullet.timeToLive = 5;
                model = new FBXModel("Resources/Models/Ember2", "", 1.0f);
                model.lighted = false;
                bullet.models.Add(model);
                bullets.Add(bullet);

                bullet = new Bullet();
                bullet.damage = 100 * power;
                bullet.rateL = 45 * size;
                bullet.dragL = 0.3f;
                bullet.dragR = 0.3f;
                bullet.size = 2;
                bullet.timeToLive = 5;
                model = new FBXModel("Resources/Models/Ember2", "", 0.6f);
                model.lighted = false;
                bullet.models.Add(model);
                bullets.Add(bullet);

                bullet = new Bullet();
                bullet.damage = 100 * power;
                bullet.rateL = 45 * size;
                bullet.dragL = 0.3f;
                bullet.dragR = 0.3f;
                bullet.size = 2;
                bullet.timeToLive = 5;
                model = new FBXModel("Resources/Models/Ember2", "", 0.6f);
                model.lighted = false;
                bullet.models.Add(model);
                bullets.Add(bullet);
            }

            for (int i = 0; i < particles; i++)
            {
                Console.WriteLine("Explosion Bullet");
                int m = rand.Next(bullets.Count);
                bullet = bullets[m].clone();
                bullet.scaleModels(size);

                bullet.lastPosition = position.Clone();
                bullet.velocity = velocity.Clone();

                bullet.position.T = position.T;
                bullet.position.R = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)(MathHelper.TwoPi * rand.NextDouble())) *
                    Quaternion.CreateFromAxisAngle(Vector3.Backward, (float)(MathHelper.TwoPi * rand.NextDouble())) *
                    Quaternion.CreateFromAxisAngle(Vector3.Right, (float)(MathHelper.TwoPi * rand.NextDouble()));

                bullet.throttle(1f);

                Engine.GetInstance().gameScene.track(bullet, GO_TYPE.BULLET);
            }
        }
    }
}
