using System;
using System.Collections.Generic;
using Albion.Network;

namespace Albion.FishBot
{
    public class FishingMiniGameEvent : BaseEvent
    {
        public FishingMiniGameEvent(Dictionary<byte, object> parameters) : base(parameters)
        {
            Id = int.Parse(parameters[0].ToString());
            Position = (float[])parameters[1];
            Status = byte.Parse(parameters[4].ToString());
            ownerID = int.Parse(parameters[3].ToString());
        }
        public int Id { get; }
        public float[] Position { get; }
        public byte Status { get; }
        public int ownerID { get; }
    }
}