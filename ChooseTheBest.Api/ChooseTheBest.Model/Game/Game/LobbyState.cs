using ChooseTheBest.Model.Game.Lobby;
using ChooseTheBest.Model.Game.Players;

namespace ChooseTheBest.Model.Game.Game
{
	public class LobbyState
	{
		public LobbyInfo LobbyInfo { get; set; }
		public string GameStageType { get; set; }
		public PlayerInfo[] Players { get; set; }
		public string[] DisconnectedPlayers { get; set; }

		public PackageCreationState PackageCreationState { get; set; }
	}
}