using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crossword
{
    public class WordObject
    {
        private WordInfo wordInfo;
        private Tassello[] tasselli;

        private bool completed;
        public bool Completed => completed;


        public WordObject(WordInfo wordInfo)
        {
            this.wordInfo = wordInfo;
            tasselli = new Tassello[wordInfo.word.Length];

            AddTasselli();
        }

        private void AddTasselli()
        {
            for(int i = 0; i < wordInfo.word.Length; i++)
            {
                GameObject tasselloObj;
                if (true)   //FIX!!!! CONTROLLO SE UN TASSELLO LI' NON E' GIA' POSIZIONATO!!
                {
                    tasselloObj = GameObject.Instantiate(CrosswordLogic.Instance.TasselloPrefab, CrosswordLogic.Instance.CrosswordPanel.transform);
                }
                else
                {
                    tasselloObj = null; //FIX!!! SETTA tasselloObj = AL TASSELLO GIA' POSIZIONATO
                }
                Tassello tassello = tasselloObj.GetComponent<Tassello>();
                tasselli[i] = tassello;
                tassello.wordObjectParents.Add(this);
            }
        }

        public void CheckWordCompletion()
        {
            string currentString = "";
            foreach(Tassello tassello in tasselli)
                currentString += tassello.Lettera;
            if(currentString == wordInfo.word)
                completed = true;
        }

    }
}
