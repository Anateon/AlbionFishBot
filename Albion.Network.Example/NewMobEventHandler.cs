using System;
using System.Threading.Tasks;

namespace Albion.Network.Example
{
    public class NewMobEventHandler : EventPacketHandler<NewMobEvent>
    {
        public NewMobEventHandler() : base(EventCodes.NewMob) { }

        protected override Task OnActionAsync(NewMobEvent value)
        {
            //Console.WriteLine("ID: " + value.Id);
            Console.WriteLine("ID: " + value.Id + " \\pos: " + value.Position[0] +";"+ value.Position[1]);
            return Task.CompletedTask;
        }
    }
}
