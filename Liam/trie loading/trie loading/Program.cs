using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;
using System.Diagnostics;
namespace trie_loading
{
    class Program
    {
        static void Main(string[] args)
        {
            Trie<string> trie = new Trie<string>();
           
            string val = "100";
            //Add some key-value pairs to the trie

           // var reader = new StreamReader(File.OpenRead(@"C:\Users\liam\Downloads\EOWL-v1.1.2\CSV Format\A Words"));
           
            Stopwatch timePerParse;
            
               String[] file_array = Directory.GetFiles(@"C:\Users\liam\Downloads\EOWL-v1.1.2\CSV Format");



               foreach (String s in file_array)
               {
                   //For each file in the files array
                   //Iterate through the delimited files
                   //Add each word to the database
                   String[] lines = File.ReadAllLines(s);

                   //while (!reader.EndOfStream)
                   //{
                   foreach (String p in lines)
                   {
                       trie.Put(p, val);
                       val = (int.Parse(val) + 1).ToString();
                   }

               }

              
              
               //for (int i = 0; i < 10000000; i++)
               //{
               //    trie.Search("elaborite");
               //}
               long ticksThisTime = 0;
               timePerParse = Stopwatch.StartNew();
               List<String> test = trie.AIsearch("masturbates");
               Console.Write( "\n");
               //for (int i = 0; i < test.Count(); i++)
               //{
               //   // Console.Write(test[i] + "     "+i+"\n");
               //}

               Console.Write( "\nhelp please   \n");
              // {
              //     trie.Search("elaborite");
               //}
                   timePerParse.Stop();
               ticksThisTime = timePerParse.ElapsedMilliseconds;
               Console.Write(ticksThisTime);
               Console.Read();
        }
    }
}
