using System;
using System.Threading;
using System.Threading.Tasks;
using Albion.Network;

namespace Albion.FishBot
{
    public class FishingMiniGameEventHandler : EventPacketHandler<FishingMiniGameEvent>
    {
        public FishingMiniGameEventHandler() : base(331) { }

        protected override Task OnActionAsync(FishingMiniGameEvent value)
        {
            try
            {
                Program.LastOwnerID = value.ownerID;
                //Math.Sqrt(Math.Pow(var.Value.X - MyInfo.X, 2) + Math.Pow(var.Value.Y - MyInfo.Y, 2))
                if ((Program.NeedOwnerID == value.ownerID || Program.NeedOwnerID == 0) && (Math.Sqrt(Math.Pow(value.Position[0] - Program.MyPos.X, 2) + Math.Pow(value.Position[1] - Program.MyPos.Y, 2)) <= 15.05))
                {
                    //Console.WriteLine(value.ownerID);
                    Program.mutexObj.WaitOne();
                    Program.Poplovok.X = value.Position[0];
                    Program.Poplovok.Y = value.Position[1];
                    Program.Poplovok.status = value.Status;
                    Program.Poplovok.ID = value.Id;
                    Program.Poplovok.ownerID = value.ownerID;
                    Program.mutexObj.ReleaseMutex();
                    if (value.Status == 2)
                    {
                        Program.ClickThread = new Thread(new ThreadStart(Program.TriggerClicker));
                        Program.ClickThread.Start();
                    }
                }
            }
            catch (Exception exception)
            {
                throw;
            }
            return Task.CompletedTask;
        }
    }
}