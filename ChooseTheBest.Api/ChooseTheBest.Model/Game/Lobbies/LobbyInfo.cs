namespace ChooseTheBest.Model.Game.Lobbies
{
	public class LobbyInfo
	{
		public string LobbyId { get; set; }
		public string LobbyName { get; set; }
		public string LobbyLeader { get; set; }
		public bool HasPassword { get; set; }
		public int PlayersCount { get; set; }
		public int MaxPlayersCount { get; set; }
		public string GameModeType { get; set; }

		public string LobbyState { get; set; }
	}
}