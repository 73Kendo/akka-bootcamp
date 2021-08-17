using System;
using System.Collections.Generic;
using System.Text;

namespace WinTail
{
    class Messages
    {
        #region Neutral/system messages
        /// <summary>
        /// Marker class to continue processing
        /// </summary>
        public class ContinueProcessing { }
        #endregion
        /// <summary>
        /// Base classs for signalling that user input was valid
        /// </summary>
        #region Success messages
        public class InputSuccess
        {
            public InputSuccess(string reason)
            {
                Reason = reason;
            }

            public string Reason { get; private set; }
            
        }
        #endregion
        #region Error messages
        /// <summary>
        /// Base class for singnal
        /// </summary>
        public class InputError
        {
            public string Reason { get; private set; }

            public InputError(string reason)
            {
                Reason = reason;
            }
        }
        /// <summary>
        /// User provided blank input
        /// </summary>
        public class NullInputError : InputError
        {
            public NullInputError(string reason) : base(reason)
            {
            }
        }
        /// <summary>
        /// User provided invalid input (curently, input w / odd # chars)
        /// </summary>
        public class ValidatonError : InputError
        {
            public ValidatonError(string reason) : base(reason)
            {
            }
        }
        #endregion


    }
}
