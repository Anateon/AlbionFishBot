using System.Collections.Generic;
using System.IO;
using Albion.Network;

namespace AlbionFishBot
{
    public class LeaveEvent : BaseEvent
    {
        public LeaveEvent(Dictionary<byte, object> parameters) : base(parameters)
        {
            Id = int.Parse(parameters[0].ToString());
        }
        public int Id { get; }

    }
}
