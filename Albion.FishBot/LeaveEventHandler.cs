using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Albion.Network;

namespace Albion.FishBot
{
    public class LeaveEventHandler : EventPacketHandler<LeaveEvent>
    {

        public LeaveEventHandler() : base(1)
        {
        }

        protected override Task OnActionAsync(LeaveEvent value)
        {
            try
            {
                if (Program.Poplovok != null && Program.Poplovok.ID == value.Id)
                {
                    Program.Poplovok.ID = 4;
                    Console.WriteLine("ИСЧЕЗ!");
                }
                else
                {

                }
            }
            catch (Exception)
            {
                //Console.WriteLine("error LeaveEventHandler:" + exception.Message);

            }
            return Task.CompletedTask;
        }
    }
}
