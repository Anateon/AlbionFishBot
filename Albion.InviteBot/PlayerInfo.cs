namespace Albion.InviteBot
{
    public class PlayerInfo
    {
        public string Name;
        public string GuildName;
        public bool isInvited = false;
        public override string ToString()
        {
            return $"isInvited:'{isInvited}'\tName: '{Name}'\tGuild: '{GuildName}'";
        }
    }
}