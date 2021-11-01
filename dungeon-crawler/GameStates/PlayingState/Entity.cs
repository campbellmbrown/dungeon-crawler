using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Entity
    {
        private enum DestinationState
        {
            AtDestination,
            OffDestination,
        }

        private const float MOVEMENT_SPEED = 48f; // 16 pixels/second.
        private const float DESTINATION_HYSTERESIS = 0.5f; // How many pixels away for the destination to be considered reached.
        private const float MAX_QUEUED_DESTINATIONS = 3;

        private DestinationState destinationState;
        private Queue<Vector2> _destinations;
        public Vector2 position { get; private set; }
        private GridSquare _gridSquare;
        public GridSquare gridSquare
        {
            get
            {
                return _gridSquare;
            }
            set
            {
                if (_destinations.Count < 3)
                {
                    _gridSquare = value;
                    _destinations.Enqueue(_gridSquare.position);
                    destinationState = DestinationState.OffDestination;
                }
            }
        }

        public Entity()
        {
            _destinations = new Queue<Vector2>();
            destinationState = DestinationState.OffDestination;
        }

        public void ChangeGridSquare(GridSquare gridSquare)
        {
            this.gridSquare = gridSquare;
        }

        public virtual void FrameTick(GameTime gameTime)
        {
            if (destinationState == DestinationState.OffDestination)
            {
                Vector2 currentDestination = _destinations.Peek();
                position += Vector2.Normalize(currentDestination - position) * MOVEMENT_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if ((currentDestination - position).Length() < DESTINATION_HYSTERESIS)
                {
                    position = currentDestination;
                    _destinations.Dequeue();
                    if (_destinations.Count == 0)
                    {
                        destinationState = DestinationState.AtDestination;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(new RectangleF(position.X, position.Y, 4, 4), Color.Yellow);
        }

        public virtual void ActionTick()
        {

        }
    }
}
