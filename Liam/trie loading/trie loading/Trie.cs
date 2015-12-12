using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trie_loading
{

    public class Trie<V> where V : class
    {

        private TrieNode<V> root;

        /// <summary>
        /// Matcher object for matching prefixes of strings to the strings stored in this trie.
        /// </summary>
        public IPrefixMatcher<V> Matcher { get; private set; }

        /// <summary>
        /// Create an empty trie with an empty root node.
        /// </summary>
        public Trie()
        {
            this.root = new TrieNode<V>(null, '\0');
            this.Matcher = new PrefixMatcher<V>(this.root);
        }

        /// <summary>
        /// Put a new key value pair, overwriting the existing value if the given key is already in use.
        /// </summary>
        /// <param name="key">Key to search for value by.</param>
        /// <param name="value">Value associated with key.</param>
        public void Put(string key, V value)
        {
            TrieNode<V> node = root;
            foreach (char c in key)
            {
                node = node.AddChild(c);
            }
            node.Value = value;
        }

        /// <summary>
        /// Remove the value that a key leads to and any redundant nodes which result from this action.
        /// Clears the current matching process.
        /// </summary>
        /// <param name="key">Key of the value to remove.</param>
        public void Remove(string key)
        {
            TrieNode<V> node = root;
            foreach (char c in key)
            {
                node = node.GetChild(c);
            }
            node.Value = null;

            //Remove all ancestor nodes which don't lead to a value.
            while (node != root && !node.IsTerminater() && node.NumChildren() == 0)
            {
                char prevKey = node.Key;
                node = node.Parent;
                node.RemoveChild(prevKey);
            }

            Matcher.ResetMatch();
        }
        public Boolean Search(string key)
        {
            TrieNode<V> node = root;
            foreach (char c in key)
            {
                if (node.ContainsKey(c))
                {
                    node = node.GetChild(c);
                }
                else
                {
                    return false;
                }
            }
            if (node.IsTerminater())
            {
                return true;
            }
            else
            {
                return false;
            }

            
        }
        public List<string> AIsearch(string key)
        {
            TrieNode<V> node = root;
           
            string currentWord = "";
            List<string> foundWords = new List<string>();
            foundWords = (AIalgorithm(node, foundWords, currentWord, key));
            return foundWords;
        }
        private List<string> AIalgorithm(TrieNode<V> node,List<string> foundwords, string currentWord, string inputString)
        {

            currentWord += node.Key;

             if (inputString == "" || node.IsLeaf())
             {
                 if (node.IsTerminater())
                    {
                        foundwords.Add(currentWord);
                    }
                 
                 return foundwords;
             }
             if (node.IsTerminater())
            {
                foundwords.Add(currentWord);
            }
            
            HashSet<char> characterSet = new HashSet<char>( inputString );
             foreach (char c in characterSet)
             {
                 if (node.ContainsKey(c))
                 {
                     int index1 = inputString.IndexOf(c);
                   String newinputString = inputString.Remove(index1, 1);

                   foundwords.Concat(AIalgorithm(node.GetChild(c), foundwords, currentWord, newinputString)); 
                 }
         
             }
            return foundwords;
         
        }
            //List<char> testedletters =new List<char>();


           
           
                
            //    foreach (char c in inputString)
            //    {
            //       // Console.Write(c);
            //        if (node.ContainsKey(c) == false)
            //        {
            //            return foundwords;
            //        }
            //        else if (node.ContainsKey(c))// && !testedletters.Contains(c))
            //        {
            //            currentWord = currentWord + c;
            //           // node = node.GetChild(c);
            //            if (node.IsTerminater())
            //            {
            //                if (foundwords.Contains(currentWord) == false)
            //                {
            //                    foundwords.Add(currentWord);
            //                }
            //            }
            //            if (node.IsLeaf() == true)
            //            {
                            
            //                return foundwords;
            //            }
            //            string temp = inputString;
            //            node = node.GetChild(c);
            //            //TrieNode<V> node1;
            //           // TrieNode<V> node2;
            //            //currentWord = currentWord + c;
            //            int index1 = inputString.IndexOf(c);
            //            string newinputString = inputString.Remove(index1, 1);
            //            char newchar = inputString.Last();
            //            testedletters.Add(c);
            //            inputString = temp;
            //            //Console.Write(c);
            //            if (newinputString == "")
            //            {
            //                return foundwords;
            //            }
            //          //  Console.Write("\n");
            //           // Console.Write(currentWord);
            //          //  Console.Write("\n");

                       
            //         //  foundwords = (AIalgorithm(node.GetChild(newchar), foundwords, currentWord, newinputString));
            //            // = foundwords.Concat(AIalgorithm(node, foundwords, currentWord, inputString));
                    }
                


               // return (AIalgorithm(node, foundwords, currentWord, inputString));

               // return foundwords;
               // return foundwords;
        
         

    
}
