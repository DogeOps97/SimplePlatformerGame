using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;

namespace needmoretest

{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Vector2 baseScreenSize = new Vector2(800, 600);
        private Matrix globalTransformation;
        int backbufferWidth, backbufferHeight;


        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;


        // store for keyboard input
        private KeyboardState keyboardState;

        // The number of levels in the Levels directory of our content
        private const int numberOfLevels = 1;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here     
            ScalePresentationArea();
            LoadNextLevel();
        }

        public void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
            System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }


        // Allows the game to run logic such as updating the world,
        // checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
 

            // TODO: Add your update logic here
            //Confirm the screen has not been resized by the user
            if (backbufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight ||
                backbufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
            {
                ScalePresentationArea();
            }
            // Handle polling for our input and handling high-level input
            //HandleInput(gameTime);

            keyboardState = Keyboard.GetState();

            // update our level, passing down the GameTime along with all of our input states
            level.Update(gameTime, keyboardState);
            base.Update(gameTime);
        }
      
        private void LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex);
        }



        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);

            level.Draw(gameTime, _spriteBatch);  
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
