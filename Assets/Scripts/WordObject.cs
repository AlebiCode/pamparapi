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
        public Tassello[] tasselli;
        private bool completed = false;

        public bool Completed{
            set { completed = value; }
            get { return completed; }
        }
        //public Tassello[] Tasselli => tasselli;
        public WordInfo GetWordInfo => wordInfo;

        //---------------------------------------------

        public WordObject(WordInfo wordInfo)
        {
            this.wordInfo = wordInfo;
            tasselli = new Tassello[wordInfo.word.Length];
        }


        public bool CheckWordCompletion()
        {
            string currentString = "";
            foreach(Tassello tassello in tasselli)
                currentString += tassello.Lettera;
            if (currentString == wordInfo.word)
            {
                Completed = true;
            }
            return Completed;
        }

    }
}
