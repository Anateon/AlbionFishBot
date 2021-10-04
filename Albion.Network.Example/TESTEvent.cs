using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Albion.Network.Example
{
    public class TESTEvent : BaseEvent
    {
        public TESTEvent(Dictionary<byte, object> parameters) : base(parameters)
        {
            string writePath = @"C:\hta2.txt";
            string json = JsonConvert.SerializeObject(parameters);

            Console.WriteLine(json);
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(json);
            }

            //Console.WriteLine(parameters);
        }
    }
}
