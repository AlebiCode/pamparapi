using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crossword
{
    public class CrosswordLogic : MonoBehaviour
    {
        //COSTANTI
        private static Color
            standardColor = Color.white,
            wordSelectionColor = Color.cyan,
            tasselloSelectionColor = Color.yellow,
            startingLetterColor = Color.grey,
            correctWordColor = Color.green;

        //VARIABILI
        private static CrosswordLogic instance;

        [SerializeField] private Tassello tasselloPrefab;
        [SerializeField] private GameObject crosswordParent;
        [SerializeField] private InputField inputField;
        [SerializeField] private Text pamparapiText;

        private List<WordObject> wordObjects = new List<WordObject>();
        private WordObject currentSelectedWordObject = null;
        private Tassello currentSelectedTassello = null;


        //PROPERTIES
        public static CrosswordLogic Instance => instance;
        [SerializeField] private Tassello TasselloPrefab => tasselloPrefab;
        [SerializeField] private GameObject CrosswordPanel => crosswordParent;
        public WordObject CurrentSelectedWordObject
        {
            set
            {
                if (currentSelectedWordObject != null)
                    if(!currentSelectedWordObject.Completed)
                        foreach (Tassello tassello in currentSelectedWordObject.tasselli)
                            SetTasselloColor(tassello, standardColor);

                currentSelectedWordObject = value;

                if(currentSelectedWordObject != null)
                    foreach (Tassello tassello in currentSelectedWordObject.tasselli)
                        SetTasselloColor(tassello, wordSelectionColor);

                //------debug
                if(currentSelectedWordObject != null)
                    pamparapiText.text = currentSelectedWordObject.GetWordInfo.word;
            }
            get { return currentSelectedWordObject; }
        }
        public Tassello CurrentSelectedTassello 
        {
            set
            {
                if (CurrentSelectedTassello)
                    if(CurrentSelectedTassello.wordObjectParents[0] == currentSelectedWordObject || CurrentSelectedTassello.wordObjectParents[1] == currentSelectedWordObject)
                        SetTasselloColor(currentSelectedTassello, wordSelectionColor);
                    else
                        SetTasselloColor(currentSelectedTassello, standardColor);
                currentSelectedTassello = value;
                if (CurrentSelectedTassello)
                    SetTasselloColor(currentSelectedTassello, tasselloSelectionColor);
            }
            get { return currentSelectedTassello; }
        }

        private bool LevelCompleted {
            get {
                foreach (WordObject wordObject in wordObjects)
                    if (!wordObject.Completed)
                        return false;
                return true;
            }
        }

        //METODI UNITY

        private void Awake()
        {
            InstantiateSelf();
        }

        void Start()
        {
            GenerateCrossword();
        }

        private void Update()
        {

        }

        #region METODI

        private void InstantiateSelf()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void GenerateCrossword()
        {
            ResetValuesAndGameBoard();
            GenerationInfo generationInfo = CrosswordGenerator.GenerateWords();

            WordInfo[] wordInfos = generationInfo.wordInfoList.ToArray();
            float tasselloSize = tasselloPrefab.GetComponent<RectTransform>().rect.width;
            Vector3 offSet = -new Vector3((generationInfo.xMax + generationInfo.xMin) / 2f, -(generationInfo.yMax + generationInfo.yMin) / 2f, 0) * tasselloSize; ;
            List<(int, int, Tassello)> insertions = new List<(int, int, Tassello)>();
            foreach (WordInfo wordInfo in wordInfos)
            {
                WordObject newWordobject = new WordObject(wordInfo);
                
                wordObjects.Add(newWordobject);
                AddTasselliToGameWord(newWordobject, tasselloSize, offSet, insertions);
            }

            PlaceStartingLetters();
        }

        private void ResetValuesAndGameBoard()
        {
            CurrentSelectedWordObject = null;
            CurrentSelectedTassello = null;
            wordObjects.Clear();
            foreach (Transform child in crosswordParent.GetComponentInChildren<Transform>())
                Destroy(child.gameObject);

            pamparapiText.text = "Maybe you should try getting a job";
        }

        //NON MI PIACE COME CONTROLLA SE E' GIA STATO PIAZZATO UN TASSELLO, MA FUNZIONA COMUNQUE LOL
        private void AddTasselliToGameWord(WordObject wordObject, float tasselloSize, Vector3 offSet, List<(int, int, Tassello)> insertions = null)
        {
            for (int i = 0; i < wordObject.GetWordInfo.word.Length; i++)
            {
                Vector2Int coords;
                if(wordObject.GetWordInfo.horizontal)
                    coords = new Vector2Int(wordObject.GetWordInfo.x + i, wordObject.GetWordInfo.y);
                else
                    coords = new Vector2Int(wordObject.GetWordInfo.x, wordObject.GetWordInfo.y + i);

                Tassello tassello = null;
                //Controllo se un tassello lì era già posizionato. Se sì assegno a tassello quel valore
                foreach ((int, int, Tassello) triple in insertions)
                {
                    if (triple.Item1 == coords.x && triple.Item2 == coords.y)
                    {
                        tassello = triple.Item3;
                        break;
                    }
                }

                //Assegnazione valori nei 2 casi diversi
                if (tassello == null)
                {
                    tassello = GameObject.Instantiate(TasselloPrefab, CrosswordPanel.transform);
                    tassello.transform.localPosition += (new Vector3(coords.x,-coords.y,0) * tasselloSize) + offSet;
                    tassello.wordObjectParents[0] = wordObject;
                    insertions.Add((coords.x, coords.y, tassello));
                }
                else
                {
                    tassello.wordObjectParents[1] = wordObject;
                }
                wordObject.tasselli[i] = tassello;
            }
        }

        /// <summary>
        /// Piazza (idealmente) una lettera per parola. Come è scritto male LOL!!
        /// </summary>
        private void PlaceStartingLetters()
        {
            foreach (WordObject wordObject in wordObjects)
            {
                int placement = Random.Range(0, wordObject.GetWordInfo.word.Length);
                if (wordObject.tasselli[placement].locked)
                    placement = FindNextFreeTasselloIndex(wordObject);
                wordObject.tasselli[placement].SetLettera(wordObject.GetWordInfo.word[placement]);
                wordObject.tasselli[placement].locked = true;
                SetTasselloColor(wordObject.tasselli[placement], startingLetterColor, false);
            }
        }

        public void OnWordSelection(WordObject wordObject)
        {
           if (!wordObject.Completed)
            {
                CurrentSelectedWordObject = wordObject;
                CurrentSelectedTassello = FindNextFreeTassello(CurrentSelectedWordObject);
                if(CurrentSelectedTassello == null)
                    CurrentSelectedTassello = wordObject.tasselli[wordObject.tasselli.Length-1];
            }
        }
        /// <summary>
        /// Trova prossimo tassello
        /// </summary>
        /// <param name="wordObject">wordobject in cui cercare il tassello</param>
        /// <returns></returns>
        private Tassello FindNextFreeTassello(WordObject wordObject)
        {
            foreach (Tassello tassello in wordObject.tasselli)
            {
                if (tassello.Lettera == ' ')
                    return tassello;
            }
            return null;
        }

        private int FindNextFreeTasselloIndex(WordObject wordObject)
        {
            for(int i = 0; i < wordObject.tasselli.Length; i++)
            {
                if (wordObject.tasselli[i].Lettera == ' ')
                    return i;
            }
            return -1;
        }

        public void OnBackSpacePress()
        {
            RemoveLetter();
        }

        private void AddLetter(Tassello tassello, char lettera)
        {
            if (tassello)
            {
                Debug.Log("AddLetter called " + tassello.wordObjectParents[0] + tassello.wordObjectParents[1]);
                tassello.SetLettera(lettera);
                foreach (WordObject wordObject in tassello.wordObjectParents)
                    if(wordObject != null) {
                        if (wordObject.CheckWordCompletion())
                        {
                            Debug.Log("Word was completed correctly");
                            foreach(Tassello tassello2 in wordObject.tasselli)
                                SetTasselloColor(tassello2, correctWordColor, false);
                            if (LevelCompleted)
                            {
                                Debug.Log("GAME HAS ENDED!!!!!");
                                GameCompletion();
                            }
                            else
                            {
                                CurrentSelectedWordObject = GetNotCompletedWord();
                                CurrentSelectedTassello = FindNextFreeTassello(CurrentSelectedWordObject);
                            }
                        }
                        else if (wordObject == currentSelectedWordObject)
                        {
                            //cambio solo se c'è un altro tassello libero (ovvero non era l'ultima lettera che ho inserito)
                            Tassello nextFreeTassello = FindNextFreeTassello(CurrentSelectedWordObject);
                            if(nextFreeTassello)
                                CurrentSelectedTassello = nextFreeTassello;
                            else
                            {
                                //Comportamento "hai riempito la parola ma è scorretta"
                                //METTI QUALCHE ANIMAZIONCINA!
                                Debug.Log("La lettera è sbagliata!!");
                                CurrentSelectedTassello.SetLettera(' ');
                            }
                        }
                    }
            }
        }

        private void RemoveLetter()
        {
            for (int i = currentSelectedWordObject.tasselli.Length - 1; i >= 0; i--)
            {
                if (currentSelectedWordObject.tasselli[i].Lettera != ' ' && !currentSelectedWordObject.tasselli[i].locked)
                {
                    currentSelectedWordObject.tasselli[i].SetLettera(' ');
                    CurrentSelectedTassello = FindNextFreeTassello(currentSelectedWordObject);
                    break;
                }
            }
        }

        public WordObject GetNotCompletedWord()
        {
            foreach(WordObject wordObject in wordObjects)
                if(!wordObject.Completed)
                    return wordObject;
            return null;
        }

        private void GameCompletion()
        {
            currentSelectedTassello = null;
            currentSelectedWordObject = null;
            pamparapiText.text = "GGWP";

            GameManager.instance.SoftCurrency += 1000;
            GameManager.instance.Fullness = Mathf.Max(GameManager.instance.Fullness - 0.1f, 0);
            GameManager.instance.SaveDataToJson();
        }

        public void CallLetterInsert(int asciiCode)
        {
            //gli acsii per le lettere maiuscole dalla A alla Z sono da 65 a 90
            AddLetter(currentSelectedTassello, (char)asciiCode);
        }

        public void CallLetterRemoval()
        {
            RemoveLetter();
        }

        private void SetTasselloColor(Tassello tassello, Color color, bool checkIfLocked = true)
        {
            if (checkIfLocked && tassello.locked)
               return;
            tassello.GetComponent<Image>().color = color;
        }

        #endregion
    }

}
