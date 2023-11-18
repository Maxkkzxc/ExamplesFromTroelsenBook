﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyEBookReader
{
    internal class Program
    {
        private static string theEBook = "";
        static void Main(string[] args)
        {
            GetBook();
            Console.WriteLine("Downloading book...");
            Console.ReadLine();
        }

        static void GetBook()
        {
            WebClient wc = new WebClient();

            wc.DownloadStringCompleted += (s, eArgs) =>
            {
                theEBook = eArgs.Result;
                Console.WriteLine("Download complete.");
                GetStats();
            };
            wc.DownloadStringAsync(new Uri("http://www.gutenberg.org/files/98/98-0.txt"));
        }

        static void GetStats()
        {
            string[] words = theEBook.Split(new char[] {' ', '\u000A', ',', '.', ';', ':', '-', '?', '/' },
                StringSplitOptions.RemoveEmptyEntries);

            string[] tenMostCommon = null;
            string longestWord = string.Empty;

            Parallel.Invoke(
                () =>
                {
                    tenMostCommon = FindTenMostCommon(words);
                },
                () =>
                {
                    longestWord = FindLongestWord(words);
                });

            StringBuilder bookStats = new StringBuilder("Ten Most Common Words are:\n");
            foreach (string s in tenMostCommon)
            {
                bookStats.AppendLine(s);
            }
            bookStats.AppendLine($"Longest word is: {longestWord}");
            bookStats.AppendLine();
            Console.WriteLine(bookStats.ToString(), "Book info");
        }

        static string[] FindTenMostCommon(string[] words)
        {
            var frequencyOrder = from word in words
                                 where word.Length > 6
                                 group word by word into g
                                 orderby g.Count() descending
                                 select g.Key;

            string[] commonWords = (frequencyOrder.Take(10)).ToArray();
            return commonWords;
        }
        static string FindLongestWord(string[] words)
        {
            return (from w in words orderby w.Length descending select w).FirstOrDefault();
        }
    }
}