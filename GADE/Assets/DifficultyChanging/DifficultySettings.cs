using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class DifficultySettings : MonoBehaviour
    {
        public Dropdown difficultyDropdown;

        //private List<string> difficultyOptions = new List<string> { "Easy", "Medium", "Hard" };
        // Start is called before the first frame update
        private void Start()
        {
            // Populate the dropdown with difficulty options
            
            difficultyDropdown.AddOptions(new List<string> { "Easy", "Medium", "Hard" });
        }


        public void SaveDifficulty()
        {
            string selectedDifficulty = difficultyDropdown.options[difficultyDropdown.value].text;      // Save the selectedDifficulty to a variable or settings file
      
            Debug.Log("Selected difficulty: " + selectedDifficulty);
    }
    }
