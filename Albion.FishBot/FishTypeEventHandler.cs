using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Albion.Network;

namespace Albion.FishBot
{
    public class FishTypeEventHandler : EventPacketHandler<FishTypeEvent>
    {
        public FishTypeEventHandler() : base(333) { }

        protected override Task OnActionAsync(FishTypeEvent value)
        {
            if (value.Info[0] == 0.75 &&
                value.Info[1] == 1 &&
                value.Info[2] == 60 &&
                value.Info[3] == 1 &&
                value.Info[5] == 3.5 &&
                value.Info[7] == 1 &&
                value.Info[8] == 2 &&
                value.Info[9] == 1.5 &&
                value.Info[10] == 12 &&
                value.Info[12] == 2)
            {
                //Debug.WriteLine("Камень");
                Program.Trash = true;
            }

            return Task.CompletedTask;
        }
    }
}