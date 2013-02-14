using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyrUtil
{
    public class Play
    {
        public string fn = "";

        public void RunMe()
        {
            if (System.IO.File.Exists(fn))
            {
                //Console.WriteLine(System.DateTime.Now.ToString() + " : BPM - " + filepath + " : " + additionalInfo);
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(fn);
                player.PlaySync();
            }
            else
            {
                Console.WriteLine(System.DateTime.Now.ToString() + " : File missing  " + fn);
            }
        }
    }
}
