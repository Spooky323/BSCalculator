using System;
using System.IO;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace ScoreAcc
{
    class Program
    {
        public enum Diffs
        {
            easy = 0,
            expert = 1,
            expertPlus = 2,
            hard = 3,
            normal = 4
        }

        public class Map
        {
            public string Key { get; private set; }
            public int Notes { get; private set; }
            public readonly Diffs Difficulty;
            public Map(string id, int notes, Diffs _diff)
            {
                this.Key = id;
                this.Notes = notes;
                this.Difficulty = _diff;
            }
        }
        public static int GetMaxScore(Map map)
        {
            int max = 115;
            int score = 0;
            int i;
            for (i = 1; i <= map.Notes; i++)
            {
                if (i >= 1 && i < 3)
                {
                    max = 115 * 1;
                    score = score + max;
                }
                else if (i >= 3 && i < 6)
                {
                    max = 115 * 2;
                    score = score + max;
                }
                else if (i >= 6 && i < 14)
                {
                    max = 115 * 4;
                    score = score + max;
                }
                else if (i >= 14)
                {
                    max = 115 * 8;
                    score = score + max;
                    break;
                }
            }
            if (map.Notes > 14)
            {
                return score = score + (map.Notes - i) * 115 * 8;
            }
            return score;
        }
        public static Map GetMapInfo(string key)
        {
            string endpoint = "https://beatsaver.com/api/maps/detail/" + key;    // Make the API GET Call
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";  // User Agent to Perevent Error 404
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string data = reader.ReadToEnd();
            JObject jdata = JObject.Parse(data);
            Console.WriteLine(); // space
            Console.WriteLine("Selected Song = "+jdata["name"]);
            Console.WriteLine("Mapper = " + jdata["metadata"]["levelAuthorName"]+"\n");
            Console.WriteLine("Avilable Difficulties : ");
            for (int i = 0; i < 5; i++)                                                          // Prints Avilable Difficulties
            {
                JToken difficuly = jdata["metadata"]["difficulties"]["" + (Diffs)i + ""];
                string test = difficuly.ToString();
                JValue compare = new JValue("true");
                if (bool.Parse(test) == true)
                {
                    Console.WriteLine(i + ". " + (Diffs)(i));
                }
            }
            Console.WriteLine("Please Enter the requested difficulty : ");
            int choice = int.Parse(Console.ReadLine());
            int notes = (int)(jdata["metadata"]["characteristics"][0]["difficulties"]["" + (Diffs)choice + ""]["notes"]);
            Map map = new Map(key, notes, (Diffs)choice);       // Building the Map Object
            return map;
        }
        static void Main(string[] args)
        {
            Console.Write("Please Enter Map Link : " );
            string url = Console.ReadLine();
            Console.WriteLine();  // space
            string[] id = url.Split('/');
            string key = id[id.Length - 1];
            Map map = GetMapInfo(key);
            Console.WriteLine();  // space
            Console.Write("Please Enter score/accuracy(%) : ");
            var input = Console.ReadLine();
            if (input[input.Length - 1] == '%')
            {
                input = input.Remove(input.Length - 1);
                Console.WriteLine("Need Scores is "+Math.Round(double.Parse(input) / 100 * (double)GetMaxScore(map)));

            }
            else
            {
                Console.WriteLine("Needed Accuracy is " + double.Parse(input) / GetMaxScore(map) * 100 + "%");
            }
            Console.ReadLine();
        }
    }
}
