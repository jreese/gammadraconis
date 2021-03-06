using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GammaDraconis.Core;
using GammaDraconis.Core.Input;
using GammaDraconis.Video;
using GammaDraconis.Video.GUI;


namespace GammaDraconis.Types
{
    /// <summary>
    /// A Player is a special racer that is controlled by a human.
    /// </summary>
    class Player : Racer
    {
        public static Player[] players = new Player[4];
        public static Random random = new Random();

        public PlayerIndex index;
        public PlayerInput input;
        public GameObject arrow;

        double invulnerabilityTimer = 0;
        bool dead = false;
        int deathTimer = 0;

        public Interface playerHUD;

        public List<GameObject> dust;
        public static float dustCount = 60;
        public static float dustDistance = 350f;

        private float rateModifier = 1;

        /// <summary>
        /// Construct a player at the specified PlayerIndex.
        /// </summary>
        /// <param name="index">The player's PlayerIndex.</param>
        public Player(PlayerIndex index)
            : base()
        {
            this.index = index;

            input = GammaDraconis.GetInstance().InputManager.GetPlayerInput(index);
            camera = new Coords();
            viewport = (Renderer.Viewports)index;
            Player.players[(int)index] = this;

            arrow = Proto.getThing("CheckpointArrow", new Coords());
            
            playerHUD = (Interface)GammaDraconis.GetInstance().GameLua.DoString("playerHudIndex = " + ((int)index + 1) + "\nreturn dofile( 'Interfaces/PlayerHUD/PlayerHUD.lua' )")[0];

            dust = new List<GameObject>();

            explosion = new Explosion();
            explosion.size = 2f;
            explosion.particles = 50;
        }

        /// <summary>
        /// Set up dust particles to give a sense of speed.
        /// </summary>
        public void setupDust() {
            float dist = 2f * dustDistance;
            float dist2 = dustDistance;
            
            for (int i = 0; i < dustCount; i++)
            {
                float fx = (float)random.NextDouble() * dist - dist2;
                float fy = (float)random.NextDouble() * dist - dist2;
                float fz = (float)random.NextDouble() * dist - dist2;
                
                GameObject dusto = Proto.getThing("Dust");
                dusto.position.T = Matrix.CreateTranslation(fx, fy, fz) * position.matrix();
                dust.Add(dusto);
            }
        }

        private int thinkcount = 0;

