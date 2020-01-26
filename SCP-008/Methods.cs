using System.Collections.Generic;
using EXILED;
using MEC;
using UnityEngine;

namespace SCP008
{
	public class Methods
	{
		private readonly Plugin plugin;
		public Methods(Plugin plugin) => this.plugin = plugin;

		public void InfectPlayer(ReferenceHub player)
		{
			if (plugin.InfectedPlayers.Contains(player))
				return;
			plugin.InfectedPlayers.Add(player);

			plugin.Coroutines.Add(Timing.RunCoroutine(DoInfectionTimer(player), $"{player.characterClassManager.UserId}"));
		}

		private IEnumerator<float> DoInfectionTimer(ReferenceHub player)
		{
			Broadcast broadcast = player.gameObject.GetComponent<Broadcast>();
			for (int i = 0; i < plugin.InfectionLength; i++)
			{
				if (!plugin.InfectedPlayers.Contains(player))
					yield break;

				broadcast.CallRpcClearElements();
				broadcast.CallRpcAddElement($"You are infected with SCP-008. The infection will take over in {plugin.InfectionLength - 1} seconds!", 1, false);
				yield return Timing.WaitForSeconds(1f);
			}

			player.characterClassManager.NetworkCurClass = RoleType.Scp0492;
			foreach (ReferenceHub hub in EXILED.Plugin.GetHubs())
				if (Vector3.Distance(hub.gameObject.transform.position, player.gameObject.transform.position) < 10f && hub.characterClassManager.IsHuman())
					InfectPlayer(hub);
		}

		public void CurePlayer(ReferenceHub player)
		{
			if (!plugin.InfectedPlayers.Contains(player))
				return;
			plugin.InfectedPlayers.Remove(player);

			Timing.KillCoroutines($"{player.characterClassManager.UserId}");
		}
	}
}