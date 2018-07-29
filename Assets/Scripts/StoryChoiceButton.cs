using UnityEngine;
using UnityEngine.UI;

public class StoryChoiceButton : MonoBehaviour {

    public Text textObject;

    [HideInInspector]
    public int choiceID = 0;

    // Get a reference to the text component.
    private void Awake() {
        textObject = GetComponentInChildren<Text>();
    }

    // Called after spawning, sets up the choice ID.
    public void Setup(int choiceID) {
        this.choiceID = choiceID;
    }

    // Called when the button is clicked.
    public void Clicked() {
        InteractiveStorySystem.Refresh(choiceID);
    }
}
