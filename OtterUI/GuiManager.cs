/*
    OtterUI is a GUI library for Otter (2D Game Engine for C# : http://otter2d.com/), 
    based on OtterSauceGui : http://otter2d.com/forum/index.php/topic,117.0.html
*/

using Otter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OtterUI
{
    /// <summary>
    /// This is the base class for creating an OtterUI in Otter.
    /// Use AddWidget to add widgets to this entity, then add the GuiManager object to your Scene after you have added the widgets.
    /// </summary>
    public class GuiManager : Entity
    {
        # region Private Fields

        private Game game;
        private List<List<GuiButton>> buttonGroups = null;
        private List<string> groupNames = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new GuiManager entity to add gui widgets to
        /// </summary>
        /// <param name="g">The instance of the game. Used for passing certain values to widgets</param>
        /// <param name="s">The Surface in your Scene you wish to render the gui to</param>
        public GuiManager(Game g, Surface s = null)
        {
            Surface = s;
            game = g;
            buttonGroups = new List<List<GuiButton>>();
            groupNames = new List<string>();
        }

        #endregion

        #region Entity Overrides

        public override void Update()
        {
            base.Update();
        }

        public override void Render()
        {
            base.Render();
            Surface.Render();
        }

        public override void Added()
        {
            base.Added();
            CreateGroups();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Use this method to add buttons with the type ButtonType.RADIO to a group, so they can react to each other.
        /// The buttons may not function correctly if they are not ButtonType.RADIO
        /// </summary>
        /// <param name="name">The name of the group.  Each name represents a different group</param>
        /// <param name="button">The Button you wish to add</param>
        public void AddButtonToGroup(string name, GuiButton button)
        {
            bool nameExists = false;
            foreach (string n in groupNames)
            {
                if (n == name)
                {
                    nameExists = true;
                    break;
                }
            }

            if (!nameExists) groupNames.Add(name);

            button.groupName = name;
            Console.WriteLine(button.groupName);
        }

        /// <summary>
        /// When you have created a widget and definded its properties, Use this to add the widget component to 
        /// your GuiManager object
        /// </summary>
        /// <param name="w">The widget you wish to add</param>
        public void AddWidget(Widget w)
        {
            w.SetGame(game);
            AddComponent(w);
        }

        /// <summary>
        /// Removes the GuiManager entity from the Scene and resets its base values.  It can be re-added to the scene after it is removed
        /// If you wish to use the same gui in your game again
        /// </summary>
        public void Remove()
        {
            ResetValues();
            RemoveSelf();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a list for each group of radio buttons, and passes the list to each button in
        /// the group to make them aware of eachother
        /// </summary>
        private void CreateGroups()
        {
            foreach (string name in groupNames)
            {
                buttonGroups.Add(new List<GuiButton>());

                foreach (GuiButton b in GetComponents<GuiButton>())
                    if (b.groupName == name)
                        buttonGroups.Last().Add(b);

                foreach (GuiButton b in GetComponents<GuiButton>())
                    if (b.groupName == name)
                        b.buttonGroup = buttonGroups.Last();
            }
        }

        /// <summary>
        /// Resets specific fields in each widget
        /// </summary>
        private void ResetValues()
        {
            foreach (Widget w in Components)
            {
                w.isActive = false;
                w.hasClicked = false;
            }
        }

        #endregion
    }
}