        /// <summary>
        /// Handle death, input, and dust.
        /// </summary>
        /// <param name="gameTime">The current time.</param>
        public override void think(GameTime gameTime)
        {
            playerHUD.Update(gameTime);

            #region Death handling
            if (health <= 0 && !dead)
            {
                dead = true;
                velocity = new Coords();
                deathTimer = 2000;
                
                OnDeath();
                Engine.GetInstance().gameScene.ignore(this);
                foreach (FBXModel model in models)
                {
                    model.visible = false;
                }
                shieldModel.visible = false;
                Engine.GetInstance().gameScene.track(this, GO_TYPE.THINKABLE);
            }

            if (dead)
            {
                deathTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (deathTimer <= 0)
                {
                    dead = false;
                    health = maxHealth;
                    shield = maxShield;
                    position = Engine.GetInstance().race.checkpoint(this, 0).position.Clone();
                    invulnerabilityTimer = 2 + gameTime.ElapsedRealTime.TotalSeconds;

                    Engine.GetInstance().gameScene.ignore(this);
                    foreach (FBXModel model in models)
                    {
                        model.visible = true;
                    }
                    Engine.GetInstance().gameScene.track(this, GO_TYPE.PLAYER);
                }
                else
                {
                    base.think(gameTime);
                    return;
                }
            }
            #endregion

            #region Invulnerability Timer
            if (invulnerabilityTimer > 0)
            {
                invincible = true;
                invulnerabilityTimer -= gameTime.ElapsedRealTime.TotalSeconds;
            }
            else
            {
                invincible = false;
            }
            #endregion

            #region Keyboard input handling
            if (input.inputDown("Up"))
            {
                pitch(1);
            }
            if (input.inputDown("Down"))
            {
                pitch(-1);
            }
            if (input.inputDown("Left"))
            {
                yaw(-1);
            }
            if (input.inputDown("Right"))
            {
                yaw(1);
            }
            if (input.inputDown("RollLeft"))
            {
                roll(-1);
            }
            if (input.inputDown("RollRight"))
            {
                roll(1);
            }

            if (input.inputDown("ThrottleUp"))
            {
                throttle(1);
            }
            if (input.inputDown("ThrottleDown"))
            {
                throttle(-1);
            }
            if (input.inputDown("Reset"))
            {
                health = 0;
            }
            if (input.inputPressed("Pause"))
            {
                Engine.GetInstance().enginePaused = !Engine.GetInstance().enginePaused;
            }
            if(input.inputPressed("Menu"))
            {
                Audio.stopAll();
                Audio.play("ambience");
                GammaDraconis.GetInstance().changeState(GammaDraconis.GameStates.MainMenu);
            }
            #endregion

            #region Gamepad input handling
            {
                pitch(input.axis("Pitch"));
                turn(input.axis("Turn"));
                roll(input.axis("Roll"));
                yaw(input.axis("Yaw"));

                throttle(input.axis("Throttle"));

                // Rotate the camera around the player
                camera.R = Quaternion.CreateFromYawPitchRoll((float)Math.PI * input.axis("CameraX"), (float)Math.PI * -input.axis("CameraY"), 0f);
            }
            #endregion

            #region Controller-independent handling
            {
                if (input.inputDown("Fire1"))
                {
                    firePrimary();
                }
				if (input.inputDown("Fire2"))
				{
					fireSecondary();
				}
            }
            #endregion

            #region It's getting dusty in here
            Vector3 pos = position.pos();
            Matrix m = position.matrix();

            float dist = 1f * dustDistance;
            float dist2 = 0.5f * dustDistance;
            float distZ = dustDistance * 0.8f;

            foreach (GameObject dusto in dust)
            {
                if (Vector3.Distance(pos, dusto.position.pos()) > dustDistance)
                {
                    float fx = (float)random.NextDouble() * dist - dist2;
                    float fy = (float)random.NextDouble() * dist - dist2;

                    dusto.position.T = Matrix.CreateTranslation(fx, fy, -distZ) * m;
                }

            }
            #endregion

            base.think(gameTime);
        }

        public override string ToString()
        {
            return "Player " + index;
        }

        /// <summary>
        /// Create a Player object from a ship definition.
        /// </summary>
        /// <param name="ship">The target ship</param>
        /// <param name="index">Player index</param>
        /// <returns>New player object</returns>
        public static Player cloneShip(GameObject ship, PlayerIndex index)
        {
            Player go = new Player(index);

            go.mass = ship.mass;
            go.size = ship.size;

            if (ship.explosion != null)
            {
                go.explosion = ship.explosion.clone();
            }
            
            go.rateL = ship.rateL;
            go.rateR = ship.rateR;
            go.dragL = ship.dragL;
            go.dragR = ship.dragR;

            go.maxHealth = ship.maxHealth;
            go.health = ship.maxHealth;
            go.maxShield = ship.maxShield;
            go.shield = ship.maxShield;
            go.shieldIncreaseRate = ship.shieldIncreaseRate;

            foreach (FBXModel model in ship.models)
            {
                go.models.Add(model.clone());
            }

            if (ship.shieldModel != null)
            {
                go.shieldModel = ship.shieldModel.clone();
            }
            
            foreach(MountPoint mount in ship.mounts)
            {
                go.mounts.Add(mount.clone());
            }

            foreach (Turret turret in ship.turrets)
            {
                go.turrets.Add(turret.clone());
            }

            go.relativeLookAt = new Vector3(ship.relativeLookAt.X, ship.relativeLookAt.Y, ship.relativeLookAt.Z);
            go.relativeLookFrom = new Vector3(ship.relativeLookFrom.X, ship.relativeLookFrom.Y, ship.relativeLookFrom.Z);
            go.relationalScale = ship.relationalScale;
            
            go.thrusterSFX = ship.thrusterSFX;
            go.engine_startSFX = ship.engine_startSFX;
            go.onDeathSound = ship.onDeathSound;

            return go;
        }
    }
}
