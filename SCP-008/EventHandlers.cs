using System.Collections.Generic;
using EXILED;
using EXILED.Extensions;
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
			if (ev.Info.Amount >= ev.Player.playerStats.health)
				if (plugin.InfectedPlayers.Contains(ev.Player))
				{
					plugin.Functions.CurePlayer(ev.Player);
					if (ev.Player.characterClassManager.CurClass != RoleType.Scp0492 && plugin.TurnInfectedOnDeath)
					{
						Vector3 pos = ev.Player.gameObject.transform.position;
						Timing.RunCoroutine(plugin.Functions.TurnIntoZombie(ev.Player, new Vector3(pos.x, pos.y, pos.z)));
						ev.Player.inventory.ServerDropAll();
						ev.Info = new PlayerStats.HitInfo(0f, ev.Info.Attacker, ev.Info.GetDamageType(), ev.Info.PlyId);
						return;
					}
				}

			if (ev.Attacker == null || string.IsNullOrEmpty(ev.Attacker.characterClassManager.UserId))
			{
				Log.Debug("Attacker could not be found.");
				return;
			}

			if (ev.Attacker.characterClassManager.CurClass == RoleType.Scp0492 && ev.Player.GetTeam() != Team.SCP)
			{
				int r = plugin.Gen.Next(100);
				Log.Debug($"Roll: {r}. Target: {plugin.InfectionChance}");
				if (r <= plugin.InfectionChance)
				{
					Log.Debug($"Infecting {ev.Player.nicknameSync.MyNick} ({ev.Player.characterClassManager.CurClass}");
					plugin.Functions.InfectPlayer(ev.Player);
				}
			}
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