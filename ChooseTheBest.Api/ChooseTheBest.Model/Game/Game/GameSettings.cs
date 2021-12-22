namespace ChooseTheBest.Model.Game.Game
{
	public class GameSettings
	{
		public string GameModeType { get; set; }
		public int VoteTime { get; set; }
		public int WinnersCount { get; set; }
		public bool AllowOneForAllAbility { get; set; }
		public bool HidePlayersVotes { get; set; }
	}
}