using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crossword{
    public class CrosswordLogic : MonoBehaviour
    {
        //VARIABILI
        private static CrosswordLogic instance;

        [SerializeField] private Tassello tasselloPrefab;
        [SerializeField] private GameObject crosswordParent;
        [SerializeField] private UnityEngine.UI.InputField inputField;

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
                if (CurrentSelectedWordObject != null)
                    foreach (Tassello tassello in CurrentSelectedWordObject.tasselli)
                        tassello.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                currentSelectedWordObject = value;
                if(CurrentSelectedWordObject != null)
                    foreach (Tassello tassello in CurrentSelectedWordObject.tasselli)
                        tassello.GetComponent<UnityEngine.UI.Image>().color = Color.blue;
            }
            get { return currentSelectedWordObject; }
        }
        public Tassello CurrentSelectedTassello 
        {
            set
            {
                if (CurrentSelectedTassello)
                    if(CurrentSelectedTassello.wordObjectParents[0] == currentSelectedWordObject || CurrentSelectedTassello.wordObjectParents[1] == currentSelectedWordObject)
                        currentSelectedTassello.GetComponent<UnityEngine.UI.Image>().color = Color.blue;
                    else
                        currentSelectedTassello.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                currentSelectedTassello = value;
                if (CurrentSelectedTassello)
                    currentSelectedTassello.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
            }
            get { return currentSelectedTassello; }
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

        public void DEBUGLetterInsert() //FIX!!!! REMOVE FROM FINAL BUILD!!
        {
            Debug.Log("ADD letter called!");
            AddLetter(CurrentSelectedTassello, inputField.text[0]);
            inputField.text = "";
        }

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
        }

        private void ResetValuesAndGameBoard()
        {
            CurrentSelectedWordObject = null;
            CurrentSelectedTassello = null;
            wordObjects.Clear();
            foreach (Transform child in crosswordParent.GetComponentInChildren<Transform>())
                Destroy(child.gameObject);
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

        private Tassello FindNextFreeTassello(WordObject wordObject)
        {
            foreach (Tassello tassello in wordObject.tasselli)
            {
                if (tassello.Lettera == ' ')
                    return tassello;
            }
            return null;
        }

        /// <summary>
        /// Called when a tassello is pressed
        /// </summary>
        /// <param name="lettera"></param>
        public void OnTasselloPress(char lettera)
        {
            if (CurrentSelectedTassello)
            {
                AddLetter(CurrentSelectedTassello, lettera);
                CurrentSelectedTassello = FindNextFreeTassello(CurrentSelectedWordObject);
            }
        }

        public void OnBackSpacePress()
        {
            RemoveLetter();
        }

        private void AddLetter(Tassello tassello, char lettera)
        {
            if (tassello)
            {
                tassello.SetLettera(lettera);
                foreach (WordObject wordObject in tassello.wordObjectParents)
                    if(wordObject != null) {
                        if (wordObject.CheckWordCompletion())
                        {
                            Debug.Log("Word was completed correctly");
                            if (CheckGameCompletion())
                            {
                                Debug.Log("GAME HAS ENDED!!!!!");
                            }
                            else
                            {
                                CurrentSelectedWordObject = GetNotCompletedWord();
                                CurrentSelectedTassello = FindNextFreeTassello(CurrentSelectedWordObject);
                            }
                        }
                        else
                        {
                            //cambio solo se c'è un altro tassello libero (ovvero non era l'ultima lettera che ho inserito)
                            Tassello nextFreeTassello = FindNextFreeTassello(CurrentSelectedWordObject);
                            if(nextFreeTassello)
                                CurrentSelectedTassello = nextFreeTassello;
                            else
                            {
                                //Comportamento "hai riempito la parola ma è scorretta"
                                //METTI QUALCHE ANIMAZIONCINA!
                                CurrentSelectedTassello.SetLettera(' ');
                            }
                        }
                    }
            }
        }

        private void RemoveLetter()
        {
            bool selectionTasselloWasReached = false;
            for (int i = currentSelectedWordObject.tasselli.Length - 1; i >= 0; i--)
            {
                if (selectionTasselloWasReached)
                {
                    bool valid = true;
                    foreach (WordObject parent in currentSelectedWordObject.tasselli[i].wordObjectParents)
                        if (parent != null)
                            if (parent.Completed)
                                valid = false;
                    if (valid)
                    {
                        CurrentSelectedTassello = currentSelectedWordObject.tasselli[i];
                        CurrentSelectedTassello.SetLettera(' ');
                        break;
                    }
                }
                else if (currentSelectedWordObject.tasselli[i] == currentSelectedTassello)
                    selectionTasselloWasReached = true;
            }
        }

        public WordObject GetNotCompletedWord()
        {
            foreach(WordObject wordObject in wordObjects)
                if(!wordObject.Completed)
                    return wordObject;
            return null;
        }

        public bool CheckGameCompletion()
        {
            foreach (WordObject wordObject in wordObjects)
                if(!wordObject.Completed)
                    return false;
            return true;
        }

        #endregion
    }

}
