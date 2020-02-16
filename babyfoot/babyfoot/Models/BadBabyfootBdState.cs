using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace babyfoot.Models
{
    public class BadBdStateException : Exception
    {
        public BadBdStateException() : base()
        {

        }

        public BadBdStateException(String message) : base(message)
        {
            
        }
    }
}
