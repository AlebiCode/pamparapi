using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crossword{
    public class CrosswordLogic : MonoBehaviour
    {
        //VARIABILI
        private static CrosswordLogic instance;
        [SerializeField] private GameObject tasselloPrefab;
        [SerializeField] private GameObject crosswordPanel;
        private List<WordObject> wordObjects = new List<WordObject>();
        private int tasselloID = 0;
        private WordObject currentSelectedWordObject = null;

        //PROPERTIES
        public static CrosswordLogic Instance => instance;
        public GameObject TasselloPrefab => tasselloPrefab;
        public GameObject CrosswordPanel => crosswordPanel;

        private WordObject NextWordObject {
            get {
                foreach (WordObject wordObject in wordObjects)
                {
                    if (!wordObject.Completed)
                    {
                        return wordObject;
                    }
                }
                return null;
            }
        }

        //METODI UNITY

        private void Awake()
        {
            InstantiateSelf();
        }

        void Start()
        {
            GenerateWordObjects();
        }

        void Update()
        {
        
        }

        //METODI

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

        private void GenerateWordObjects()
        {
            WordInfo[] wordInfos = CrosswordGenerator.GenerateWords().ToArray();
            foreach (WordInfo wordInfo in wordInfos)
            {
                AddWordObject(wordInfo);
            }
        }

        public void AddWordObject(WordInfo wordInfo)
        {
            wordObjects.Add(new WordObject(wordInfo));
        }

        public void PlaceLetterInTassello(char c)
        {
            currentSelectedWordObject.Tasselli[tasselloID].SetLettera(c);
            SetNextTassello(tasselloID);
        }
        public void ReomveLetterInTassello()
        {
            currentSelectedWordObject.Tasselli[tasselloID].SetLettera(' ');
            //FIX!!!SELEZIONA AUTOMATICAMENTE IL TASSELLO PRECEDENTE
        }

        private void SetNextTassello(int tasselloID)
        {
            int newID = tasselloID+1;
            while (newID < currentSelectedWordObject.Tasselli.Length)
            {
                WordObject parent1 = currentSelectedWordObject.Tasselli[newID].wordObjectParents[0];
                WordObject parent2 = currentSelectedWordObject.Tasselli[newID].wordObjectParents[0];
                if (parent2 != null)
                {
                    if((currentSelectedWordObject != parent1 && parent1.Completed) || (currentSelectedWordObject != parent2 && parent2.Completed))
                        newID++;    //la lettera che sarebbe stata dopo è di una parola già completata. Vai a quella dopo ancora
                }
                else
                    this.tasselloID = tasselloID;
            }
            //La parola è stata completata (con successo? non è detto)

            
        }

        private void ChangeSelectedWord(WordObject newSelection)
        {
            currentSelectedWordObject = newSelection;
        }

        public void CheckForGameCompletion()
        {
            WordObject wordObj = NextWordObject;
            if (wordObj == null)
            {
                //FIX!!!! GAME ENDS!!!
            }
            else
            {
                ChangeSelectedWord(wordObj);
            }
        }

        private void ChangeSelectedTassello(int id)
        {
            tasselloID = id;
        }
        /// <summary>
        /// In base al tassello con cui interagisco, se non è di una parola completata e corretta, seleziona la parola corrente e il suo primo tassello libero, oppure l'ultimo tassello se non ci sono tasselli liberi (e quindi è una parola errata)
        /// </summary>
        /// <param name="tassello">Il tassello con cui ho interagito.</param>
        public void UpdateSelection(Tassello tassello)
        {
            //FIX!!!!! se ci sono più di un parenbte per il tassello??
            if (!tassello.wordObjectParents[0].Completed)
            {
                ChangeSelectedWord(tassello.wordObjectParents[0]);
                int i = 0;
                while (currentSelectedWordObject.Tasselli[i].Lettera != ' ' && i < currentSelectedWordObject.Tasselli.Length-1)
                    i++;
                ChangeSelectedTassello(i);
            }
        }

    }
}
