using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InteractiveStorySystem : MonoBehaviour {

    public Text storyText;
    public Transform choiceHolder;
    public Transform choiceButtonPrefab;
    public InteractiveStoryItem startingItem;

    public List<string> inventory = new List<string>();

    private static InteractiveStorySystem self;
    private InteractiveStoryItem currentItem;

	// Use this for initialization
	void Awake () {
        self = this;
        currentItem = startingItem;
        SetupElements();
    }

    void SetupElements() {
        // Delete all old children
        foreach (Transform child in choiceHolder) {
            Destroy(child.gameObject);
        }

        // Put all the refreshable items in.
        storyText.text = currentItem.StoryInfo;

        // Spawn them one at a time
        int index = 0;
        foreach (InteractiveStoryItem.StoryChoice item in currentItem.choices) {
            Transform inst = Instantiate(choiceButtonPrefab, choiceHolder);

            // Assign text
            StoryChoiceButton choice = inst.GetComponent<StoryChoiceButton>();
            choice.choiceID = index;
            choice.textObject.text = item.ChoiceString;

            // Increment choice index
            index++;
        }
    }

    void RefreshStory(int choice) {
        // In the event the story branches based on the player having an item, like a key,
        // perform the branch.
        if (currentItem.choices[choice].UseItemBranch) {
            // Does the inventory contain the item?
            if (inventory.Contains(currentItem.choices[choice].ItemBranch.RequiredItem)) {
                // If it's consumable, remove it.
                if (currentItem.choices[choice].ItemBranch.ConsumeItem)
                    inventory.Remove(currentItem.choices[choice].ItemBranch.RequiredItem);

                // Advance to next story item because player had inventory item.
                currentItem = currentItem.choices[choice].ItemBranch.withItem;
            } else {
                // Player did not have inventory item.
                currentItem = currentItem.choices[choice].ItemBranch.withoutItem;
            }
        } else {
            // No item branching, simply go to the next story point.
            currentItem = currentItem.choices[choice].pointsTo;
        }
        
        // Set up the UI to reflect the change.
        SetupElements();
    }
	
    // Static wrapper for RefreshStory() so we don't have to GetComponent or Find this object
	public static void Refresh(int choice) {
        self.RefreshStory(choice);
    }
}
