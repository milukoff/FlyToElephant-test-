using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FlyToElephant
{
    public class Program 
    {
            static void Main()
            {
                Console.WriteLine("Как из мухи сделать слона");
                Console.WriteLine("Введите путь к словарю");
                string FileDictionary = Console.ReadLine();
                if (!File.Exists(FileDictionary))
                {
                    Console.WriteLine("Словарь не найден!");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                Console.WriteLine("Введите путь к списку исходных и конечных слов");
                string FileFirstEndWords = Console.ReadLine();
                if (!File.Exists(FileFirstEndWords))
                {
                    Console.WriteLine("Файл, содержащий список исохдного и конечного слов не найден");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                List<string> dictionaryArray = File.ReadAllLines(FileDictionary).ToList();
                List<string> FirstEndWordArray = File.ReadAllLines(FileFirstEndWords).ToList();
                if (FirstEndWordArray.Count>=2)
                {
                    string FirstWord = FirstEndWordArray[0];
                    string EndWord = FirstEndWordArray[1];
                    FirstWord = FirstWord.Trim().ToLower();
                    EndWord = EndWord.Trim().ToLower();
                    // Фильтруем словарь (убираем слова равные начальному и конечному и только по длине равные начальному)
                    // приводим все слова в нижний регистр, удаляя пробелы вокруг слов
                    dictionaryArray = FilterDictionaryArray(dictionaryArray, FirstWord, EndWord);
                    //Строим граф по словарю из слов, которые могут содержать цепочки трансформации мухи в слона
                    List<GraphDictionary> GraphFlyToElephant = BuldGraphFlyToElephant(dictionaryArray, FirstWord, EndWord);
                    //Из графа выбираем нашу цепочку трансформации
                    List<string> ResultArray = BuildChainArray(GraphFlyToElephant, EndWord);
                    if (ResultArray==null || ResultArray.Count()==0)
                    {
                        Console.WriteLine("Цепочка трансформации не найдена");
                    }
                    else
                    {
                        foreach (var item in ResultArray)
                        {
                            Console.WriteLine();
                            Console.Write(item);
                            Console.Write("\n");
                        }
                    }
                    Console.ReadLine();
                }
            }
            
            /// <summary>
            /// Класс, содержащий в себе структуру графа 
            /// </summary>
            public class GraphDictionary
            {
                public string Node {get; set;}
                public string childNode { get; set; } 
            }
            /// <summary>
            /// Функция заменяет символ в заданной строке по указанному индексу позиции
            /// </summary>
            /// <param name="input">исходное слово</param>
            /// <param name="index">позиция для замены</param>
            /// <param name="ch">символ, на который необходимо заменить символ по позиции</param>
            /// <returns>Возвращает измененую строку, в в случае если строка не пустуя и индекс не более размера строки,
            /// ту же строку, в случае если индекс больше размера строки
            /// и null в случае если строка пустая
            /// </returns>
            public static string ReplaceOneLetter(string input, int index, char ch)
            {
                if (string.IsNullOrEmpty(input))
                    return null;
                else if (input.Length < (index + 1))
                    return input;
                else
                {
                    char[] chars = input.ToCharArray();
                    chars[index] = ch;
                    return new string(chars);
                }
            }
            /// <summary>
            /// Функция составляет список из всех встречающихся букв в словаре и в конечном слове
            /// </summary>
            /// <param name="dictionary">словарь</param>
            /// <param name="endWord">конечное слово</param>
            /// <returns>Возвращает массив уникальных символов встречающихся в словаре и в конечном слове</returns>
            public static char[] DictionaryCharArray(List<string> dictionary, string endWord)
            {
                List<string> dict = new List<string>(dictionary);
                dict.Add(endWord);
                var chars = dict.SelectMany(s => s.ToCharArray());
                char[] distinctChars = chars.Distinct().OrderBy(s=>s).ToArray();
                return distinctChars;
            }
            
            /// <summary>
            /// Функция фильтрует словарь, оставляя в нем только слова по размеру равные начальному и не содержащие слова, равные начальному и конечному
            /// При этому у каждого слова удаляет лишние пробелы
            /// и приводит к нижнему регистру
            /// </summary>
            /// <param name="dictionaryArray">исходный словарь</param>
            /// <param name="firstWord">Первое слово для исключения</param>
            /// <param name="endWord">Второе слово для исключения</param>
            /// <returns>Возвращает List string - отфильтрованный и приведенный к нижнему регистру словарь</returns>
            public static List<string> FilterDictionaryArray(List<string> dictionaryArray, string firstWord, string endWord)
            {
                return dictionaryArray
                        .Select(w => w.Trim().ToLower())
                        .Where(word => word.Length == firstWord.Trim().ToLower().Length && word != firstWord.Trim().ToLower() && word != endWord.Trim().ToLower())
                        .ToList();
            }
            
            /// <summary>
            /// Функция возвращает цепочку элементов графа от начального до конечного, где конечный элемент цепочки равен искоемому
            /// </summary>
            /// <param name="graphDictionary">словарь в виде структуры графа</param>
            /// <param name="endWord">Искуемоме слово</param>
            /// <returns>Возвращает цепочку в виде List string от начального до конечного элемента </returns>
            public static List<string> BuildChainArray(List<GraphDictionary> graphDictionary, string endWord)
            {
                List<string> result = new List<string>();
                if (string.IsNullOrEmpty(endWord)) return null;
                if (graphDictionary == null) return null;

                var item = graphDictionary.FirstOrDefault(s => s.childNode == endWord);
                if (item != null)
                {
                    result.Add(endWord);
                    while (item != null && !string.IsNullOrEmpty(item.Node)) 
                    {
                        result.Add(item.Node);
                        item = graphDictionary.FirstOrDefault(s => s.childNode == item.Node);
                    }
                }
                result = new List<string>(result.Reverse<string>().ToList());
                return result;
            }
            /// <summary>
            /// Функция возвращает результат поиска в виде структуры, содержащий в себе все возможные цепочки из словаря до момента "найден конечный элемент"
            /// </summary>
            /// <param name="dictionary">словарь </param>
            /// <param name="beginWord">начальное слово </param>
            /// <param name="endWord">конечное слово</param>
            /// <returns>Возвращает результат поиска слов для трансформации мухи в слона в виде графа, одна из ребер которого содержит цепочку трансформации</returns>
            public static List<GraphDictionary> BuldGraphFlyToElephant(List<string> dictionary, string beginWord, string endWord)
            {

                if (string.IsNullOrEmpty(beginWord) || string.IsNullOrEmpty(endWord) || dictionary == null || dictionary.Count == 0)
                {
                    return null;
                }
               
                Queue<String> queue = new Queue<string>(); //Очередь для проссмотра 
                
                List<GraphDictionary> graphDict = new List<GraphDictionary>();

                queue.Enqueue(beginWord);//Кладем в очередь начальное слово

                graphDict.Add(new GraphDictionary() { Node = null, childNode = beginWord });//В граф словаря кладем первый элемент

                char[] dictCharArr = DictionaryCharArray(dictionary, endWord);//Для уменьшение количества проходов по словарю составляем список возможных симоволов, встречающихся в словаре
                while (queue.Count > 0)
                {
                    int count = queue.Count;
 
                    for (int i = 0; i < count; i++) //Методом поиска в ширину по графу проссматриваем каждый элемент
                    {
                        String  currentWord = queue.Dequeue();

                        for (int j = 0; j < currentWord.Length; j++)
                        {
                            //Подменяем каждую букву в текущем элементе на возможный символ (есть в словаре)
                            for (int chArrIndex = 0; chArrIndex < dictCharArr.Length; chArrIndex++)
                            {
                                if (!dictCharArr[chArrIndex].Equals(currentWord[j])) //Пропускаем если символ тот же
                                {
                                    String transformedWord = ReplaceOneLetter(currentWord, j, dictCharArr[chArrIndex]);
                                    if (transformedWord.Equals(endWord)) //Если получившееся слово = конечному то возвращаем результат
                                    {
                                        graphDict.Add(new GraphDictionary() { Node = currentWord, childNode = transformedWord });

                                        return graphDict;
                                    }

                                    if (dictionary.Contains(transformedWord)) //Если получившееся слово есть словаре то добавляем в очередь, и удаляем из словаря
                                    {
                                        queue.Enqueue(transformedWord);
                                        dictionary.Remove(transformedWord);
                                        graphDict.Add(new GraphDictionary() { Node = currentWord, childNode = transformedWord });
                                        //Добавляем слово в структуру графа
                                    }
                                }
                            }
                        }
                    }
                }
                return null;
            }
            /// <summary>
            /// Функция "превращает" муху в слона
            /// </summary>
            /// <param name="dictionary">словарь </param>
            /// <param name="beginWord">начальное слово </param>
            /// <param name="endWord">конечное слово</param>
            /// <returns>Возвращает результат поиска слов для трансформации мухи в слона в виде цепочки превращения</returns>
            public static List<string> TransformationFlyToElephant(List<string> dictionary, string beginWord, string endWord)
            {
                if (string.IsNullOrEmpty(beginWord) || string.IsNullOrEmpty(endWord) || dictionary == null || dictionary.Count == 0)
                {
                    return null;
                }
                else
                {
                    List<GraphDictionary> graphFlyToElephant = BuldGraphFlyToElephant(dictionary, beginWord, endWord);
                    return BuildChainArray(graphFlyToElephant, endWord);
                }
                
            }
        }

}
