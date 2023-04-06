#region File Description
//-----------------------------------------------------------------------------
// Animation.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework.Graphics;

namespace needmoretest
{

    // All frames are required to be square
    class Animation
    {
        // All frames in the animation arranged horizontally.
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;


        // Duration of time for each frame.
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        // Loop or not?
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        // Gets the number of frames in the animation.
        public int FrameCount
        {
            // Has to be square frames
            get { return Texture.Width / FrameHeight; }
        }
        // Gets the width of a frame in the animation.
        public int FrameWidth
        {
            // Assume square frames.
            get { return Texture.Height; }
        }

    
        // Gets the height of a frame in the animation.

        public int FrameHeight
        {
            get { return Texture.Height; }
        }
        /// Constructors a new animation. 
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }
    }
}