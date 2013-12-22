using System;
using Srk.BetaseriesApi.Resources;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Error from the betaseries service.
    /// </summary>
    public partial class BetaError {

        /// <summary>
        /// Is a string like "1001".
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Convertion from <see cref="Code"/> to an integer.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int")]
        public ushort IntCode { get; private set; }

        /// <summary>
        /// French message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Translated message.
        /// </summary>
        public string DisplayMessage {
            get {
                string msg = null;
                try {
                    msg = ErrorStrings.ResourceManager.GetString(string.Concat("ErrorDetail", Code));
                } catch (Exception) {
                    //TODO: catch the right exception here
                    msg += string.Format(ErrorStrings.ErrorDetailUnknown, Code);
                }

                return msg;
            }
        }

        /// <summary>
        /// Class .ctor without message.
        /// </summary>
        /// <param name="code"></param>
        public BetaError(string code) : this(code, null) { }

        /// <summary>
        /// Class .ctor with message.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public BetaError(string code, string message) {
            if (code == null)
                throw new ArgumentNullException("code");
            if (code.Length != 4)
                throw new ArgumentException("Code must be 4 caracters long. ", "code");

            ushort i = 0;
#if PocketPC
            try {
                i = ushort.Parse(code);
            } catch {
                throw new ArgumentException("Error code is invalid", "code");
            }
#else
            if (!ushort.TryParse(code, out i))
                throw new ArgumentException("Error code is invalid", "code");
#endif

            IntCode = i;
            Message = message;
            this.Code = code;
        }

    }
}
