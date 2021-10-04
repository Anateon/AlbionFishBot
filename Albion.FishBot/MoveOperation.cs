using System.Collections.Generic;
using Albion.Network;

namespace Albion.FishBot
{
    public class MoveOperation : BaseOperation
    {
        public MoveOperation(Dictionary<byte, object> parameters) : base(parameters)
        {
            Position = (float[])parameters[1];
            NewPosition = (float[])parameters[3];
        }

        public float[] Position { get; }
        //public float Direction { get; }
        public float[] NewPosition { get; }
        //public float Speed { get; }
    }
}
