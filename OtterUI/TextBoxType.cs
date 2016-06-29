namespace OtterUI
{
    /// <summary>
    /// Used to define what characters you wish to allow the user to input
    /// </summary>
    public enum TextBoxType
    {
        /// <summary>
        /// The default type for text boxes is NORMAL.  Will allow all alphanumerical and special characters.
        /// </summary>
        NORMAL,

        /// <summary>
        /// Will accept the same characters as NORMAL, but will not display them. (replaces characters with "*").  
        /// Call GetPassword() to retrieve the raw password from the  text box for processing
        /// </summary>
        PASSWORD,

        /// <summary>
        /// Allows alpha characters, ".", "-", "_", and "@" characters
        /// </summary>
        EMAIL,

        /// <summary>
        /// Allows numbers to be entered only
        /// </summary>
        NUMERICAL,

        /// <summary>
        /// Allows numbers and the "." character
        /// </summary>
        NUMERICALDECI
    }
}
