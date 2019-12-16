using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationAssistant.TranslationServices.Core
{
    /// <summary>
    /// Throw when the credentials are missing.
    /// </summary>
    [Serializable]
    public class CredentialsMissingException : Exception
    {
        public CredentialsMissingException(string message) : base(message) { }

        public CredentialsMissingException()
        {
        }

        public CredentialsMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CredentialsMissingException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
