using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyTypes : MonoBehaviour
{
    private Dropdown difficultyDropdown;

    private void Start()
    {
        difficultyDropdown = GetComponent<Dropdown>();
        difficultyDropdown.onValueChanged.AddListener(OnDifficultySelection);
    }

    private void OnDifficultySelection(int index)
    {
        string selectedDifficulty = difficultyDropdown.options[index].text;     //Gets the difficulty opption thats selected
        //Handles the selectid difficulty and is used for the AI level
        Debug.Log("Selected Difficulty: " + selectedDifficulty);
    }
}

