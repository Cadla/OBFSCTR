using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Obfuscator.Configuration.COM
{
    public class Event : Member
    {
        EventDefinition _eventDefinition;

        internal EventDefinition EventDefinition        
        {
            get
            {
                return _eventDefinition;
            }
        }
        
        internal Event(EventDefinition eventDefinition)
            : base(eventDefinition)
        {
            _eventDefinition = eventDefinition;
        }

        public Type EventType
        {
            get
            {
                return new COM.Type(_eventDefinition.EventType);
            }
        }


        //public Method AddMethod { get; internal set; }
        //public Method RemoveMethod { get; internal set; }
        
    }
}
