using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;

namespace Albion.Network.Example
{
    public class TESTEventHandler : EventPacketHandler<TESTEvent>
    {
        public Dictionary<string, ChelInfo> cheliki;

        public TESTEventHandler() : base(-666)
        {

        }

        protected override Task OnActionAsync(TESTEvent value)
        {
            //Console.WriteLine("ID: " + value.Id + " \\pos: " + value.Position[0] + ";" + value.Position[1]);
            return Task.CompletedTask;
        }
    }
}
