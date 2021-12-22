namespace ChooseTheBest.Model.Game.Game
{
	public class RoundState
	{
		public string[] Titles { get; set; }
		public InnerRoundResult[] RoundResults { get; set; }
		public InnerRoundInfo RoundInfo { get; set; }

		public int TitlesChose { get; set; }
		public int TitlesTotal { get; set; }
	}
}