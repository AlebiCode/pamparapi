using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Crossword
{
    [System.Serializable]
    public static class CrosswordGenerator
    {
        const string JSON_PATH = "C:/Users/Borghesi Alessandro/Desktop/TEST.json";
        //const char DEFAULT_CHAR = '.';
        public const int MAX_ROW = 7, MAX_COLUMN = 9;
        static int TARGET_NUMBER_OF_WORDS = 10;
        const int MAX_ITERATIONS = 20;   //n. massimo di tentativi per trovare una parola inseribile.

        static int iterations;  //contatore iterazioni per trovare una parola inseribile. Viene azzerato se una parola viene trovata.

        static string[,] dictionary = null;

        static GenerationInfo generationInfo;

        static List<int> usedDictionaryIDs = new List<int>();

        static List<string> failedWords = new List<string>();


        private static int DictionaryLenght => dictionary.GetLength(0);

        /// <summary>
        /// Ritorna le informazioni del crossword che genera.
        /// </summary>
        /// <returns></returns>
        public static GenerationInfo GenerateWords()
        {
            if (dictionary == null)
                InizializzaDizionario();

            InizializzaVariabili();

            AddFirstWordToMatrix();

            while (generationInfo.wordInfoList.Count < TARGET_NUMBER_OF_WORDS)
            {
                if (iterations > MAX_ITERATIONS)
                {
                    Debug.LogWarning("!! !! !! Raggiunto numero iterazioni massimo. !! !! !!");
                    break;
                }
                AddWordToMatrix(true);
            }
            //--------DEBUGGING------------
            string debugString = "";
            foreach (WordInfo w in generationInfo.wordInfoList)
                debugString += ("<" + w.word + "> ");
            debugString += "\nCi sono stati " + generationInfo.wordInfoList.Count + " inserimenti riusciti.\nCi sono stati " + failedWords.Count + " inserimenti falliti\n";
            debugString += "x Massimi e minimi: " + generationInfo.xMax + "," + generationInfo.xMin + "\ty Massimi e minimi: " + generationInfo.yMax + "," + generationInfo.yMin + "\n\n";
            Debug.Log(debugString);
            //PrintMatrixFromList();
            //--------END-DEBUGGING--------
            return generationInfo;
        }
            

        private static void InizializzaVariabili()
        {
            iterations = 0;
            generationInfo = new GenerationInfo();
            generationInfo.wordInfoList = new List<WordInfo>();
            usedDictionaryIDs.Clear();
            failedWords.Clear();
        }

        public static void InizializzaDizionario()
        {
            try
            {
                TextAsset jsonFile = Resources.Load<TextAsset>("Dictionaries/TEST");
                Dizionario cc;//= JsonUtility.FromJson<CoppiaClass>(jsonFile.ToString());
                cc = JsonConvert.DeserializeObject<Dizionario>(jsonFile.ToString());
                dictionary = cc.words;
                Debug.Log(dictionary[0,0] + "," + dictionary[0,1] + " arr size: " + dictionary.GetLength(0) );
            }
            catch
            {
                Debug.LogWarning("File json non trovato, o non leggibile. Controllare presenza e corretteza del file al percorso " + JSON_PATH);
            }
        }

        private static void AddFirstWordToMatrix()
        {
            int id = Random.Range(0, DictionaryLenght);
            string word = dictionary[id,0];   

            bool horizontal = Random.Range(0, 2) == 1;

            //Debug.Log("Prima parola: <" + word + ">, orientamento orizzontale=" + horizontal);

            generationInfo.wordInfoList.Add(CreateWordInfo(word, dictionary[id, 1], 0,0,horizontal));
            usedDictionaryIDs.Add(id);
            if (horizontal)
                generationInfo.xMax = word.Length - 1;
            else
                generationInfo.yMax = word.Length - 1;
        }

        private static void AddWordToMatrix(bool uniqueWords = false)  //!!!! VISTO CHE E' FATTO MALE, SE uniqueWords = false, PUO' ANCHE RIPROVARE CON PAROLE CHE ERANO FALLITE!!
        {
            WordInfo newWord;

            int id = Random.Range(0, DictionaryLenght);
            if (uniqueWords)
                while (usedDictionaryIDs.Contains(id))
                {
                    id = Random.Range(0, DictionaryLenght);
                    if (DictionaryLenght <= usedDictionaryIDs.Count)
                    {
                        Debug.LogWarning("!! !! !! !! Il dizionario non contiene abbastanza elementi per usare il numero di parole richieste differenti. Questo potrebbe essere stato causato da tentativi falliti prematuri, che potrebbero invece funzionare una volta aggiunte più parole.");
                        TARGET_NUMBER_OF_WORDS = generationInfo.wordInfoList.Count;
                        return;
                    }
                }

            newWord.word = dictionary[id,0];
            usedDictionaryIDs.Add(id);

            //Console.WriteLine("Trying to add <" + newWord.word + ">");
            //FIX!!!!!!!!!!!!!!! AGGIUNGI RANDOMIZZAZIONE NELL'ORDINE DI CONTROLLO!!!! int[] randomIndexOrder = RandomOrderArray(newWord.word.Length);
            for (int charIndex = 0; charIndex < newWord.word.Length; charIndex++)   //per ogni lettera della nuova parola...
            {
                //Console.WriteLine("---Controllo per <" + newWord.word[charIndex] + ">");
                foreach (WordInfo tangentWordInfo in generationInfo.wordInfoList)              //per ogni parola già presente
                {
                    //Console.WriteLine("------Controllo con <" + tangentWordInfo.word + "> alla posizione (" + tangentWordInfo.x + "," + tangentWordInfo.y + ")");
                    for (int i = 0; i < tangentWordInfo.word.Length; i++)       //per ogni lettera della parola già presente
                    {
                        if (newWord.word[charIndex] == tangentWordInfo.word[i])
                        {
                            //Console.WriteLine("---------EQUIVALENZA <" + newWord.word[charIndex] + "> alla posizione " + i + " della parola " + tangentWordInfo.word + ".");
                            //ora controllo se posso posizionare.
                            if (tangentWordInfo.horizontal)
                            {
                                newWord.x = tangentWordInfo.x + i;
                                newWord.y = tangentWordInfo.y - charIndex;
                                newWord.horizontal = !tangentWordInfo.horizontal;

                                int wordMinCoord = newWord.y;                                   //FIX SEMPLIFICA?
                                int wordMaxCoord = newWord.y + newWord.word.Length - 1;

                                if (Mathf.Max(generationInfo.yMax, wordMaxCoord) - Mathf.Min(generationInfo.yMin, wordMinCoord) < MAX_ROW)   //la parola può stare nei limiti di grandezza.
                                {
                                    bool valid = true;
                                    foreach (WordInfo w in generationInfo.wordInfoList)
                                    {
                                        //se la w è orizzontale
                                        if (w.horizontal)
                                        {
                                            if (newWord.x >= w.x && newWord.x <= w.x + w.word.Length - 1)// se w ha nel suo x range la x della nuova parola
                                            {
                                                if (w.y == wordMinCoord - 1 || w.y == wordMaxCoord + 1)     //controllo che nessuna parola sia subito prima o subito dopo i bordi della nuova parola
                                                {
                                                    valid = false;
                                                    break;
                                                }
                                                else if (w.y >= wordMinCoord || w.y <= wordMaxCoord)         //se inoltre w.y è nel y range della nuova parola (quindi le 2 nuove parole si incrociano)
                                                {
                                                    if (newWord.x - w.x < w.word.Length && newWord.x - w.x >= 0 &&
                                                        w.y - newWord.y < newWord.word.Length && w.y - newWord.y >= 0)
                                                        if (w.word[newWord.x - w.x] != newWord.word[w.y - newWord.y])   //controllo se il char nel punto di incrocio coincide
                                                        {
                                                            valid = false;
                                                            break;
                                                        }
                                                }
                                            }                                                           //non ha nel suo x range la x della nuova parola
                                            else
                                            {

                                                if (w.x == newWord.x + 1 || (w.x + w.word.Length) == newWord.x)     //controllo che nessuna parola finisca subito prima o parta subito dopo
                                                {
                                                    valid = false;
                                                    break;
                                                }
                                            }

                                        }
                                        else //se invece w è verticale
                                        {
                                            if (w.y + w.word.Length - 1 >= newWord.y || w.y <= newWord.y + newWord.word.Length - 1) //se hanno delle y in comune
                                                if (newWord.x >= w.x - 1 && newWord.x <= w.x + 1)                                       //controllo se sono parallele adiacenti o parallele tangenti (clippate)
                                                {
                                                    valid = false;
                                                    break;
                                                }
                                        }
                                    }
                                    if (valid)
                                    {
                                        newWord.clue = dictionary[id, 1];
                                        generationInfo.wordInfoList.Add(newWord);
                                        generationInfo.yMax = Mathf.Max(generationInfo.yMax, wordMaxCoord);
                                        generationInfo.yMin = Mathf.Min(generationInfo.yMin, wordMinCoord);
                                        iterations = 0;
                                        //Console.WriteLine("------------Parola <" + newWord.word + "> aggiunta con successo.");
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                newWord.x = tangentWordInfo.x - charIndex;
                                newWord.y = tangentWordInfo.y + i;
                                newWord.horizontal = !tangentWordInfo.horizontal;

                                int wordMinCoord = newWord.x;                                   //FIX SEMPLIFICA?
                                int wordMaxCoord = newWord.x + newWord.word.Length - 1;

                                if (Mathf.Max(generationInfo.xMax, wordMaxCoord) - Mathf.Min(generationInfo.xMin, wordMinCoord) < MAX_COLUMN)   //la parola può stare nei limiti di grandezza.
                                {
                                    bool valid = true;
                                    foreach (WordInfo w in generationInfo.wordInfoList)
                                    {
                                        //se la w non è orizzontale
                                        if (!w.horizontal)
                                        {
                                            if (newWord.y >= w.y && newWord.y <= w.y + w.word.Length - 1)// se w ha nel suo x range la x della nuova parola
                                            {
                                                if (w.x == wordMinCoord - 1 || w.x == wordMaxCoord + 1)     //controllo che nessuna parola sia subito prima o subito dopo i bordi della nuova parola
                                                {
                                                    valid = false;
                                                    break;
                                                }
                                                else if (w.x >= wordMinCoord || w.x <= wordMaxCoord)         //se inoltre w.y è nel y range della nuova parola (quindi le 2 nuove parole si incrociano)
                                                {
                                                    if (newWord.y - w.y < w.word.Length && newWord.y-w.y >= 0 &&
                                                        w.x - newWord.x < newWord.word.Length && w.x - newWord.x >= 0)
                                                        if (w.word[newWord.y - w.y] != newWord.word[w.x - newWord.x])   //controllo se il char nel punto di incrocio coincide
                                                        {
                                                            valid = false;
                                                            break;
                                                        }
                                                }
                                            }                                                           //non ha nel suo x range la x della nuova parola
                                            else
                                            {
                                                if (w.y == newWord.y + 1 || (w.y + w.word.Length) == newWord.y)     //controllo che nessuna parola finisca subito prima o parta subito dopo
                                                {
                                                    valid = false;
                                                    break;
                                                }
                                            }

                                        }
                                        else //se invece w è verticale
                                        {
                                            if (w.x + w.word.Length - 1 >= newWord.x || w.x <= newWord.x + newWord.word.Length - 1) //se hanno delle y in comune
                                                if (newWord.y >= w.y - 1 && newWord.y <= w.y + 1)                                       //controllo se sono parallele adiacenti o parallele tangenti (clippate)
                                                {
                                                    valid = false;
                                                    break;
                                                }
                                        }
                                    }
                                    if (valid)
                                    {
                                        newWord.clue = dictionary[id, 1];
                                        generationInfo.wordInfoList.Add(newWord);
                                        generationInfo.xMax = Mathf.Max(generationInfo.xMax, wordMaxCoord);
                                        generationInfo.xMin = Mathf.Min(generationInfo.xMin, wordMinCoord);
                                        iterations = 0;
                                        //Console.WriteLine("------------Parola <" + newWord.word + "> aggiunta con successo.");
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //se arrivo qua, non è stato possibile inserire la parola che ho scelto.
            iterations++;
            failedWords.Add(newWord.word);
            //Console.WriteLine("------------FALLITO INSERIMENTO <" + newWord.word + ">.");
        }

        private static void PrintMatrixFromList()
        {
            try
            {
                string outputStr = "";
                char[][] output = new char[MAX_COLUMN][];
                for (int i = 0; i < MAX_COLUMN; i++)
                {
                    output[i] = new char[MAX_ROW];
                    for (int j = 0; j < MAX_ROW; j++)
                    {
                        output[i][j] = '-';
                    }
                }

                foreach (WordInfo wordInfo in generationInfo.wordInfoList)
                {
                    for (int i = 0; i < wordInfo.word.Length; i++)
                    {
                        if (wordInfo.horizontal)
                            output[-generationInfo.xMin + wordInfo.x + i][-generationInfo.yMin + wordInfo.y] = wordInfo.word[i];
                        else
                            output[-generationInfo.xMin + wordInfo.x][-generationInfo.yMin + wordInfo.y + i] = wordInfo.word[i];
                    }
                }

                for (int i = 0; i < MAX_ROW; i++)
                {
                    for (int j = 0; j < MAX_COLUMN; j++)
                    {
                        outputStr += output[j][i].ToString() + "\t";
                    }
                    outputStr += "\n";
                }
                Debug.Log(outputStr.ToUpper());
            }
            catch
            {
                //perchè non tiene in considerazione che le cooridnate possono sbordare da 0 a MAX_COLUMN o da 0 a MAX_ROW!!! amen!!!
                Debug.LogWarning("Ho avuto problemi a stampare la matrice per il debugging. Chissenefrega lol non ho voglia di vedere perchè non funziona");
            }
        }

        static WordInfo CreateWordInfo(string word, string clue, int x, int y, bool horizontal/*, int dictionaryId*/)
        {
            WordInfo info = new WordInfo();
            info.word = word;
            info.clue = clue;
            info.x = x;
            info.y = y;
            info.horizontal = horizontal;
            //info.dictionaryID = dictionaryId;
            return info;
        }

    }
    public struct WordInfo
    {
        public string word;
        public string clue;
        public int x, y;
        public bool horizontal;

        //public int dictionaryID;
    }

    public struct GenerationInfo
    {
        public List<WordInfo> wordInfoList;
        public List<(int,int)> incroci;
        public int xMax, xMin, yMax, yMin;

    }

    [System.Serializable]
    public class Dizionario
    {
        public string[,] words;
    }

}

