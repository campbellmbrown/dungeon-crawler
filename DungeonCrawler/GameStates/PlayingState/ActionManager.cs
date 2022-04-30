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
        const float SecondsPerAction = 0.25f;
        float DecimalComplete { get; }
        ActionState ActionState { get; }
        void Start();
        void Stop();
    }

    public enum ActionState
    {
        Stopped,
        Stopping,
        Starting,
        InProgress,
        Restarting,
    }

    public class ActionManager : IActionManager
    {
        readonly ILogManager _logManager;

        public float DecimalComplete => _timePassed / IActionManager.SecondsPerAction;
        public ActionState ActionState { get; private set; } = ActionState.Stopped;

        float _timePassed = 0f;

        public ActionManager(ILogManager logManager)
        {
            _logManager = logManager;
        }

        public void Start()
        {
            _timePassed = 0;
            _logManager.Log($"Transition from {ActionState} to {ActionState.Starting}");
            ActionState = ActionState.Starting;
        }

        public void Stop()
        {
            _logManager.Log($"Transition from {ActionState} to {ActionState.Stopping}");
            ActionState = ActionState.Stopping;
        }

        public void FrameTick(IGameTimeWrapper gameTime)
        {
            switch (ActionState)
            {
                case ActionState.Stopped:
                    // Do nothing
                    break;
                case ActionState.Stopping:
                    _logManager.Log($"Transition from {ActionState} to {ActionState.Stopped}");
                    ActionState = ActionState.Stopped;
                    break;
                case ActionState.Starting:
                    _logManager.Log($"Transition from {ActionState} to {ActionState.InProgress}");
                    ActionState = ActionState.InProgress;
                    break;
                case ActionState.InProgress:
                    Progress(gameTime);
                    break;
                case ActionState.Restarting:
                    // We haven't got a stop signal so start the next action
                    _timePassed = 0;
                    _logManager.Log($"Transition from {ActionState} to {ActionState.InProgress}");
                    ActionState = ActionState.InProgress;
                    break;
            }
        }

        void Progress(IGameTimeWrapper gameTime)
        {
            _timePassed += gameTime.TimeDiffSec;
            if (_timePassed >= IActionManager.SecondsPerAction)
            {
                _timePassed = IActionManager.SecondsPerAction;
                _logManager.Log($"Transition from {ActionState} to {ActionState.Restarting}");
                ActionState = ActionState.Restarting;
            }
        }
    }
}
