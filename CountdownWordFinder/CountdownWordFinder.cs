using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CountdownWordFinder.Properties;

namespace CountdownWordFinder
{
    class CountdownWordFinder
    {
        static readonly Dictionary<string, HashSet<string>> Dictionary = JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(Resource.dictionary);

        private static void DictionaryLoader()
        {
            // This method only needs to be ran once if a dictionary.json file isn't present 
            // Dictionary text file was taken from https://github.com/dwyl/english-words

            string line = "";
            Dictionary<string, HashSet<string>> words = new Dictionary<string, HashSet<string>>();
            StreamReader sr = new StreamReader(Resource.dictionaryText);

            // Reads through entire dictionary file, line by line and finds words that are between 5 and 9 characters long
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (5 > line.Length || line.Length > 9) 
                    continue;
                // Saves a copy of the word for use in the dictionary hashset
                string word = line;

                // Orders the string alphabetically and does a check to see if that particular order of letters has appeared before
                // if it hasn't been seen before then a new hash set is created with the word as the initialising value.
                // If the letter ordering has been seen before, it is appended to the end of the set.
                line = string.Concat(line.OrderBy(c => c));
                if (!words.ContainsKey(line))
                {
                    HashSet<string> wordSet = new HashSet<string>
                    {
                        word
                    };
                    words.Add(line, wordSet);
                }
                else
                    words[line].Add(word);
            }
            sr.Close();

            // Creates the json file which can be used by the dictionary every time the program is run, saving on computation time.
            using (StreamWriter file = File.CreateText(Resource.dictionary))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, words);
            }
            Console.WriteLine("Done!");
        }

        private static IEnumerable<IEnumerable<T>> CombinationFinder<T>(IEnumerable<T> items, int count)
        {
            // Returns a list of all possible combinations of the items passed through.
            // The combinations will be 'count' long i.e if 2 is passed through, all 
            // the combinations returned will have two characters in them.

            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    foreach (var result in CombinationFinder(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }

                i++;
            }
        }

        private static void DisplaySet(HashSet<string> set)
        {
            //Console.Write("{");
            foreach (string i in set)
            {
                Console.Write("{0}", i);
            }
            Console.WriteLine();
        }

        private static void WordFinder(string orderedLetters)
        {
            // Checks to see if the letter combination is a dictionary value and displays the hash set associated with it, if so.
            if (Dictionary.ContainsKey(orderedLetters))
                DisplaySet(Dictionary[orderedLetters]);
        }

        private static void GetCombinations(List<char> letters)
        {
            // We're only interested in the 5-9 letter characters because anything else isn't really worth the time.
            for (int i = 5; i <= 9; i++)
            {
                Console.WriteLine("{0} Letter Words: ", i);
                var result = CombinationFinder(letters, i);
                foreach (var combination in result)
                {
                    string ordered = string.Join("", combination.ToArray());
                    // Orders the combination for use as a dictionary key.
                    ordered = string.Concat(ordered.OrderBy(c => c));
                    WordFinder(ordered);
                }

                Console.WriteLine();
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Enter 9 characters:");
            string l = Console.ReadLine();

            if (l.Length < 9)
                Console.WriteLine("Not enough characters entered!");
            else
            {
                List<char> letters = new List<char>();
                l = l.ToLower();
                letters.AddRange(l);

                GetCombinations(letters);
                Console.WriteLine("Finished!");
            }
            Console.ReadKey();
        }
    }
}
