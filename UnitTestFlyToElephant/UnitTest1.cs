using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using FlyToElephant;

namespace UnitTestFlyToElephant
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void CheckBuildChainArray()
        {
            List<string> dictionary = new List<string>() { "hot", "dot", "dog", "lot", "log", "pot", "pol", "los" };

            List<Program.GraphDictionary> graphDictActual = new List<Program.GraphDictionary>();

            graphDictActual.Add(new Program.GraphDictionary() { Node = null, childNode = "hit" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "hit", childNode = "hot" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "hot", childNode = "dot" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "hot", childNode = "lot" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "hot", childNode = "pot" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "dot", childNode = "dog" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "lot", childNode = "log" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "los", childNode = "lot" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "pot", childNode = "pol" });
            graphDictActual.Add(new Program.GraphDictionary() { Node = "dog", childNode = "cog" });
            
            List<string> ActualStringArr = new List<string>() { "hit", "hot", "dot", "dog", "cog" };
            
            List<string> CurrentStringArr = Program.BuildChainArray(graphDictActual, "cog");
            
            CollectionAssert.AreEqual(CurrentStringArr, ActualStringArr);

            CurrentStringArr = Program.BuildChainArray(null, "cog");

            Assert.AreEqual(null, CurrentStringArr);

            CurrentStringArr = Program.BuildChainArray(graphDictActual, null);

            Assert.AreEqual(null, CurrentStringArr);

        }
        
        [TestMethod]
        public void CheckBuldGraphFlyToElephant()
        {
            List<string> dictionary = new List<string>() { "hot", "dot", "dog", "lot", "log","pot","pol","los" };

            List<Program.GraphDictionary> graphDictActual = new List<Program.GraphDictionary>();

            graphDictActual.Add(new Program.GraphDictionary(){Node=null, childNode = "hit" });
            graphDictActual.Add(new Program.GraphDictionary(){Node="hit",childNode="hot"});
            graphDictActual.Add(new Program.GraphDictionary(){Node="hot",childNode="dot"});
            graphDictActual.Add(new Program.GraphDictionary(){Node="hot",childNode="lot"});
            graphDictActual.Add(new Program.GraphDictionary(){Node="hot",childNode="pot"});
            graphDictActual.Add(new Program.GraphDictionary(){Node="dot",childNode="dog"});
            graphDictActual.Add(new Program.GraphDictionary(){Node="lot",childNode="log"});
            graphDictActual.Add(new Program.GraphDictionary() { Node = "lot", childNode = "los" });
            graphDictActual.Add(new Program.GraphDictionary(){Node="pot",childNode="pol"});
            graphDictActual.Add(new Program.GraphDictionary(){Node="dog",childNode="cog"});

            List<Program.GraphDictionary> graphDictCurrent = Program.BuldGraphFlyToElephant(dictionary, "hit", "cog");

            Assert.IsTrue(graphDictCurrent.SequenceEqual(graphDictActual, new GraphDictionaryEqualityComparer()));

            graphDictCurrent = Program.BuldGraphFlyToElephant(dictionary, "zzz", "cog");

            Assert.AreEqual(null, graphDictCurrent);

            graphDictCurrent = Program.BuldGraphFlyToElephant(null, "hit", "cog");
            
            Assert.AreEqual(null, graphDictCurrent);

        }

        public class GraphDictionaryEqualityComparer : IEqualityComparer<Program.GraphDictionary>
        {
            public bool Equals(Program.GraphDictionary x, Program.GraphDictionary y)
            {
                if (object.ReferenceEquals(x, y)) return true;

                if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;

                return x.childNode == y.childNode && x.Node == y.Node;
            }

            public int GetHashCode(Program.GraphDictionary obj)
            {
                if (object.ReferenceEquals(obj, null)) return 0;

                int hashCodechildNode = obj.childNode == null ? 0 : obj.childNode.GetHashCode();
                int hasCodeNode = obj.Node.GetHashCode();

                return hashCodechildNode ^ hasCodeNode;
            }
        }

        [TestMethod]
        public void CheckDictionaryCharArray()
        {
            List<string> dictionary = new List<string>() { "рот", "тот", "мот", "пот" };
            CollectionAssert.AreEqual(new char[] { 'д','м','о', 'п', 'р', 'т'}, Program.DictionaryCharArray(dictionary,"дот"));
        }
        
        [TestMethod]
        public void CheckReplaceOneLetter()
        {
            string input = "сок";
            int index = 0;
            char letter = 'к';

            Assert.AreEqual("кок", Program.ReplaceOneLetter(input, index, letter));

            index = 8;
            Assert.AreEqual("сок", Program.ReplaceOneLetter(input, index, letter));

            input = null;
            Assert.AreEqual(null, Program.ReplaceOneLetter(input, index, letter));
        }

        [TestMethod]
        public void CheckFilterdictionaryArray()
        {
            List<string> dict = new List<string> { "сок", "КОТ", "Ток", "Работа", "Чушь", "ДЛЯ" };
            string firstWord = "сок";
            string endWord = "для";

            CollectionAssert.AreEqual(new List<string>() { "кот", "ток" }, Program.FilterDictionaryArray(dict, firstWord, endWord));
        }
        [TestMethod]

        public void  CheckTransformationFlyToElephant()
        {
            List<string> dict = new List<string> {"кот", "тон", "нота", "коты", "рот", "рота", "тот"};
            string firstWord = "кот";
            string endWord = "тон";
            List<string> actual = new List<string> {"кот", "тот", "тон"};
            List<string> current = Program.TransformationFlyToElephant(dict,firstWord,endWord);
            
            CollectionAssert.AreEqual(actual, current);

            current = Program.TransformationFlyToElephant(null, firstWord, endWord);

            Assert.AreEqual(null, current);

            current = Program.TransformationFlyToElephant(dict, null, endWord);

            Assert.AreEqual(null, current);

            current = Program.TransformationFlyToElephant(dict, firstWord, null);

            Assert.AreEqual(null, current);

            current = Program.TransformationFlyToElephant(dict, "сок", endWord);

            CollectionAssert.AreNotEqual(actual,current);

        }
    }
}
