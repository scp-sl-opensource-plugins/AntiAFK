using Exiled.API.Interfaces;

namespace AntiAFK
{
    public class Translation : ITranslation
    {
        public string AfkMessage { get; internal set; } = "<b>Don`t AFK | [AFK_TIME]/[MAX_AFK_TIME]</b>";
        public string MessageReplacedPlayer { get; internal set; } = "<b>You are AFK-Replaced!</b>";
    }
}
