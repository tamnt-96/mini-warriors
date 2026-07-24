namespace TWR.Core
{
    public class GameState_Init : IState
    {
        public void OnEnter()
        {
            EventBus<ChangeStateEvent>.Publish(new ChangeStateEvent { stateType = typeof(GameState_Lobby) });
        }

        public void OnUpdate() { }

        public void OnExit() { }
    }
}
