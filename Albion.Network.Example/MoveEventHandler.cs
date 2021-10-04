using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albion.Network.Example
{
    public class MoveEventHandler : EventPacketHandler<MoveEvent>
    {
        public Dictionary<string, ChelInfo> cheliki;

        public MoveEventHandler(Dictionary<string, ChelInfo> a) : base(EventCodes.Move)
        {
            cheliki = a;
        }

        protected override Task OnActionAsync(MoveEvent value)
        {
            if (cheliki.ContainsKey(value.Id))
            {
                var tmp = cheliki[value.Id];
                tmp.time = DateTime.Now;
                tmp.X = value.Position[0];
                tmp.Y = value.Position[1];
                cheliki[value.Id] = tmp;
            }

            //Console.WriteLine($"Id: {value.Id} x: {value.Position[0]} y: {value.Position[1]}");
            return Task.CompletedTask;
        }
    }
}
