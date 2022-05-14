namespace DungeonCrawler.GameStates.PlayingState
{
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
            _timePassed = IActionManager.SecondsPerAction;
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
