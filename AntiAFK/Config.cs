using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;

namespace AntiAFK
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public uint MaxAfkTime { get; set; } = 180;

        public uint MessageAfkTime { get; set; } = 120;
        public bool PlayMessageSound { get; set; } = true;

        public bool PlayerReplacerIsEnabled { get; set; } = true;

        public ActionType ActionWithAfkPlayers { get; set; } = ActionType.ForceclassToSpectator;

        public HashSet<RoleTypeId> AfkIgnoreRoles { get; set; } = new HashSet<RoleTypeId>()
        {
            RoleTypeId.Spectator,
            RoleTypeId.Scp079,
            RoleTypeId.Tutorial,
            RoleTypeId.Overwatch,
            (RoleTypeId)22
        };
    }
}
