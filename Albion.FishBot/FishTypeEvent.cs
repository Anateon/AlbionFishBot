using System;
using System.Collections.Generic;
using Albion.Network;
using Newtonsoft.Json;

namespace Albion.FishBot
{
    public class FishTypeEvent : BaseEvent
    {
        public FishTypeEvent(Dictionary<byte, object> parameters) : base(parameters)
        {
            for (byte i = 3; i <= 15; i++)
            {
                try
                {
                    float.TryParse(parameters[i].ToString(), out Info[i - 3]);
                }
                catch (Exception)
                {
                    Info[i - 3] = 0;
                }
            }
            //string json = JsonConvert.SerializeObject(parameters);
            //Console.WriteLine(json);

        }
        public float[] Info = new float[13];
    }
}