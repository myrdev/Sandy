using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vigil
{
    class Config
    {
        //public Dictionary<string, Dictionary<string, string>> option; //NYI
        //public Dictionary<string, Dictionary<string, string>> keypress; //NYI
        public Dictionary<string, Dictionary<string, string>> timer;

        private Dictionary<string, string> SplitDelineatedKVPairs(string s, char pairDelineator, char keyValueDelineator)
        {
            var result = new Dictionary<string, string>();

            var pairs = s.Split(pairDelineator);
            foreach (var pair in pairs)
            {
                var kvp = pair.Split(keyValueDelineator);
                result.Add(kvp[0], kvp[1]);
                //Console.WriteLine("found K:" + kvp[0] + " and V:" + kvp[1]);
            }

            return result;
        }

        public Config(string fileLoc)
        {
            //<option name,<bunch of stuff about that option>>
            timer = new Dictionary<string, Dictionary<string, string>>();

            if (System.IO.File.Exists(fileLoc))
            {
                string[] unparsedLines = System.IO.File.ReadAllLines(fileLoc);


                foreach (string line in unparsedLines)
                {
                    if (line.StartsWith("#"))
                    {
                        //skip comments
                    }
                    else
                    {
                        if (line.StartsWith("keypress="))
                        {
                            var keypress = SplitDelineatedKVPairs(line, '♣', '=');

                            if (keypress["keypress"] == "timer")
                            {
                                Console.WriteLine("Adding keypress: " + line);
                                timer.Add(keypress["key"], keypress);
                            }
                            else
                            {
                                Console.WriteLine("UNKNOWn<keypress type> " + keypress["keypress"]);
                            }

                        }
                        else
                        {
                            Console.WriteLine("UNKNOWN<optionType> " + line);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("File missing from exe directory: " + fileLoc);
            }
        }
    }
}
