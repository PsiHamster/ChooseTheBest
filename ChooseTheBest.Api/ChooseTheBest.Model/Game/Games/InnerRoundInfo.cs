using System;

namespace ChooseTheBest.Model.Game.Game
{
	public class InnerRoundInfo
	{
		public string GameModeType { get; set; }
		public DateTimeOffset? RoundEndTime { get; set; }
		public string[] VotedPlayers { get; set; }

		public DefaultInnerRoundInfo DefaultInnerRoundInfo { get; set; }
	}
}