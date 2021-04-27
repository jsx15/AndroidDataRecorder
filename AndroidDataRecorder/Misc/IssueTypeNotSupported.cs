using System;
using System.Data.SqlTypes;

namespace AndroidDataRecorder.Misc
{
    [Serializable]
    public class IssueTypeNotSupported : Exception
    {
        public IssueTypeNotSupported() : base() { }
        public IssueTypeNotSupported(string message) : base(message) { }
        public IssueTypeNotSupported(string message, Exception inner) : base(message, inner) { }
        
        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected IssueTypeNotSupported(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}