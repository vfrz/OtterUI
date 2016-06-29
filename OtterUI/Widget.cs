using Otter;
using System;

namespace OtterUI
{
    public class Widget : Component
    {
        #region Private Fields

        /// <summary>
        /// If a shortcut is defined, this will be set to true and the widget will listen for the key to be pressed.
        /// Do not use shortcuts if there are GuiTextBoxes in your gui
        /// </summary>
        private bool isShortcutSet = false;

        /// <summary>
        /// The Key to be used as the shortcut key for this widget.
        /// </summary>
        private Key Shortcut;

        #endregion

        #region Protected Fields

        /// <summary>
        /// Current game instance
        /// </summary>
        protected Game game;

        /// <summary>
        /// Width of the widget
        /// </summary>
        protected int Width;

        /// <summary>
        /// Height of the widget
        /// </summary>
        protected int Height;

        /// <summary>
        /// X coordinate relative to its surface
        /// </summary>
        protected int PosX;

        /// <summary>
        /// Y coordinate relative to its surface
        /// </summary>
        protected int PosY;

        /// <summary>
        /// How long a "click" graphic should disply, in frames, before firing the next event
        /// </summary>
        protected int clickTimer = 15;

        /// <summary>
        /// Counts the frames when clicked
        /// </summary>
        protected int clickCounter = 0;

        /// <summary>
        /// Used for ButtonType.DOWNABLE GuiButtons.  If true, will repeatedly fire OnClickEvent when the left mouse button
        /// is held down on a widget.  If false, will only fire OnClickEvent once when the button is clicked.
        /// </summary>
        protected bool isDownable = false;

        /// <summary>
        /// Flag used to tell other events if the OnClickEvent has been invoked
        /// </summary>
        protected bool hasOnClickFired = false;

        /// <summary>
        /// Flag used to tell other events if the OnActiveEvent has been invoked
        /// </summary>
        protected bool hasOnActiveFired = false;

        /// <summary>
        /// Flag used to tell other events if the OnInactiveEvent has been invoked
        /// </summary>
        protected bool hasOnInactiveFired = false;

        /// <summary>
        /// Flag used to tell other events if the OnHoverEvent has been invoked
        /// </summary>
        protected bool hasOnHoverFired = false;

        #endregion

        #region Public Fields

        /// <summary>
        /// If a widget has just been clicked, it becomes "active".  
        /// There can only be one active widget at any time regardless of grouping, or which GuiManager the widget belongs to
        /// </summary>
        public bool isActive = false;

        /// <summary>
        /// If a widget has just been clicked
        /// </summary>
        public bool hasClicked = false;

        #endregion

        #region Events

        /// <summary>
        /// Fired when a user clicks a widget.  In most cases it will only fire ONCE per click.
        /// If the widget field "isDownable" is true, it will fire EVERY FRAME while the mouse button is held over it.
        /// </summary>
        public event EventHandler OnClickEvent;

        /// <summary>
        /// Fired ONCE when a widget becomes active.  A widget becomes active after the user clicks on it.  Only one widget can be active in the game scene at any one time.
        /// Used mainly for changing widget graphics based on the isActive flag, and for choosing which GuiTextBox should allow input.
        /// </summary>
        public event EventHandler OnActiveEvent;

        /// <summary>
        /// Fires ONCE when a widget becomes inactive.  Used mainly for preventing GuiTextBox input and changing widget graphics.
        /// </summary>
        public event EventHandler OnInactiveEvent;

        /// <summary>
        /// Fires ONCE when a users mouse hovors over the widget.  Used mainly for changing the widget graphic.  
        /// Reverts back to an "active" or "inactive" graphic when the mouse moves out of bounds of the widget.
        /// </summary>
        public event EventHandler OnHoverEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialises the widget with basic properties required by all widgets, and initialises events for the widget
        /// </summary>
        /// <param name="px">X position of the widget relative to the GuiManagers surface</param>
        /// <param name="py">Y position of the widget relative to the GuiManagers surface</param>
        /// <param name="w">Width of the widget</param>
        /// <param name="h">Height of the widget</param>
        public Widget(int px, int py, int w, int h)
        {
            PosX = px;
            PosY = py;
            Width = w;
            Height = h;

            Shortcut = Key.Unknown;

            OnClickEvent += OnClick;
            OnActiveEvent += OnActive;
            OnInactiveEvent += OnInactive;
            OnHoverEvent += OnHover;

        }

        #endregion

        #region Component Overrides

