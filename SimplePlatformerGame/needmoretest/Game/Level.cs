#region File Description
//-----------------------------------------------------------------------------
// Level.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace needmoretest
{
    /// <summary>
    /// A uniform grid of tiles with collections of gems and enemies.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    class Level : IDisposable
    {
        // Physical structure of the level.
        private Tile[,] tiles;

        // Entities in the level.
        public Player Player
        {
            get { return player; }
        }
        Player player;


        // Key locations in the level.        
        private Vector2 start;


        // Level game state.
        private Random random = new Random(354668); // Arbitrary, but constant seed

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;



        #region Loading
        /// Constructs a new level.

        /// The service provider that will be used to construct a ContentManager.
  
        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "Content");

            LoadTiles(fileStream);
        }


        // Iterates over every tile in the structure file and loads its
        // appearance and behavior. Also checks start points.
        private void LoadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            

        }


        // Load the tile's appearance and behavior
   
        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Player 1 start point
                case '1':
                    return LoadStartTile(x, y);

                // Impassable block
                case '#':
                    //return LoadVarietyTile("dirt", 0, TileCollision.Impassable);
                    return LoadTile("dirt0", TileCollision.Impassable);



                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        // Creates a new tile. 
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }


       
        // Loads a tile with a random appearance.

        // The content name prefix for this group of tile variations. Tile groups are
        // name LikeThis0.png and LikeThis1.png and LikeThis2.png.
        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }

        // Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, start);

            return new Tile(null, TileCollision.Passable);
        }




        // Unloads the level content.
 
        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        #region Bounds and collision

        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            // though no respawns
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            // else just return the collision status 
            return tiles[x, y].Collision;
        }
 
        // Gets the bounding rectangle of a tile in world space.
      
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        // Width of level measured in tiles
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        //
        // Updates all objects in the world, performs collision between them,
        // and handles the time limit with scoring.
        // 
        public void Update(
            GameTime gameTime, 
            KeyboardState keyboardState 
            )
        {
            // Pause while the player is dead or time is expired.
            if (!Player.IsAlive)
            {
                // Still want to perform physics on the player.
                Player.ApplyPhysics(gameTime);
            }   
            else
            {    
                Player.Update(gameTime, keyboardState);

                // check if player fell down out of the bounds
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    StartNewLife();
            }
        }


        // Restores the player to the starting point to try the level again.
        public void StartNewLife()
        {
            Player.Reset(start);
        }

        #endregion

        #region Draw
        /// Draw tiles and characters
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            

            DrawTiles(spriteBatch);

            Player.Draw(gameTime, spriteBatch);

            
        }

        // Draws each tile in the level.
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }

        #endregion
    }
}