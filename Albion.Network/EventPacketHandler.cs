using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Albion.Network
{
    public abstract class EventPacketHandler<TEvent> : PacketHandler<EventPacket> where TEvent : BaseEvent
    {
        private readonly int eventCode;

        public EventPacketHandler(int eventCode)
        {
            this.eventCode = eventCode;
        }

        protected abstract Task OnActionAsync(TEvent value);

        protected internal override Task OnHandleAsync(EventPacket packet)
        {
            List<int> vs = new List<int>() { 3, 93, 148, 18, 10, 20, 255, 81, 94 };
#if DEBUG
            if (eventCode == -666)
            {
                if (packet.EventCode == 333)//packet.EventCode == 332)
                {
                    if (!vs.Contains(packet.EventCode))
                    {
                        Console.Write($"\n{packet.EventCode}\t{(EnumEvents)packet.EventCode} ");
                        string json = JsonConvert.SerializeObject(packet.Parameters);
                        Console.Write(json);
                    }
                    return NextAsync(packet);
                }
                //Console.WriteLine($"{packet.EventCode}\t{(EnumEvents) packet.EventCode}");
            }
#endif

            if (eventCode != packet.EventCode)
            {
                return NextAsync(packet);
            }
            else
            {
                TEvent instance = (TEvent)Activator.CreateInstance(typeof(TEvent), packet.Parameters);

                return OnActionAsync(instance);
            }
        }
    }
}