using ChooseTheBest.Model.Game.Players;

namespace ChooseTheBest.Model.Game.Lobbies
{
	public class Lobby
	{
		public LobbyInfo LobbyInfo { get; set; }
		public LobbySettings LobbySettings { get; set; }
		public PlayerInfo[] Players { get; set; }
		public string ConnectionString { get; set; }
	}
}
