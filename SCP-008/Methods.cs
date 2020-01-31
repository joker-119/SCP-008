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
			for (int i = 0; i < plugin.InfectionLength; i++)
			{
				if (!plugin.InfectedPlayers.Contains(player))
					yield break;

				player.gameObject.GetComponent<Broadcast>().RpcClearElements();
				player.Broadcast(1, $"You are infected with SCP-008. The infection will take over in {plugin.InfectionLength - i} seconds!");
				yield return Timing.WaitForSeconds(1f);
			}

			GameObject gameObject = player.gameObject;
			Vector3 pos = gameObject.transform.position;

			Timing.RunCoroutine(TurnIntoZombie(player, pos));

			yield return Timing.WaitForSeconds(0.6f);
			
			foreach (ReferenceHub hub in EXILED.Plugin.GetHubs())
				if (Vector3.Distance(hub.gameObject.transform.position, player.gameObject.transform.position) < 10f && hub.characterClassManager.IsHuman())
					InfectPlayer(hub);
			CurePlayer(player);
		}
		
		public IEnumerator<float> TurnIntoZombie(ReferenceHub player, Vector3 position)
		{
			yield return Timing.WaitForSeconds(0.3f);
			player.characterClassManager.SetClassIDAdv(RoleType.Scp0492, true);
			yield return Timing.WaitForSeconds(1f);
			player.plyMovementSync.OverridePosition(position, player.gameObject.transform.rotation.y);
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