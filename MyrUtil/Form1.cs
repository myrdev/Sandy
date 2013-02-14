using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading; 


namespace MyrUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Config conf = new Config(@"config.txt");

            WinAPI api = new WinAPI(conf.timer);
            Thread InterceptThread = new Thread(new ThreadStart(api.RunMe));
            InterceptThread.Start();

            var timers = new CheckTimers(api, conf.timer);
            Thread TimerThread = new Thread(new ThreadStart(timers.RunMe));
            TimerThread.Start();

            DrawTimers(conf.timer);
        }

        private void DrawTimers(Dictionary<string,Dictionary<string,string>> ts){
            var col = 0;
            var row = 0;
            //var timerPanel = new GroupBox();
            foreach (var t in ts)
            {
                if(t.Value.ContainsKey("image")){
                    var pb = new PictureBox();
                    pb.Size = new Size(100, 100);
                    var img = Image.FromFile(t.Value["image"]).Resize(100,100);
                    Console.WriteLine("Image added: " + t.Value["image"] + " at " + row + ":"+col);
                    pb.Image = img;
                    pb.Location = new Point(col*100, row*100);
                    pb.Visible = true;
                    Controls.Add(pb);

                    row += col == 2 ? 1 : 0;
                    col = (col+1)%3;
                    
                    
                    

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