        public override void Update()
        {
            base.Update();
            CheckBounds();
            CheckKey();
            ClickTimer();
        }

        public override void Added()
        {

        }

        #endregion

        #region Protected Virtual Methods

        /// <summary>
        /// Checks the click timer and sets hasClicked as false if the timer has finished
        /// </summary>
        protected virtual void ClickTimer()
        {
            if (hasClicked && clickCounter == clickTimer)
                hasClicked = false;
            else
                clickCounter++;
        }

        #endregion

        #region Public Virtual Methods

        /// <summary>
        /// The default method added to OnActiveEvent.  Is triggered by all widgets.
        /// Dont call this method directly
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public virtual void OnActive(object sender, EventArgs e)
        {
            ResetEventFlags();
            hasOnActiveFired = true;
        }

        /// <summary>
        /// The default method added to OnInactiveEvent.  Is triggered by all widgets.
        /// Dont call this method directly
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public virtual void OnInactive(object sender, EventArgs e)
        {
            ResetEventFlags();
            hasOnInactiveFired = true;
        }

        /// <summary>
        /// The default method added to OnClickEvent.  Is triggered by all widgets.
        /// Dont call this method directly
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public virtual void OnClick(object sender, EventArgs e)
        {
            ResetEventFlags();
            hasOnClickFired = true;
        }

        /// <summary>
        /// The default method added to OnHoverEvent.  Is triggered by all widgets.
        /// Dont call this method directly
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public virtual void OnHover(object sender, EventArgs e)
        {
            ResetEventFlags();
            hasOnHoverFired = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the game instance for the widget to use.
        /// Dont call this directly.
        /// </summary>
        /// <param name="g">The current game instance</param>
        public void SetGame(Game g)
        {
            game = g;
        }

        /// <summary>
        /// Set the shortcut key you want to trigger the widget.  Using the shortcut key
        /// invokes an OnClickEvent.
        /// Dont use shortcuts if your gui contains a GuiTextBox
        /// </summary>
        /// <param name="k">The Key you want to reserve for the widget</param>
        public void SetShortcut(Key k)
        {
            Shortcut = k;
            isShortcutSet = true;
        }

        /// <summary>
        /// Call this to manually simulate a click on a widget from within your code.
        /// </summary>
        public void Click()
        {
            hasClicked = true;
            clickCounter = 0;
            isActive = true;
            OnClickEvent.Invoke(this, null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is in charge of checking collisions, your mouse position in the gui, and fires events based
        /// on user actions.
        /// </summary>
        private void CheckBounds()
        {
            bool inBounds = Util.InRect(Scene.MouseX, Scene.MouseY, Scene.CameraX + PosX, Scene.CameraY + PosY, Width, Height);

            if (inBounds && CheckMouseClick())
            {
                Click();
            }
            else if (!inBounds && Input.Instance.MouseButtonPressed(MouseButton.Left))
            {
                if (!hasOnInactiveFired) OnInactiveEvent.Invoke(this, null);
                isActive = false;
            }
            else if (inBounds && !hasClicked)
            {
                if (!hasOnHoverFired) OnHoverEvent.Invoke(this, null);
            }
            else if (!inBounds && !hasClicked)
            {
                if (isActive)
                {
                    if (!hasOnActiveFired) OnActiveEvent.Invoke(this, null);
                }
                else
                {
                    if (!hasOnInactiveFired) OnInactiveEvent.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// If you set a shortcut key for the widget, this method checks to see
        /// when the user presses the key, and responds to it with a Click()
        /// </summary>
        private void CheckKey()
        {
            if (Input.Instance.KeyPressed(Shortcut) && isShortcutSet)
                Click();
        }

        /// <summary>
        /// Checks if the mouse has been "Pressed" or is "down" based on the widget type.
        /// </summary>
        /// <returns>Returns true if the mouse is clicked in the bounds of a widget</returns>
        private bool CheckMouseClick()
        {
            if (isDownable)
                return Input.Instance.MouseButtonDown(MouseButton.Left);
            else
                return Input.Instance.MouseButtonPressed(MouseButton.Left);
        }

        /// <summary>
        /// Resets flags each time an event is fired, to allow other events to fire.
        /// The flags help control when events are fired and how frequently.
        /// </summary>
        private void ResetEventFlags()
        {
            hasOnClickFired = false;
            hasOnActiveFired = false;
            hasOnInactiveFired = false;
            hasOnHoverFired = false;
        }

        #endregion
    }
}
