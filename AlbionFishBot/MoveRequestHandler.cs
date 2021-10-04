using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using Albion.Network;

namespace AlbionFishBot
{
    public class MoveRequestHandler : RequestPacketHandler<MoveOperation>
    {

        public MoveRequestHandler() : base(22)
        {
        }

        protected override Task OnActionAsync(MoveOperation value)
        {
            try
            {
                Program.mutexObj.WaitOne();
                Program.MyPos = new Poplovok()
                {
                    X = value.Position[0],
                    Y = value.Position[1]
                };
                Program.mutexObj.ReleaseMutex();
            }
            catch (Exception exception)
            {
                throw;
            }
            return Task.CompletedTask;
        }
    }
}
