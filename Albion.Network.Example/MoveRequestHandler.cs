using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Albion.Network.Example
{
    public class MoveRequestHandler : RequestPacketHandler<MoveOperation>
    {
        public ChelInfo MyInfo;

        public MoveRequestHandler(ChelInfo myInfo) : base(OperationCodes.Move)
        {
            MyInfo = myInfo;
        }

        protected override Task OnActionAsync(MoveOperation value)
        {
            MyInfo.X = value.Position[0];
            MyInfo.Y = value.Position[1];
            //Debug.WriteLine($"Move request {value.Position[0]}; {value.Position[1]}");
            //Console.WriteLine($"Move request {value.Position[0]}; {value.Position[1]}");

            return Task.CompletedTask;
        }
    }
}
