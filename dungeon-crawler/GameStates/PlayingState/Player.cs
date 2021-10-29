using dungeoncrawler.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace dungeoncrawler.GameStates.PlayingState
{
    public class Player : Entity
    {
        private readonly ViewManager _viewManager;
        private readonly GridManager _gridManager;
        private readonly PlayingState _playingState;

        private readonly InputManager _inputManager;
        private int _desiredXMovement = 0;
        private int _desiredYMovement = 0;

        public Player(ViewManager viewManager, GridManager gridManager, PlayingState playingState) :
            base()
        {
            _viewManager = viewManager;
            _gridManager = gridManager;
            _playingState = playingState;

            _inputManager = new InputManager();
            _inputManager.AddSingleShotInput(Keys.W, MoveUp);
            _inputManager.AddSingleShotInput(Keys.S, MoveDown);
            _inputManager.AddSingleShotInput(Keys.A, MoveLeft);
            _inputManager.AddSingleShotInput(Keys.D, MoveRight);
        }

        public override void FrameTick(GameTime gameTime)
        {
            _inputManager.FrameTick();
            _viewManager.UpdateCameraPosition(gridSquare.position);
            base.FrameTick(gameTime);
        }

        public void MoveUp()
        {
            _desiredYMovement -= 1;
            MovementCommon();
        }

        public void MoveDown()
        {
            _desiredYMovement += 1;
            MovementCommon();
        }

        public void MoveLeft()
        {
            _desiredXMovement -= 1;
            MovementCommon();
        }

        public void MoveRight()
        {
            _desiredXMovement += 1;
            MovementCommon();
        }

        private void MovementCommon()
        {
            _playingState.ActionTick();
        }

        public override void ActionTick()
        {
            _gridManager.MoveEntityByDifference(this, _desiredXMovement, _desiredYMovement);

            _desiredXMovement = 0;
            _desiredYMovement = 0;
        }
    }
}
