using Otter;
using System;
using System.Collections.Generic;

namespace OtterUI
{
    /// <summary>
    /// This class is used to create your "buttons" before adding them to the GuiManager.  All button types are created with this class.
    /// </summary>
    public class GuiButton : Widget
    {
        #region Private Fields

        private Text buttonText = null;
        private Image buttonInsetImage = null;
        private Image buttonImage = null;
        private Spritemap<string> buttonSpriteMap = null;
        private ButtonType buttonType;
        private bool hasOnSelectedFired = false;

        #endregion

        #region Public Fields

        /// <summary>
        /// Contains a list of other RADIO GuiButtons that share the same group as this widget.
        /// </summary>
        public List<GuiButton> buttonGroup;

        /// <summary>
        /// The name of the group the GuiButton is part of.
        /// </summary>
        public string groupName;

        /// <summary>
        /// Used to sign a unique identifier to the GuiButton
        /// </summary>
        public string buttonID = "";

        /// <summary>
        /// If true, then no base graphics have been defined for the GuiButton, so the GuiButton will use ugly rectangle primatives instead :)
        /// </summary>
        public bool isDefaultImage = false;

        /// <summary>
        /// Used with TOGGLE GuiButtons and RADIO GuiButtons to mark if the button is selected or not
        /// </summary>
        public bool isSelected = false;

        #endregion

        #region Events

        /// <summary>
        /// Fires when a TOGGLE or RADIO GuiButton is clicked on.  Not the same as OnActiveEvent.  Many widgets can be flagged as "selected" at the same time.
        /// </summary>
        public event EventHandler OnSelectedEvent;

        /// <summary>
        /// Fires when a TOGGLE or RADIO GuiButton is deselected.
        /// </summary>
        public event EventHandler OnDeselectedEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates the GuiButton component to be added to the GuiManager and initialises basic fields 
        /// </summary>
        /// <param name="px">X position of the GuiButton relative to the GuiManagers Surface</param>
        /// <param name="py">Y position of the GuiButton relative to the GuiManagers Surface</param>
        /// <param name="w">Width of the widget</param>
        /// <param name="h">Height of the widget</param>
        /// <param name="type">The type of GuiButton to create.  This will determine how the GuiButton will behave.  Defaults to NORMAL</param>
        public GuiButton(int px, int py, int w, int h, ButtonType type = ButtonType.NORMAL)
            : base(px, py, w, h)
        {
            buttonType = type;
            buttonGroup = new List<GuiButton>();
            if (buttonType == ButtonType.DOWNABLE) isDownable = true;
            OnSelectedEvent += new EventHandler(OnSelected);
            OnDeselectedEvent += new EventHandler(OnDeselected);
        }

        #endregion

        #region Component Overrides

        /// <summary>
        /// Adds added logic specific to RADIO and TOGGLE buttons to the widget update loop
        /// </summary>
        public override void Update()
        {
            base.Update();

            if ((buttonType == ButtonType.RADIO || buttonType == ButtonType.TOGGLE) && isSelected)
            {
                if (!hasOnSelectedFired)
                {
                    hasOnSelectedFired = true;
                    OnSelectedEvent.Invoke(this, null);
                    Console.WriteLine("OnSelected fired");
                }

                if (isDefaultImage)
                    buttonImage.Color = Color.Cyan;
                else if (buttonSpriteMap != null)
                    buttonSpriteMap.Play("clicked");
            }
            else if ((buttonType == ButtonType.RADIO || buttonType == ButtonType.TOGGLE) && !isSelected)
            {
                if (hasOnSelectedFired)
                {
                    hasOnSelectedFired = false;
                    OnDeselectedEvent.Invoke(this, null);
                    Console.WriteLine("OnDeselected fired");
                }

                if (!hasOnHoverFired)
                {
                    if (isDefaultImage)
                        buttonImage.Color = Color.White;
                    else if (buttonSpriteMap != null)
                        buttonSpriteMap.Play("inactive");
                }
            }
        }

        /// <summary>
        /// When the component is added to GuiManager, checks to see if the button has any base images defined.  If no images have been set, the button will use
        /// defaults (ugly rectangles)
        /// </summary>
        public override void Added()
        {
            base.Added();
            if (buttonSpriteMap == null && buttonImage == null)
            {
                isDefaultImage = true;
                buttonImage = Image.CreateRectangle(Width, Height);
            }
            else
                isDefaultImage = false;
        }

        /// <summary>
        /// Components have no native 'Graphic' attributes/fields like Entities do, so the images have to be rendered here.
        /// </summary>
        public override void Render()
        {
            base.Render();
            if (buttonImage != null) Draw.Graphic(buttonImage, PosX, PosY);
            if (buttonSpriteMap != null) Draw.Graphic(buttonSpriteMap, PosX, PosY);
            if (buttonInsetImage != null) Draw.Graphic(buttonInsetImage, PosX, PosY);
            if (buttonText != null) Draw.Graphic(buttonText, PosX, PosY);
        }

        #endregion

        #region widget Overrides

        /// <summary>
        /// Adds some basic/default behavior for button graphics to the OnActiveEvent
        /// </summary>
        /// <param name="sender">EventHandler parameters</param>
        /// <param name="e">EventHandler parameters</param>
        public override void OnActive(object sender, EventArgs e)
        {
            base.OnActive(sender, e);
            if (isDefaultImage) buttonImage.Color = Color.Grey;
            if (buttonSpriteMap != null) buttonSpriteMap.Play("active");
        }

