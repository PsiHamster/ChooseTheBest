using ChooseTheBest.Model.Game.Players;

namespace ChooseTheBest.Model.Game.Lobby
{
	public class Lobby
	{
		public LobbyInfo LobbyInfo { get; set; }
		public PlayerInfo[] Players { get; set; }
	}
}
