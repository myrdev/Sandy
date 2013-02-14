using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading; 


namespace Vigil
{
    public partial class Form1 : Form
    {
        CheckTimers Timer;
        WinAPI API;

        public Form1()
        {
            InitializeComponent();

            Config conf = new Config(@"config.txt");

            API = new WinAPI(conf.timer);
            Thread InterceptThread = new Thread(new ThreadStart(API.RunMe));
            InterceptThread.IsBackground = true;
            InterceptThread.Start();

            Timer = new CheckTimers(API, conf.timer);
            DrawTimers(conf.timer);

            Thread TimerThread = new Thread(new ThreadStart(Timer.RunMe));
            TimerThread.IsBackground = true;
            TimerThread.Start();
        }

        private void DrawTimers(Dictionary<string,Dictionary<string,string>> ts){
            var col = 0;
            var row = 0;
            //var timerPanel = new GroupBox();
            foreach (var t in ts)
            {
                if(t.Value.ContainsKey("image")){

                    var pb = new Label();
                    pb.Size = new Size(100, 100);
                    var img = Image.FromFile(t.Value["image"]).Resize(90, 90);
                    Console.WriteLine("Image added: " + t.Value["image"] + " at " + row + ":" + col);
                    pb.Image = img;
                    pb.Text = t.Key;
                    pb.ForeColor = Color.White;
                    pb.TextAlign = ContentAlignment.BottomCenter;
                    pb.Font = new Font( FontFamily.GenericSerif, (float)20, FontStyle.Bold);
                    pb.Padding = new System.Windows.Forms.Padding(5);//dodgy hack so there is 5 pixels on the top and left, by setting it to size 90x90 in a 100x100 frame, we also get a 5px border on bot+right too
                    pb.BackColor = System.Drawing.Color.Transparent;
                    pb.Location = new Point(col * 100, row * 100);
                    Controls.Add(pb);

                    Timer.AddTimerUI(t.Key, pb);

                    row += col == 2 ? 1 : 0;
                    col = (col + 1) % 3;
                    

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}
