using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Events
{
    internal class CustomEventData
    {
        public string Name { get; set; }
        public WeightedRandomEvent Event { get; set; }
        public Floor[] Floors { get; set; }
    }
}
