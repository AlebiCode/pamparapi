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
        private Tassello currentSelectedTassello = null;
        private WordObject currentSelectedWordObject = null;

        //PROPERTIES
        public static CrosswordLogic Instance => instance;
        public GameObject TasselloPrefab => tasselloPrefab;
        public GameObject CrosswordPanel => crosswordPanel;


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

        public void FillSelectedTassello(char c)
        {
            if (currentSelectedTassello != null)
            {
                currentSelectedTassello.SetLettera(c);
                //FIX!!!SELEZIONA AUTOMATICAMENTE IL TASSELLO SUCCESSIVO
            }
        }
    }
}