        /// <summary>
        /// Adds some basic/default behavior for button graphics to the OnInactiveEvent
        /// </summary>
        /// <param name="sender">EventHandler parameters</param>
        /// <param name="e">EventHandler parameters</param>
        public override void OnInactive(object sender, EventArgs e)
        {
            base.OnInactive(sender, e);
            if (isDefaultImage) buttonImage.Color = Color.White;
            if (buttonSpriteMap != null) buttonSpriteMap.Play("inactive");
        }

        /// <summary>
        /// Adds some basic/default behavior for button graphics to the OnClickEvent
        /// </summary>
        /// <param name="sender">EventHandler parameters</param>
        /// <param name="e">EventHandler parameters</param>
        public override void OnClick(object sender, EventArgs e)
        {
            base.OnClick(sender, e);
            CheckType();

            if (isDefaultImage)
                buttonImage.Color = Color.Cyan;
            else if (buttonSpriteMap != null)
                buttonSpriteMap.Play("clicked");
        }

        /// <summary>
        /// Adds some basic/default behavior for button graphics to the OnHoverEvent
        /// </summary>
        /// <param name="sender">EventHandler parameters</param>
        /// <param name="e">EventHandler parameters</param>
        public override void OnHover(object sender, EventArgs e)
        {
            base.OnHover(sender, e);

            if (isDefaultImage) buttonImage.Color = Color.Blue;
            if (buttonSpriteMap != null) buttonSpriteMap.Play("hover");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Default method added to the OnSelectedEvent
        /// </summary>
        /// <param name="sender">EventHandler parameters</param>
        /// <param name="e">EventHandler parameters</param>
        public void OnSelected(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Default method added to the OnDeselectedEvent
        /// </summary>
        /// <param name="sender">EventHandler parameters</param>
        /// <param name="e">EventHandler parameters</param>
        public void OnDeselected(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Set the text to be displayed on the button
        /// </summary>
        /// <param name="text">The text string</param>
        /// <param name="fontPath">The path of the font you wish to use. Use "" for the default Otter font..</param>
        /// <param name="size">Font size.</param>
        public void SetText(string text, string fontPath, int size)
        {
            buttonText = new Text(text, fontPath, size);
            buttonText.Color = Color.Black;
            buttonText.CenterTextOrigin();
            buttonText.SetPosition(Width / 2, Height / 2);
        }

        /// <summary>
        /// Define a spritemap to be used by the button.  Each spritemap should contain either 3, or 4 images.  Each image will be used depending on which
        /// event state its in.  If you do not want your buttons to change their appearance when "active", use a spritemap with only 3 images. The "active" and "inactive" images will be the same.
        /// The images in the spritemap will be used in this index order:
        /// index 0: "inactive" image
        /// index 1: "hover" image
        /// index 2: "clicked" image
        /// index 3: "active" image
        /// </summary>
        /// <param name="path">The path where the graphic file is stored</param>
        public void SetSpriteMap(string path)
        {
            buttonSpriteMap = new Spritemap<string>(path, Width, Height);
            if (buttonSpriteMap.Frames == 4)
            {
                buttonSpriteMap.Add("inactive", new int[] { 0 }, new float[] { 1f });
                buttonSpriteMap.Add("hover", new int[] { 1 }, new float[] { 1f });
                buttonSpriteMap.Add("clicked", new int[] { 2 }, new float[] { 1f });
                buttonSpriteMap.Add("active", new int[] { 3 }, new float[] { 1f });
            }
            else if (buttonSpriteMap.Frames == 3)
            {
                buttonSpriteMap.Add("inactive", new int[] { 0 }, new float[] { 1f });
                buttonSpriteMap.Add("hover", new int[] { 1 }, new float[] { 1f });
                buttonSpriteMap.Add("clicked", new int[] { 2 }, new float[] { 1f });
                buttonSpriteMap.Add("active", new int[] { 0 }, new float[] { 1f });
            }
        }

        /// <summary>
        /// Sets the buttons text to a new string.  Will throw an error if "SetText" has never been called.
        /// </summary>
        /// <param name="text">The new text for the button</param>
        public void ChangeText(string text)
        {
            buttonText.String = text;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Runs certain methods based on the buttons ButtonType
        /// </summary>
        private void CheckType()
        {
            switch (buttonType)
            {
                case ButtonType.NORMAL:
                    break;
                case ButtonType.RADIO:
                    CheckGroup();
                    break;
                case ButtonType.TOGGLE:
                    CheckSelected();
                    break;
                case ButtonType.DOWNABLE:
                    break;
            }
        }

        /// <summary>
        /// Used for RADIO buttons.  When a RADIO button is selected, all other RADIO buttons in the group are
        /// deselected
        /// </summary>
        private void CheckGroup()
        {
            foreach (GuiButton g in buttonGroup)
                g.isSelected = false;
            isSelected = true;
        }

        /// <summary>
        /// Used for TOGGLE buttons to alternate their selected state on clicking
        /// </summary>
        private void CheckSelected()
        {
            isSelected = !isSelected;
        }

        #endregion
    }
}
