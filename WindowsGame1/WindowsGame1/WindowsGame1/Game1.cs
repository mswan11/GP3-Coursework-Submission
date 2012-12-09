using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState gameState;

        #region User Defined Variables
        //------------------------------------------
        // Added for use with fonts
        //------------------------------------------
        SpriteFont fontToUse;

        //--------------------------------------------------
        // Added for use with playing Audio via Media player
        //--------------------------------------------------
        private Song bkgMusic;
        private String songInfo;
        //--------------------------------------------------
        //Set the sound effects to use
        //--------------------------------------------------
        //private SoundEffectInstance tardisSoundInstance;
        //private SoundEffect tardisSound;
        //private SoundEffect explosionSound;
        private SoundEffect firingSound;

        public Texture2D ground;
        private Vector3 groundPosition;

        // Set the 3D model to draw.
        private Player player1;
        private Model mdlPlayer;
        private Matrix[] mdlPlayerTransforms;

        // The aspect ratio determines how to scale 3d to 2d projection.
        private float aspectRatio;

        // create an array of enemy daleks
        private Model mdlDalek;
        private Matrix[] mdDalekTransforms;
        private Daleks[] dalekList = new Daleks[GameConstants.NumDaleks];

        // create an array of laser bullets
        private Model mdlLaser;
        private Matrix[] mdlLaserTransforms;
        private Laser[] laserList = new Laser[GameConstants.NumLasers];

        public int volumeMultiplier = 1;


        private Random random = new Random();

        
        private int hitCount;

        // Set the position of the camera in world space, for our view matrix.
        public GameCamera camera;
        

        private void InitializeTransform()
        {
            player1 = new Player();

            camera = new GameCamera(player1.playerPosition);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            camera.camViewMatrix = Matrix.CreateLookAt(camera.camPosition, Vector3.Zero, Vector3.Up);

            camera.camProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45), aspectRatio, 1.0f, 350.0f);

        }

        

        private void ResetDaleks()
        {
            float xStart;
            float zStart;
            for (int i = 0; i < GameConstants.NumDaleks; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                dalekList[i].position = new Vector3(xStart, 0.0f, zStart);
                double angle = random.NextDouble() * 2 * Math.PI;
                dalekList[i].direction.X = -(float)Math.Sin(angle);
                dalekList[i].direction.Z = (float)Math.Cos(angle);
                dalekList[i].speed = GameConstants.DalekMinSpeed +
                   (float)random.NextDouble() * GameConstants.DalekMaxSpeed;
                dalekList[i].isActive = true;
            }

        }

        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.camProjectionMatrix;
                    effect.View = camera.camViewMatrix;
                }
            }
            return absoluteTransforms;
        }

        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    effect.View = Matrix.CreateLookAt(camera.camPosition, camera.camLookat, Vector3.Forward);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 50000.0f);
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        ////private void writeText(string msg, Vector2 msgPos, Color msgColour)
        //{
        //    spriteBatch.Begin();
        //    string output = msg;
        //    // Find the center of the string
        //    //Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
        //    //Vector2 FontPos = msgPos;
        //    // Draw the string
        //    spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
        //    spriteBatch.End();
        //}

        #endregion


        public enum GameState
        {
            Menu,
            Prologue,
            Game
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 800;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = false;
            Window.Title = "Khallay Masha";
            
            Window.AllowUserResizing = false;
            hitCount = 0;
            InitializeTransform();
            ResetDaleks();
            gameState = GameState.Menu;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            //-------------------------------------------------------------
            // added to load Model
            //-------------------------------------------------------------
            mdlPlayer = Content.Load<Model>(".\\Models\\player");
            mdlPlayerTransforms = SetupEffectTransformDefaults(mdlPlayer);
            mdlDalek = Content.Load<Model>(".\\Models\\dalek");
            mdDalekTransforms = SetupEffectTransformDefaults(mdlDalek);
            mdlLaser = Content.Load<Model>(".\\Models\\laser");
            mdlLaserTransforms = SetupEffectTransformDefaults(mdlLaser);
            ground = Content.Load<Texture2D>(".\\Textures\\sand");
            //-------------------------------------------------------------
            // added to load SoundFX's
            //-------------------------------------------------------------
            //tardisSound = Content.Load<SoundEffect>("Audio\\tardisEdit");
            //explosionSound = Content.Load<SoundEffect>("Audio\\explosion2");
            firingSound = Content.Load<SoundEffect>("Audio\\shot");
            //tardisSoundInstance = tardisSound.CreateInstance();
            //tardisSoundInstance.Play();


             // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            int seconds = 61;
                if ((keyboardState.IsKeyDown(Keys.Enter) || gamePadState.IsButtonDown(Buttons.A)) && (gameState == GameState.Menu))
                {
                    gameState = GameState.Prologue;
                    seconds = gameTime.TotalGameTime.Seconds;
                }

                if ((keyboardState.IsKeyDown(Keys.Enter) || gamePadState.IsButtonDown(Buttons.A)) && (gameState == GameState.Prologue) && seconds != gameTime.TotalGameTime.Seconds) 
                        gameState = GameState.Game;

                if (keyboardState.IsKeyDown(Keys.M) || gamePadState.IsButtonDown(Buttons.Y))
                {
                    if (volumeMultiplier == 0)
                        volumeMultiplier = 1;
                    else if (volumeMultiplier == 1)
                        volumeMultiplier = 0;
                }

                if (keyboardState.IsKeyDown(Keys.X) || gamePadState.IsButtonDown(Buttons.RightStick))
                {
                    if (camera.firstPerson == true)
                        camera.firstPerson = false;
                    else if (camera.firstPerson == false)
                        camera.firstPerson = true;
                }
            

            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || (Keyboard.GetState().IsKeyDown(Keys.Escape)))
                this.Exit();

            // TODO: Add your update logic here
            if (gameState == GameState.Game)
            {
                player1.MoveModel(laserList, firingSound, volumeMultiplier);
            }
            // Add velocity to the current position.
            player1.playerPosition += player1.playerVelocity;

            // Bleed off velocity over time.
            player1.playerVelocity *= 0.0f;

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < GameConstants.NumDaleks; i++)
            {
                dalekList[i].Update(timeDelta);
            }

            for (int i = 0; i < GameConstants.NumLasers; i++)
            {
                if (laserList[i].isActive)
                {
                    laserList[i].Update(timeDelta);
                }
            }

            BoundingSphere TardisSphere =
              new BoundingSphere(player1.playerPosition,
                       mdlPlayer.Meshes[0].BoundingSphere.Radius *
                             GameConstants.ShipBoundingSphereScale);

            //Check for collisions
            for (int i = 0; i < dalekList.Length; i++)
            {
                if (dalekList[i].isActive)
                {
                    BoundingSphere dalekSphereA =
                      new BoundingSphere(dalekList[i].position, mdlDalek.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.DalekBoundingSphereScale);

                    for (int k = 0; k < laserList.Length; k++)
                    {
                        if (laserList[k].isActive)
                        {
                            BoundingSphere laserSphere = new BoundingSphere(
                              laserList[k].position, mdlLaser.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.LaserBoundingSphereScale);
                            if (dalekSphereA.Intersects(laserSphere))
                            {
                                //explosionSound.Play();
                                dalekList[i].isActive = false;
                                laserList[k].isActive = false;
                                hitCount++;
                                break; //no need to check other bullets
                            }
                        }
                        if (dalekSphereA.Intersects(TardisSphere)) //Check collision between Dalek and Tardis
                        {
                            //explosionSound.Play();
                            dalekList[i].direction *= -1.0f;
                            laserList[k].isActive = false;
                            break; //no need to check other bullets
                        }

                    }
                }
            }
            //camera.camUpdate(camera, mdlPosition);
            if (camera.firstPerson == false)
                camera.camUpdate(camera, player1.playerPosition);
            if (camera.firstPerson == true)
                camera.camUpdate(camera, player1.playerPosition, player1.playerRotation);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (gameState == GameState.Menu)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                Texture2D splash = Content.Load<Texture2D>(".\\Textures\\splash");
                int screenWidth = this.graphics.PreferredBackBufferWidth;
                int screenHeight = this.graphics.PreferredBackBufferHeight;
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(splash, screenRectangle, Color.White);
                spriteBatch.End();
            }

            if (gameState == GameState.Prologue)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                Texture2D splash2 = Content.Load<Texture2D>(".\\Textures\\splash2");
                int screenWidth = this.graphics.PreferredBackBufferWidth;
                int screenHeight = this.graphics.PreferredBackBufferHeight;
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(splash2, screenRectangle, Color.White);
                spriteBatch.End();
            }

            if (gameState == GameState.Game)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                // TODO: Add your drawing code here
                for (int i = 0; i < GameConstants.NumDaleks; i++)
                {
                    if (dalekList[i].isActive)
                    {
                        Matrix dalekTransform = Matrix.CreateScale(GameConstants.DalekScalar) * Matrix.CreateTranslation(dalekList[i].position);
                        DrawModel(mdlDalek, dalekTransform, mdDalekTransforms);
                    }
                }
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (laserList[i].isActive)
                    {
                        Matrix laserTransform = Matrix.CreateScale(GameConstants.LaserScalar) * Matrix.CreateTranslation(laserList[i].position);
                        DrawModel(mdlLaser, laserTransform, mdlLaserTransforms);
                    }
                }

                groundPosition = new Vector3(0 - GameConstants.PlayfieldSizeX, 0, 0 - GameConstants.PlayfieldSizeZ);

                for (float i = groundPosition.X; i < GameConstants.PlayfieldSizeX * 2; i = +ground.Width)
                {
                    Vector2 ii;
                    ii = new Vector2(i, i);
                    spriteBatch.Draw(ground, ii, Color.White);
                }

                Matrix modelTransform = Matrix.CreateRotationY(player1.playerRotation) * Matrix.CreateTranslation(player1.playerPosition);
                DrawModel(mdlPlayer, modelTransform, mdlPlayerTransforms);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
    }
}
