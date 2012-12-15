using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forever
{
    public class ForeverException : Exception
    {
        public ForeverException(string message) : base(message) { }
    }
    public class MathematicalException : ForeverException 
    {
        public MathematicalException(string message) : base(message) { }
    }
}
