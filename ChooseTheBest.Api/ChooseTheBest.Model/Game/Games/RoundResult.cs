namespace ChooseTheBest.Model.Game.Game
{
	public class RoundResult
	{
		public string[] ExcludedTitles { get; set; }

		public InnerRoundResult[] RoundResults { get; set; }
		public int TitlesChose { get; set; }
		public int TitlesTotal { get; set; }
	}
}