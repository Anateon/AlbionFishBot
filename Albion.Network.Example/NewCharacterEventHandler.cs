using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;

namespace Albion.Network.Example
{
    public class NewCharacterEventHandler : EventPacketHandler<NewCharacterEvent>
    {
        public Dictionary<string, ChelInfo> cheliki;

        public NewCharacterEventHandler(Dictionary<string, ChelInfo> a) : base(EventCodes.NewCharacter)
        {
            cheliki = a;
        }

        protected override Task OnActionAsync(NewCharacterEvent value)
        {
            if (!cheliki.ContainsKey(value.Id))
            {
                using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Ringin.wav"))
                {
                    soundPlayer.Play();
                }
            }
            cheliki[value.Id] = new ChelInfo()
            {
                name = value.Name,
                X = value.Position[0],
                Y = value.Position[1],
                time = DateTime.Now
            };
            Console.WriteLine($"New ch Id: {value.Id} name: {value.Name} hp: {value.FullHP}/{value.NowHP}");

            return Task.CompletedTask;
        }
    }
}
