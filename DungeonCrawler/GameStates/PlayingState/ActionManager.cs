using System.Timers;

namespace DungeonCrawler.GameStates.PlayingState
{
    // Click happens
    // Player generates queue
    // Entities generate queue
    // Action manager starts
    // Entity tick progresses
    // Player moves towards destination
    // Entity moves towards destination
    // Entity tick progresses
    // Player moves towards destination
    // Entity moves towards destination
    // Entity tick is at 100%
    // Player locks to tile
    // If player has no more tiles, clear entity tiles
    // and stop the action manager
    // Entity locks to tile

    public interface IActionManager : IFrameTickable
    {
        const float SecondsPerAction = 0.2f;
        float DecimalComplete { get; }
        ActionState ActionState { get; }
        void Start();
        void Stop();
    }

    public enum ActionState
    {
        NotActive,
        InProgress,
        Finished,
    }

    public class ActionManager : IActionManager
    {
        public float DecimalComplete => _timePassed / IActionManager.SecondsPerAction;
        public ActionState ActionState { get; private set; } = ActionState.NotActive;

        float _timePassed = 0f;

        public ActionManager()
        {
        }

        public void Start()
        {
            _timePassed = 0;
            ActionState = ActionState.InProgress;
        }

        public void Stop()
        {
            ActionState = ActionState.NotActive;
        }

        public void FrameTick(IGameTimeWrapper gameTime)
        {
            switch (ActionState)
            {
                case ActionState.NotActive:
                    // Do nothing
                    break;
                case ActionState.InProgress:
                    Progress(gameTime);
                    break;
                case ActionState.Finished:
                    // We haven't got a stop signal so start the next action
                    Start();
                    break;
            }
        }

        void Progress(IGameTimeWrapper gameTime)
        {
            _timePassed += gameTime.TimeDiffSec;
            if (_timePassed >= IActionManager.SecondsPerAction)
            {
                _timePassed = IActionManager.SecondsPerAction;
                ActionState = ActionState.Finished;
            }
        }
    }
}
