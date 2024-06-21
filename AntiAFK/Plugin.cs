using AntiAFK.EventHandlers;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using PlayerRoles;
using System.Collections.Generic;

namespace AntiAFK
{
    public enum ActionType : int
    {
        ForceclassToSpectator = 0,
        Kick = 1,
        Kill = 2,
        Disconnect = 3,
        Explode = 4,
    }
    public class Plugin : Plugin<Config, Translation>
    {
        public static Plugin plugin;
        public override string Prefix => "AntiAFK";
        public override string Name => "AntiAFK";
        public override string Author => "[OPENSOURCE PLUGIN] [https://github.com/scp-sl-opensource-plugins]";
        public static Dictionary<Player, uint> AfkTime { get; internal set; } = new Dictionary<Player, uint>();
        public override void OnEnabled()
        {
            plugin = this;
            Exiled.Events.Handlers.Server.RoundStarted += ServerHandlers.OnRoundStart;
            base.OnEnabled();
        }

        public static IEnumerator<float> ChangeAfkPlayer(Player oldPlayer, Player newPlayer)
        {
            Item currentItem;

            newPlayer.Role.Set(RoleTypeId.Tutorial, SpawnReason.Revived, RoleSpawnFlags.All);
            newPlayer.Role.Set(oldPlayer.Role.Type, SpawnReason.Revived, RoleSpawnFlags.None);

            yield return Timing.WaitForSeconds(1);

            newPlayer.Broadcast(10, plugin.Translation.MessageReplacedPlayer, Broadcast.BroadcastFlags.Normal, true);

            currentItem = oldPlayer.CurrentItem;

            newPlayer.Teleport(oldPlayer);
            newPlayer.Health = oldPlayer.Health;
            newPlayer.MaxHealth = oldPlayer.MaxHealth;
            newPlayer.HumeShield = oldPlayer.HumeShield;
            newPlayer.ArtificialHealth = oldPlayer.ArtificialHealth;
            newPlayer.Rotation = oldPlayer.Rotation;
            newPlayer.Stamina = oldPlayer.Stamina;
            newPlayer.RelativePosition = oldPlayer.RelativePosition;
            newPlayer.MaxArtificialHealth = oldPlayer.MaxArtificialHealth;
            newPlayer.IsSpawnProtected = oldPlayer.IsSpawnProtected;
            if (oldPlayer.Cuffer != null) newPlayer.Cuffer = oldPlayer.Cuffer;
            foreach (var effect in oldPlayer.ActiveEffects) newPlayer.EnableEffect(effect.GetEffectType(), effect.Duration, false);
            foreach (Item item in oldPlayer.Items) item.ChangeItemOwner(oldPlayer, newPlayer);
            if (currentItem != null) newPlayer.CurrentItem = currentItem;

            switch (plugin.Config.ActionWithAfkPlayers)
            {
                case ActionType.ForceclassToSpectator:
                    oldPlayer.Role.Set(RoleTypeId.Spectator);
                    oldPlayer.Broadcast(5, "<b>AFK (forceclassed by plugin)</b>", Broadcast.BroadcastFlags.Normal, true);
                    break;
                case ActionType.Kick: oldPlayer.Kick("AFK (kicked by plugin)"); break;
                case ActionType.Kill: oldPlayer.Kill("AFK (killed by plugin)"); break;
                case ActionType.Disconnect: oldPlayer.Disconnect("AFK (disconnected by plugin)"); break;
                case ActionType.Explode:
                    oldPlayer.ExplodeEffect(ProjectileType.FragGrenade);
                    oldPlayer.Kill("AFK (exploded by plugin)");
                    break;
            }
        }

        public override void OnDisabled()
        {
            plugin = null;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerHandlers.OnRoundStart;
            base.OnDisabled();
        }
    }
}
