namespace OtterUI
{
    /// <summary>
    /// Used to define what type of button the GuiButton will be. Defaults to NORMAL
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// A NORMAL GuiButton is a button that has to be clicked each time you want it to fire
        /// </summary>
        NORMAL,

        /// <summary>
        /// A TOGGLE button is a button that can be switched on of off.  Uses the isSelected flag and OnSelectedEvent/OnDeselected event
        /// </summary>
        TOGGLE,

        /// <summary>
        /// A RADIO button is added to a group of other RADIO buttons.  Like the TOGGLE button, it uses the OnSelectedEvent and OnDeselected event.  Unlike the
        /// TOGGLE button, a single RADIO button cannot be switched on and off, and is only deselected when another RADIO button in the same group is
        /// selected
        /// </summary>
        RADIO,

        /// <summary>
        /// A DOWNABLE button is like the NORMAL button except that it will fire the OnClickEvent each frame while the mouse button is held down.
        /// </summary>
        DOWNABLE
    }
}
