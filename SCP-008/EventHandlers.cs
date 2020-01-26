using System.Collections.Generic;
using EXILED;
using MEC;
using UnityEngine;

namespace SCP008
{
	public class EventHandlers
	{
		private readonly Plugin plugin;
		public EventHandlers(Plugin plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers()
		{
			foreach (CoroutineHandle handle in plugin.Coroutines)
				Timing.KillCoroutines(handle);
		}

		public void OnRoundEnd()
		{
			foreach (CoroutineHandle handle in plugin.Coroutines)
				Timing.KillCoroutines(handle);
		}

		public void OnPlayerHurt(ref PlayerHurtEvent ev)
		{
			if (ev.Attacker == null || string.IsNullOrEmpty(ev.Attacker.characterClassManager.UserId))
				return;
			
			if (ev.Attacker.characterClassManager.CurClass == RoleType.Scp0492)
				if (plugin.Gen.Next(100) < plugin.InfectionChance)
					plugin.Functions.InfectPlayer(ev.Player);
		}

		public void OnPlayerDeath(ref PlayerDeathEvent ev)
		{
			if (plugin.InfectedPlayers.Contains(ev.Player))
			{
				plugin.Functions.CurePlayer(ev.Player);
				Timing.RunCoroutine(TurnIntoZombie(ev.Player, ev.Player.gameObject.transform.position));
			}
		}

		private IEnumerator<float> TurnIntoZombie(ReferenceHub player, Vector3 position)
		{
			yield return Timing.WaitForSeconds(0.3f);
			player.characterClassManager.SetPlayersClass(RoleType.Scp0492, player.gameObject, true);
			yield return Timing.WaitForSeconds(0.3f);
			player.plyMovementSync.OverridePosition(position, player.gameObject.transform.rotation.y);
		}

		public void OnUseMedicalItem(MedicalItemEvent ev)
		{
			if (!plugin.InfectedPlayers.Contains(ev.Player))
				return;
			
			if (ev.Item == ItemType.Medkit || ev.Item == ItemType.SCP500)
				plugin.Functions.CurePlayer(ev.Player);
		}
	}
}