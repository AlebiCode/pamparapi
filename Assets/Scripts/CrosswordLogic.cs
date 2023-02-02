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
        private List<WordObject> wordObjects = new List<WordObject>();
        private int tasselloID = 0;
        private WordObject currentSelectedWordObject = null;
        private Tassello currentSelectedTassello = null;

        [SerializeField] UnityEngine.UI.InputField inputField;

        //PROPERTIES
        public static CrosswordLogic Instance => instance;
        public Tassello TasselloPrefab => tasselloPrefab;
        public GameObject CrosswordPanel => crosswordParent;
        public WordObject CurrentSelectedWordObject
        {
            set
            {
                if (currentSelectedTassello)
                    foreach (Tassello tassello in CurrentSelectedWordObject.tasselli)
                        tassello.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                currentSelectedWordObject = value;
                if(currentSelectedTassello)
                    foreach (Tassello tassello in CurrentSelectedWordObject.tasselli)
                        tassello.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            }
            get { return currentSelectedWordObject; }
        }

        //METODI UNITY

        private void Awake()
        {
            InstantiateSelf();
        }

        void Start()
        {
            GenerateObjects();
        }

        private void Update()
        {

        }

        #region METODI

        public void DEBUGLetterInsert() //FIX!!!! REMOVE FROM FINAL BUILD!!
        {
            Debug.Log("ADD letter called!");
            AddLetter(currentSelectedTassello, inputField.text[0]);
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

        private void GenerateObjects()
        {
            GenerationInfo generationInfo = CrosswordGenerator.GenerateWords();
            WordInfo[] wordInfos = generationInfo.wordInfoList.ToArray();
            float tasselloSize = tasselloPrefab.GetComponent<RectTransform>().rect.width;
            Vector3 offSet = -new Vector3((generationInfo.xMax + generationInfo.xMin) / 2f, (generationInfo.yMax + generationInfo.yMin) / 2f, 0) * tasselloSize; ;
            foreach (WordInfo wordInfo in wordInfos)
            {
                WordObject newWordobject = new WordObject(wordInfo);
                
                wordObjects.Add(newWordobject);
                AddTasselliToGameWord(newWordobject, tasselloSize, offSet);
            }
        }

        private void AddTasselliToGameWord(WordObject wordObject, float tasselloSize, Vector3 offSet)
        {
            Tassello[][] matrix = new Tassello[CrosswordGenerator.MAX_COLUMN][];
            for(int i=0; i<matrix.Length; i++)
                matrix[i] = new Tassello[CrosswordGenerator.MAX_ROW];
            for (int i = 0; i < wordObject.GetWordInfo.word.Length; i++)
            {
                Vector2Int coords;
                if(wordObject.GetWordInfo.horizontal)
                    coords = new Vector2Int(wordObject.GetWordInfo.x + i, wordObject.GetWordInfo.y);
                else
                    coords = new Vector2Int(wordObject.GetWordInfo.x, wordObject.GetWordInfo.y + i);
                Tassello tassello = matrix[coords.x][coords.y];
                if (tassello == null)
                {
                    tassello = GameObject.Instantiate(TasselloPrefab, CrosswordPanel.transform);
                    tassello.transform.localPosition += (new Vector3(coords.x,coords.y,0) * tasselloSize) + offSet;
                    tassello.wordObjectParents[0] = wordObject;
                    matrix[coords.x][coords.y] = tassello;
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
                currentSelectedTassello = FindNextFreeTassello(CurrentSelectedWordObject);
                if(currentSelectedTassello == null)
                    currentSelectedTassello = wordObject.tasselli[wordObject.tasselli.Length-1];
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
        private void OnTasselloPress(char lettera)
        {
            if (currentSelectedTassello)
            {
                AddLetter(currentSelectedTassello, lettera);
                currentSelectedTassello = FindNextFreeTassello(CurrentSelectedWordObject);
            }
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
                            if (CheckGameCompletion())
                            {
                                Debug.Log("GAME HAS ENDED!!!!!");
                            }
                            else
                            {
                                CurrentSelectedWordObject = GetNotCompletedWord();
                                currentSelectedTassello = FindNextFreeTassello(CurrentSelectedWordObject);
                            }
                        }
                        else
                        {
                            currentSelectedTassello = FindNextFreeTassello(CurrentSelectedWordObject);
                        }
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
