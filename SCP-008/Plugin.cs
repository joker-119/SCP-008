using System;
using System.Collections.Generic;
using EXILED;
using MEC;

namespace SCP008
{
	public class Plugin : EXILED.Plugin
	{
		public Methods Functions { get; private set; }
		public EventHandlers EventHandlers { get; private set; }
		public Commands Commands { get; private set; }

		public bool Enabled;
		public float InfectionLength;
		public float InfectionChance;
		public bool TurnInfectedOnDeath;
		
		public HashSet<ReferenceHub> InfectedPlayers = new HashSet<ReferenceHub>();
		public List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
		public Random Gen = new Random();

		public override void OnEnable()
		{
			ReloadConfig();
			if (!Enabled)
				return;

			EventHandlers = new EventHandlers(this);
			Functions = new Methods(this);
			Commands = new Commands(this);

			Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
			Events.RoundEndEvent += EventHandlers.OnRoundEnd;
			Events.PlayerHurtEvent += EventHandlers.OnPlayerHurt;
			Events.UseMedicalItemEvent += EventHandlers.OnUseMedicalItem;
			Events.RemoteAdminCommandEvent += Commands.OnRaCommand;
		}

		public override void OnDisable()
		{
			Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
			Events.PlayerHurtEvent -= EventHandlers.OnPlayerHurt;
			Events.UseMedicalItemEvent -= EventHandlers.OnUseMedicalItem;
			Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
			Events.RemoteAdminCommandEvent -= Commands.OnRaCommand;

			EventHandlers = null;
			Functions = null;
			Commands = null;
		}

		public override void OnReload()
		{
		}

		public override string getName { get; } = "SCP_008";

		public void ReloadConfig()
		{
			Enabled = Config.GetBool("008_enabled", true);
			InfectionChance = Config.GetFloat("008_infection_chance", 40f);
			InfectionLength = Config.GetFloat("008_infection_length", 30f);
			TurnInfectedOnDeath = Config.GetBool("008_turn_on_death", true);
		}
	}
}