
namespace Srk.BetaseriesApiFactory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable]
    public class InvalidDocumentException : Exception
    {
        private string document;
        private string _className;
        private string _stackTraceString;
        private string _remoteStackTraceString;

        public InvalidDocumentException() { }
        
        public InvalidDocumentException(string message) : base(message) { }

        public InvalidDocumentException(string message, Exception inner) : base(message, inner) { }

        public InvalidDocumentException(string message, Exception inner, string document)
            : base(message, inner)
        {
            this.document = document;
        }

        protected InvalidDocumentException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public string Document
        {
            get { return this.document; }
        }

        public override string ToString()
        {
            return this.ToString(true, true);
        }

        private string ToString(bool needFileLineInfo, bool needMessage)
        {
            string text = needMessage ? this.Message : null;
            string text2;
            if (text == null || text.Length <= 0)
            {
                text2 = this.GetClassName();
            }
            else
            {
                text2 = this.GetClassName() + ": " + text;
            }

            if (this.InnerException != null)
            {
                text2 = string.Concat(new string[]
		        {
			        text2,
			        " ---> ",
			        this.InnerException.ToString(),
			        Environment.NewLine,
			        "   ",
			        "--- End of inner exception stack trace ---"

		        });
            }

            string stackTrace = this.GetStackTrace(needFileLineInfo);
            if (stackTrace != null)
            {
                text2 = text2 + Environment.NewLine + stackTrace;
            }

            return text2;
        }

        private string GetClassName()
        {
            if (this._className == null)
            {
                this._className = this.GetType().ToString();
            }

            return this._className;
        }

        private string GetStackTrace(bool needFileInfo)
        {
            string text = this._stackTraceString;
            string text2 = this._remoteStackTraceString;

            if (text != null)
            {
                return text2 + text;
            }

            if (this.StackTrace == null)
            {
                return text2;
            }

            string stackTrace = new StackTrace().ToString();
            return text2 + stackTrace + Environment.NewLine + this.document;
        }

    }
}
