namespace ChooseTheBest.Model.Game.Lobby
{
	public class LobbyInfo
	{
		public string LobbyName { get; set; }
		public string LobbyLeader { get; set; }
		public bool HasPassword { get; set; }
		public int PlayersCount { get; set; }
		public int MaxPlayersCount { get; set; }
		public string GameStageType { get; set; }
	}
}
