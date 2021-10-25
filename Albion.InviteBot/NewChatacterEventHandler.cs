using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Albion.Network;

namespace Albion.InviteBot
{
    public class NewChatacterEventHandler : EventPacketHandler<NewCharacterEvent>
    {

        public NewChatacterEventHandler() : base(25)
        {
        }

        protected override Task OnActionAsync(NewCharacterEvent value)
        {
            try
            {
                if (value.GuildName == null)
                {
                    //Program.mutexObj.WaitOne();
                    if (!Program.invitedList.Contains(value.Name) && !Program.chelList.Contains(value.Name))
                    {
                        Console.WriteLine($"{Program.allPlayers++}\tadd:\t{value.Name}");
                        Program.chelList.Enqueue(value.Name);
                    }
                    //Program.mutexObj.ReleaseMutex();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
            return Task.CompletedTask;
        }
    }
}
