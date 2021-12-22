using ChooseTheBest.Model.Game.Package;

namespace ChooseTheBest.Model.Game.Game
{
	public class PackageCreationState
	{
		public string PackageName { get; set; }
		public string PackageDescription { get; set; }
		public int TitlesCount { get; set; }
		public int Round { get; set; }
		public string[] VotedPlayers { get; set; }

		public TitleData[] AddedTitles { get; set; }
	}
}