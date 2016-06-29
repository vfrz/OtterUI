using Otter;
using System;
using System.Text;

namespace OtterUI
{
    /// <summary>
    /// This class allows you to create text boxes to add to your GuiManager.  Text boxes are designed to accept a single line of input.
    /// ***WARNING***
    /// Currently bugs associated with this component, which can cause screwy things to happen with the text displayed on this, and other widgets.  
    /// Take care in its use, but expect a fix/update at a (hopefully) near stage
    /// </summary>
    public class GuiTextBox : Widget
    {
        #region Private Fields

        private Text text;
        private Image TextBox;
        private TextBoxType textBoxType;
        private string CurrentTextInput = "";
        private string OldTextInput = "";
        private string PasswordString = "";

        #endregion

        #region Public Fields

        /// <summary>
        /// The maximum amount of characters the user can input and display at once
        /// </summary>
        public int MaxCharacters = 32;

        #endregion

        #region Events

        /// <summary>
        /// This event is fired everytime the text is changed by the user.  ie: Each time the user inputs
        /// a character, or deletes a character
        /// </summary>
        public event EventHandler OnTextChangeEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates and initialises your textbox widget.  After you create it and adjust its properties,
        /// add it to the GuiManager with Addwidget
        /// </summary>
        /// <param name="px">X position of the widget relative to the GuiManagers Surface</param>
        /// <param name="py">Y position of the widget relative to the GuiManagers Surface</param>
        /// <param name="w">Width of widget</param>
        /// <param name="h">Height of widget</param>
        /// <param name="size">Font size of input</param>
        /// <param name="type">Type of input</param>
        public GuiTextBox(int px, int py, int w, int h, int size = 12, TextBoxType type = TextBoxType.NORMAL)
            : base(px, py, w, h)
        {
            PosX = px;
            PosY = py;
            Width = w;
            Height = h;
            textBoxType = type;

            text = new Text();
            text.Color = Color.Black;
            text.FontSize = size;
            TextBox = Image.CreateRectangle(Width, Height);
            TextBox.Color = Color.White;
            clickTimer = 0;
            RenderAfterEntity = true;

            OnTextChangeEvent += OnTextChange;
        }

        #endregion

        #region Component Overrides

        /// <summary>
        /// Controls the behavior of the textbox relative to users actions and input
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (isActive)
            {
                OldTextInput = CurrentTextInput;

                switch (textBoxType)
                {
                    case TextBoxType.NORMAL:
                        CurrentTextInput = GetNormalString(Input.Instance.KeyString);
                        break;
                    case TextBoxType.NUMERICAL:
                        CurrentTextInput = GetNumericalString(Input.Instance.KeyString);
                        break;
                    case TextBoxType.PASSWORD:
                        CurrentTextInput = GetPasswordString(Input.Instance.KeyString);
                        break;
                    case TextBoxType.EMAIL:
                        CurrentTextInput = GetEmailString(Input.Instance.KeyString);
                        break;
                    case TextBoxType.NUMERICALDECI:
                        CurrentTextInput = GetNumericalDeciString(Input.Instance.KeyString);
                        break;
                }

                if (CurrentTextInput.Length > MaxCharacters)
                {
                    CurrentTextInput = CurrentTextInput.Substring(0, MaxCharacters);
                    Input.Instance.KeyString = CurrentTextInput;
                }

                if (CurrentTextInput.Length < MaxCharacters)
                {
                    if (Timer % 30 >= 15)
                        text.String = CurrentTextInput + "|";
                    else
                        text.String = CurrentTextInput;
                }
                else
                    text.String = CurrentTextInput;
            }
            else
                Input.Instance.KeyString = CurrentTextInput;

            if (OldTextInput != CurrentTextInput)
                OnTextChangeEvent.Invoke(this, null);
        }

        /// <summary>
        /// Components have no native Graphic fields, so graphics for the textbox component have to be Drawn 
        /// in the Render loop
        /// </summary>
        public override void Render()
        {
            base.Render();
            Draw.Graphic(TextBox, PosX, PosY);
            Draw.Graphic(text, PosX, PosY);
        }

        #endregion

        #region widget Overrides

        /// <summary>
        /// Changes the graphical appearance and sets the games Input.KeyString when the text box becomes active
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public override void OnActive(object sender, EventArgs e)
        {
            base.OnActive(sender, e);
            Input.Instance.KeyString = CurrentTextInput;
            TextBox.Color = Color.Grey;
        }

        /// <summary>
        /// Changes the graphical appearance when the text box becomes inactive
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public override void OnInactive(object sender, EventArgs e)
        {
            base.OnInactive(sender, e);
            TextBox.Color = Color.White;
        }

        /// <summary>
        /// The default method for the OnTextChangeEvent
        /// </summary>
        /// <param name="sender">EventHandler parameter</param>
        /// <param name="e">EventHandler parameter</param>
        public void OnTextChange(object sender, EventArgs e)
        {
            Console.WriteLine("Text changed!");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the text to be displayed in the textbox.  Can be deleted by the user
        /// </summary>
        /// <param name="t">The text to display</param>
        public void SetText(string t)
        {
            Input.Instance.KeyString = t;
            OldTextInput = t;
            CurrentTextInput = t;
            text.String = t;
        }

        /// <summary>
        /// Clears the text displayed in the text box and resets the Input.KeyString
        /// </summary>
        public void ClearText()
        {
            OldTextInput = "";
            CurrentTextInput = "";
            text.String = "";
            Input.Instance.ClearKeystring();
        }

        /// <summary>
        /// Returns the text displayed in the text box
        /// </summary>
        /// <returns>The text in the text box</returns>
        public string GetText()
        {
            return CurrentTextInput;
        }

        /// <summary>
        /// Returns the raw, uncensored password, which was entered in the textbox
        /// </summary>
        /// <returns>The actual string entered in the text box, not the stars</returns>
        public string GetPassword()
        {
            if (textBoxType != TextBoxType.PASSWORD)
                return null;
            else
                return PasswordString;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks the keys entered into NORMAL text box
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string containing only permitted characters</returns>
        private string GetNormalString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
                if (c != '\n')
                    sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        /// Checks the keys entered into NUMERICAL text box
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string containing only permitted characters</returns>
        private string GetNumericalString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
                if (c != '\n')
                    if (c >= '0' && c <= '9')
                        sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        /// Checks the keys entered into NUMERICALDECI text box
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string containing only permitted characters</returns>
        private string GetNumericalDeciString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
                if (c != '\n')
                    if ((c >= '0' && c <= '9') || c == '.')
                        sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        /// Checks the keys entered into PASSWORD text box.  Stores the input string in another variable for retrieval using GetPassword().
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string of "*" based on the amount of characters entered</returns>
        private string GetPasswordString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
                if (c != '\n')
                    sb.Append(c);
            PasswordString = sb.ToString();

            StringBuilder hs = new StringBuilder();
            foreach (char c in PasswordString)
                hs.Append('*');
            return hs.ToString();
        }

        /// <summary>
        /// Checks the keys entered into EMAIL text box
        /// </summary>
        /// <param name="str">The Input.KeyString of the game</param>
        /// <returns>A string containing only permitted characters</returns>
        private string GetEmailString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
                if (c != '\n')
                    if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '@' || c == '.' || c == '-' || c == '_')
                        sb.Append(c);
            return sb.ToString();
        }

        #endregion
    }
}
