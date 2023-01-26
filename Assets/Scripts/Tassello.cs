using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crossword {
    public class Tassello : MonoBehaviour
    {
        public WordObject[] wordObjectParents = new WordObject[2];
        private char lettera = ' ';

        public char Lettera => lettera;

        public void SetLettera(char lettera)    //CALLED WHEN A LETTER GETS SET ONTO THE TASSELLO
        {
            this.lettera = lettera;
            foreach (WordObject wordObject in wordObjectParents)
            {
                wordObject.CheckWordCompletion();
            }
        }

        public void SelectTassello()
        {
            CrosswordLogic.Instance.UpdateSelection(this);
        }
    }
}
