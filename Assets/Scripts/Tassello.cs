using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crossword {
    public class Tassello : MonoBehaviour
    {
        public List<WordObject> wordObjectParents = new List<WordObject>();
        private char lettera = ' ';

        public char Lettera => lettera;

        public void SetLettera(char lettera)    //CALLED WHEN A LETTER GETS DRAGGED ONTO THE TASSELLO
        {
            this.lettera = lettera;
            foreach (WordObject wordObject in wordObjectParents)
            {
                wordObject.CheckWordCompletion();
            }
        }
    }
}
