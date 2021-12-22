using System;
using ChooseTheBest.Model.Game.Package;

namespace ChooseTheBest.Model.Game.Game
{
	public class ReadyToGameState
	{
		public PackageInfo PackageInfo { get; set; }
		public GameSettings GameSettings { get; set; }
		public string[] ReadyPlayers { get; set; }
		public DateTimeOffset? StartTime { get; set; }
		
	}
}