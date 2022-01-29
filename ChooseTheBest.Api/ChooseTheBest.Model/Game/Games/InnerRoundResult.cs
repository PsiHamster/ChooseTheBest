using System.Collections.Generic;

namespace ChooseTheBest.Model.Game.Game
{
	public class InnerRoundResult
	{
		public string TitleWinner { get; set; }
		public string TitleLooser { get; set; }
		public Dictionary<string, string> PlayerVotes { get; set; }
		public bool IsRandom { get; set; }
		public bool WasOneForAllUsed { get; set; }
	}
}