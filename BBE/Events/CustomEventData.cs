using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Events
{
    public class CustomEventData
    {
        public string Name { get; set; }
        public WeightedRandomEvent Event { get; set; }
        public Floor[] Floors { get; set; }
    }
}
