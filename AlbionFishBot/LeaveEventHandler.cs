using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using Albion.Network;

namespace AlbionFishBot
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
                if (Program.Poplovok.ID == value.Id)
                {
                    Program.Poplovok.ID = 0;
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
