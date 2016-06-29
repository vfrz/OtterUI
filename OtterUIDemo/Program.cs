using Otter;
using OtterUI;
using System;

namespace OtterUIDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.game = new Game("OtterUI Demo", 800, 600);

            Global.playerSession = Global.game.AddSession("Player");
            Global.playerSession.Controller.AddButton("Up");
            Global.playerSession.Controller.Button("Up").AddKey(Key.Up);
            Global.playerSession.Controller.AddButton("Down");
            Global.playerSession.Controller.Button("Down").AddKey(Key.Down);
            Global.playerSession.Controller.AddButton("Left");
            Global.playerSession.Controller.Button("Left").AddKey(Key.Left);
            Global.playerSession.Controller.AddButton("Right");
            Global.playerSession.Controller.Button("Right").AddKey(Key.Right);

            Global.game.FirstScene = new DemoScene();
            Global.game.MouseVisible = true;

            Global.game.Start();
        }
    }

    class DemoScene : Scene
    {

        Background background = new Background();
        Player player = new Player();
        Surface guiSurface = new Surface(800, 600);
        Surface gameSurface = new Surface(800, 600);

        GuiManager testGui;
        GuiButton zoomin, zoomout;
        GuiTextBox textbox;
        GuiButton togglebutton;
        GuiButton rbutton1, rbutton2, rbutton3;
        GuiButton normbutton;

        public DemoScene()
        {
            guiSurface.Scroll = 0f;
            testGui = new GuiManager(Global.game, guiSurface);
            testGui.Layer = -10;
            CreateGui();
            Add(testGui);
        }

        public override void Begin()
        {
            base.Begin();
            background.Surface = gameSurface;
            Add(background);
            player.Surface = gameSurface;
            Add(player);
        }

        public override void Update()
        {
            base.Update();
            gameSurface.CenterCamera(player.X, player.Y);

            if (rbutton1.isSelected)
                player.Graphic.Color = Color.Red;
            else if (rbutton2.isSelected)
                player.Graphic.Color = Color.Green;
            else if (rbutton3.isSelected)
                player.Graphic.Color = Color.Blue;
        }

        public override void Render()
        {
            base.Render();
            gameSurface.Render();
            guiSurface.Render();

        }

        public void CreateGui()
        {
            zoomin = new GuiButton(10, 10, 50, 50, ButtonType.DOWNABLE);
            zoomin.SetText("+", "", 40);
            zoomin.OnClickEvent += zoomin_OnClickEvent;
            testGui.AddWidget(zoomin);

            zoomout = new GuiButton(70, 10, 50, 50, ButtonType.DOWNABLE);
            zoomout.SetText("-", "", 40);
            zoomout.OnClickEvent += zoomout_OnClickEvent;
            testGui.AddWidget(zoomout);

            togglebutton = new GuiButton(640, 10, 150, 50, ButtonType.TOGGLE);
            togglebutton.SetText("TOGGLE", "", 40);
            togglebutton.OnSelectedEvent += togglebutton_OnSelectedEvent;
            togglebutton.OnDeselectedEvent += togglebutton_OnDeselectedEvent;
            testGui.AddWidget(togglebutton);

            textbox = new GuiTextBox(200, 10, 400, 50, 36);
            textbox.MaxCharacters = 18;
            textbox.SetText("text box");
            testGui.AddWidget(textbox);

            rbutton1 = new GuiButton(10, 100, 50, 50, ButtonType.RADIO);
            rbutton1.SetText("R", "", 40);
            testGui.AddButtonToGroup("playercolor", rbutton1);
            testGui.AddWidget(rbutton1);

            rbutton2 = new GuiButton(10, 160, 50, 50, ButtonType.RADIO);
            rbutton2.SetText("G", "", 40);
            testGui.AddButtonToGroup("playercolor", rbutton2);
            testGui.AddWidget(rbutton2);

            rbutton3 = new GuiButton(10, 220, 50, 50, ButtonType.RADIO);
            rbutton3.SetText("B", "", 40);
            rbutton3.isSelected = true;
            testGui.AddButtonToGroup("playercolor", rbutton3);
            testGui.AddWidget(rbutton3);

            normbutton = new GuiButton(10, 540, 400, 50);
            normbutton.SetText("Click Me!", "", 40);
            normbutton.OnClickEvent += new EventHandler(normbutton_OnClickEvent);
            testGui.AddWidget(normbutton);
        }

        public void normbutton_OnClickEvent(object sender, EventArgs e)
        {
            if (textbox.GetText() == "")
                normbutton.SetText("No text entered!", "", 40);
            else
                normbutton.SetText(textbox.GetText(), "", 40);
        }

        public void zoomin_OnClickEvent(object sender, EventArgs e)
        {
            gameSurface.CameraZoom += 0.01f;
        }

        public void zoomout_OnClickEvent(object sender, EventArgs e)
        {
            gameSurface.CameraZoom -= 0.01f;
        }

        public void togglebutton_OnSelectedEvent(object sender, EventArgs e)
        {
            Console.WriteLine("OnSelectedEvent fired");
            textbox.SetText("Toggled!");
        }

        public void togglebutton_OnDeselectedEvent(object sender, EventArgs e)
        {
            Console.WriteLine("OnDeselectedEvent fired");
            textbox.SetText("");
        }
    }

    class Player : Entity
    {

        Image playerImage = Image.CreateRectangle(40, 40, Color.Blue);

        public Player()
        {
            playerImage.CenterOrigin();
            Graphic = playerImage;
            SetPosition(400, 300);

        }

        public override void Update()
        {
            base.Update();
            if (Global.playerSession.Controller.Button("Left").Down)
                X -= 4f;

            if (Global.playerSession.Controller.Button("Right").Down)
                X += 4f;

            if (Global.playerSession.Controller.Button("Up").Down)
                Y -= 4f;

            if (Global.playerSession.Controller.Button("Down").Down)
                Y += 4f;
        }
    }

    class Background : Entity
    {
        public Background()
        {
            Grid background = new Grid(800, 600, 20, 20, Color.Gold, Color.Grey);
            Graphic = background;
        }
    }
}
