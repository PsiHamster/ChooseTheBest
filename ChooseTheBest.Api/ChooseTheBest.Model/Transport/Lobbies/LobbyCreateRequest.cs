using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChooseTheBest.Model.Game.Lobbies;

namespace ChooseTheBest.Model.Transport.Lobbies
{
	public class LobbyCreateRequest
	{
		public string LobbyName { get; set; }
		public string? LobbyPassword { get; set; }
		public int MaxPlayers { get; set; }
		public string GameModeType { get; set; }
	}

	public class LobbyCreateResponse
	{
		public Lobby Lobby { get; set; }
	}
}
