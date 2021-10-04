using System;
using System.Threading.Tasks;
using Albion.Network;

namespace AlbionFishBot
{
    public class FishingMiniGameEventHandler : EventPacketHandler<FishingMiniGameEvent>
    {
        public FishingMiniGameEventHandler() : base(331) { }

        protected override Task OnActionAsync(FishingMiniGameEvent value)
        {
            try
            {
                var tmp = new Poplovok()
                {
                    X = value.Position[0],
                    Y = value.Position[1],
                    status = value.Status,
                    ID = value.Id
                };
                //Math.Sqrt(Math.Pow(var.Value.X - MyInfo.X, 2) + Math.Pow(var.Value.Y - MyInfo.Y, 2))
                if (Math.Sqrt(Math.Pow(tmp.X - Program.MyPos.X, 2) + Math.Pow(tmp.Y - Program.MyPos.Y, 2)) <= 15.05)
                {
                    Console.WriteLine(tmp.ToString() +"\t"+ Math.Sqrt(Math.Pow(tmp.X - Program.MyPos.X, 2) + Math.Pow(tmp.Y - Program.MyPos.Y, 2)));
                }
                //MainWindow.mutexObj.WaitOne();
                //MainWindow.chelDictionary[value.Id] = tmp;
                //MainWindow.mutexObj.ReleaseMutex();
            }
            catch (Exception exception)
            {
                throw;
            }
            return Task.CompletedTask;
        }
    }
}