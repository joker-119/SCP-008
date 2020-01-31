using EXILED;

namespace SCP008
{
	public class Commands
	{
		private readonly Plugin plugin;
		public Commands(Plugin plugin) => this.plugin = plugin;

		public void OnRaCommand(ref RACommandEvent ev)
		{
			string[] args = ev.Command.Split(' ');

			switch (args[0].ToLower())
			{
				case "infect":
				{
					ev.Allow = false;
					if (args.Length < 2)
					{
						ev.Sender.RAMessage("You must supply a player named.", false);
						return;
					}

					ReferenceHub player = Plugin.GetPlayer(args[1]);
					if (player == null)
					{
						ev.Sender.RAMessage($"Player not found: {args[1]}", false);
						return;
					}

					if (plugin.InfectedPlayers.Contains(player))
					{
						ev.Sender.RAMessage($"Player: {args[1]} is already infected.");
						return;
					}
					
					plugin.Functions.InfectPlayer(player);
					ev.Sender.RAMessage($"{args[1]} has been infected with SCP-008.");
					return;
				}
				case "cure":
				{
					ev.Allow = false;
					if (args.Length < 2)
					{
						ev.Sender.RAMessage("You must supply a player named.", false);
						return;
					}

					ReferenceHub player = Plugin.GetPlayer(args[1]);
					if (player == null)
					{
						ev.Sender.RAMessage($"Player not found: {args[1]}", false);
						return;
					}

					if (!plugin.InfectedPlayers.Contains(player))
					{
						ev.Sender.RAMessage($"Player: {args[1]} is not infected.");
						return;
					}
					
					plugin.Functions.CurePlayer(player);
					ev.Sender.RAMessage($"{args[1]} has been cured of SCP-008.");
					return;
				}
			}
		}
	}
}