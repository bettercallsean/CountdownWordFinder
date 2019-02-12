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
        static Dictionary<string, HashSet<string>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(Resource.dictionary);

        static void DictionaryLoader()
        {
            // This method only needs to be ran once if a dictionary.json file isn't present 
            // Dictionary text file was taken from https://github.com/dwyl/english-words

            string line = "";
            Dictionary<string, HashSet<string>> words = new Dictionary<string, HashSet<string>>();
            int counter = 0;
            StreamReader sr = new StreamReader(Resource.dictionaryText);

            // Reads through entire dictionary file, line by line and finds words that are between 5 and 9 characters long
            while ((line = sr.ReadLine()) != null)
            {
                line.Trim();
                if (5 <= line.Length && line.Length <= 9)
                {
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
                counter++;
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

        static IEnumerable<IEnumerable<T>> CombinationFinder<T>(IEnumerable<T> items, int count)
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
                Console.Write(" {0}", i);
            }
            Console.WriteLine();
        }

        static void WordFinder(string word)
        {
            // Checks to see if the letter combination is a dictionary value and displays the hash set associated with it, if so.
            if (dictionary.ContainsKey(word))
                DisplaySet(dictionary[word]);
        }

        static void GetCombinations(List<string> letters)
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
            Console.WriteLine("Enter 9, space-seperated characters: ");
            string l = Console.ReadLine();

            List<string> letters = l.ToLower().Split().ToList();

            GetCombinations(letters);
            Console.WriteLine("Finished!");
            
            Console.ReadKey();
        }
    }
}
