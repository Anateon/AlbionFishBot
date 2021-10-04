using System;
using System.Collections.Generic;

namespace Albion.Network.Example
{
    public class NewMobEvent : BaseEvent
    {
        public NewMobEvent(Dictionary<byte, object> parameters) : base(parameters)
        {
            Id = $"1) {(parameters.TryGetValue(0, out object q) ? q.ToString() : null)}";
            Position = (float[])parameters[7];
        }

        public string Id { get; }
        public float[] Position { get; }
    }
}
