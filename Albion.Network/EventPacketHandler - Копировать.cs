using System;
using System.IO;
using System.Threading.Tasks;

namespace Albion.Network
{
    public abstract class EventPacketHandlerForTest<TEvent> : PacketHandler<EventPacket> where TEvent : BaseEvent
    {
        private readonly int eventCode;

        public EventPacketHandlerForTest(int eventCode)
        {
            this.eventCode = eventCode;
        }
        public EventPacketHandlerForTest()
        {
            eventCode = -666;
            this.eventCode = eventCode;
        }

        protected abstract Task OnActionAsync(TEvent value);

        protected internal override Task OnHandleAsync(EventPacket packet)
        {
            if (eventCode == -666)
            {
                if (packet.EventCode != 3 && packet.EventCode != 255 && packet.EventCode != 148)
                {
                    string writePath = @"C:\logIvents.txt";

                    using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine(packet.EventCode);
                    }

                    TEvent instance = (TEvent)Activator.CreateInstance(typeof(TEvent), packet.Parameters);
                    Console.WriteLine(packet.EventCode);
                    return OnActionAsync(instance);
                }
            }
            else if (eventCode != packet.EventCode)
            {
                return NextAsync(packet);
            }
            else
            {
                TEvent instance = (TEvent)Activator.CreateInstance(typeof(TEvent), packet.Parameters);

                return OnActionAsync(instance);
            }
            return NextAsync(packet);
        }
    }
}