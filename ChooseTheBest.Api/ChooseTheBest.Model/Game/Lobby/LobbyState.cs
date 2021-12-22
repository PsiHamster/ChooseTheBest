using ChooseTheBest.Model.Game.Game;
using ChooseTheBest.Model.Game.Players;

namespace ChooseTheBest.Model.Game.Lobby
{
	public class LobbyState
	{
		public LobbyInfo LobbyInfo { get; set; }
		public string GameStageType { get; set; }
		public PlayerInfo[] Players { get; set; }
		public string[] DisconnectedPlayers { get; set; }
		
		public PackageCreationState PackageCreationState { get; set; }
		public ReadyToGameState ReadyToGameState { get; set; }
		public GameState GameState { get; set; }
		public ResultsState ResultsState { get; set; }
	}
}