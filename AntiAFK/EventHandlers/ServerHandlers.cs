using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AntiAFK.EventHandlers
{
    internal class ServerHandlers
    {
        internal static IEnumerator<float> OnRoundStart()
        {
            Plugin.AfkTime.Clear();
            Dictionary<Player, (Vector3, Quaternion)> players = new Dictionary<Player, (Vector3, Quaternion)>();
            while (!Round.IsEnded)
            {
                foreach (Player player in Player.List.Where(x => x != null))
                {
                    if (Plugin.plugin.Config.AfkIgnoreRoles.Contains(player.Role.Type))
                    {
                        players.Remove(player);
                        Plugin.AfkTime.Remove(player);
                        continue;
                    }

                    if (players.ContainsKey(player))
                    {
                        if (players[player] == (player.Position, player.Rotation))
                        {
                            Log.Debug($"{player.Nickname} | {Plugin.AfkTime[player]}");
                            if (Plugin.AfkTime[player] >= Plugin.plugin.Config.MessageAfkTime)
                            {
                                if (Plugin.plugin.Config.PlayMessageSound) player.PlayGunSound(ItemType.ParticleDisruptor, 33);
                                player.Broadcast(2, Plugin.plugin.Translation.AfkMessage.Replace("[AFK_TIME]", Plugin.AfkTime[player].ToString()).Replace("[MAX_AFK_TIME]", Plugin.plugin.Config.MaxAfkTime.ToString()), Broadcast.BroadcastFlags.Normal, true);
                            }

                            if (Plugin.AfkTime[player] < Plugin.plugin.Config.MaxAfkTime)
                            {
                                Plugin.AfkTime[player]++;
                            }
                            else
                            {
                                if (Player.List.Any(x => x.Role.Type == RoleTypeId.Spectator) && Plugin.plugin.Config.PlayerReplacerIsEnabled)
                                {
                                    Timing.RunCoroutine(Plugin.ChangeAfkPlayer(player, Player.List.Where(x => x.Role.Type == RoleTypeId.Spectator).ToList().RandomItem()));
                                }
                                else
                                {
                                    switch (Plugin.plugin.Config.ActionWithAfkPlayers)
                                    {
                                        case ActionType.ForceclassToSpectator:
                                            player.Role.Set(RoleTypeId.Spectator);
                                            player.Broadcast(5, "<b>AFK (forceclassed by plugin)</b>", Broadcast.BroadcastFlags.Normal, true);
                                            break;
                                        case ActionType.Kick: player.Kick("AFK"); break;
                                        case ActionType.Kill: player.Kill("AFK"); break;
                                        case ActionType.Disconnect: player.Disconnect("AFK (disconnected by plugin)"); break;
                                        case ActionType.Explode:
                                            player.ExplodeEffect(ProjectileType.FragGrenade);
                                            player.Kill("AFK (exploded by plugin)");
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            players[player] = (player.Position, player.Rotation);
                            Plugin.AfkTime[player] = 0;
                        }
                    }
                    else
                    {
                        players.Add(player, (player.Position, player.Rotation));
                        Plugin.AfkTime.Add(player, 0);
                    }
                }
                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}
