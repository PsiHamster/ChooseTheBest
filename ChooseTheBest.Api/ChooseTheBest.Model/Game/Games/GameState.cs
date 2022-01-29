using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChooseTheBest.Model.Game.Package;

namespace ChooseTheBest.Model.Game.Game
{
	public class GameState
	{
		public PackageInfo PackageInfo { get; set; }
		public GameSettings GameSettings { get; set; }
		public RoundResult[] RoundsResults { get; set; }
		public RoundState RoundState { get; set; }

		public int CurrentRound { get; set; }
		public int TotalRounds { get; set; }
	}
}
