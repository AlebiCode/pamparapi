using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Crossword {
    public class Tassello : MonoBehaviour
    {
        public WordObject[] wordObjectParents = new WordObject[2];
        [SerializeField] private Text lettera;

        public char Lettera => lettera.text[0];

        private void Awake()
        {
            lettera.text = " ";
        }

        public void SetLettera(char lettera)    //CALLED WHEN A LETTER GETS SET ONTO THE TASSELLO
        {
            this.lettera.text = lettera + "";
        }

        public void SelectTassello()
        {
            if(CrosswordLogic.Instance.CurrentSelectedWordObject == wordObjectParents[0] && wordObjectParents[1] != null)
                CrosswordLogic.Instance.OnWordSelection(wordObjectParents[1]);
            else
                CrosswordLogic.Instance.OnWordSelection(wordObjectParents[0]);
        }
    }
}
