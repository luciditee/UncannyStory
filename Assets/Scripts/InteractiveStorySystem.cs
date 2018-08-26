using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InteractiveStorySystem : MonoBehaviour {

    public Text storyText;
    public Transform choiceHolder;
    public Transform choiceButtonPrefab;
    public InteractiveStoryItem startingItem;
    public HistoryCascade[] cascadesToReset;
    public AudioSource loopingSource;
    public AudioSource oneShotSource;
    public Image BackgroundElement;

    public List<string> inventory = new List<string>();
    private List<InteractiveStoryItem> storyHistory = new List<InteractiveStoryItem>();

    private static InteractiveStorySystem self;
    private InteractiveStoryItem currentItem;

	// Use this for initialization
	void Awake () {
        self = this;
        StartOver();
    }

    // Start the story over.
    public void StartOver() {
        currentItem = startingItem;

        // Reset all tracked history cascades to prevent nullreferences.
        foreach (HistoryCascade cascade in cascadesToReset) {
            cascade.clickCounter = 0;
        }

        // Clear history.
        storyHistory.Clear();

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
            // Skip previously visited onetime destinations, or null (removed) destinations
            if ((item.Disabled == true && (item.cascades == null || item.cascades.historyCascades.Length == 0)) || (storyHistory.Contains(item.pointsTo) && item.pointsTo.OneTimeVisit)) {
                index++;
                continue;
            }
                    

            // Create the choice button
            Transform inst = Instantiate(choiceButtonPrefab, choiceHolder);

            // Assign text
            StoryChoiceButton choice = inst.GetComponent<StoryChoiceButton>();
            choice.choiceID = index;
            choice.textObject.text = item.ChoiceString;

            // Increment choice index
            index++;
        }

        // Apply audio loop.
        // Do not alter looping sound if current sound matches prior sound.
        if (loopingSource != null && currentItem.LoopSound != null && loopingSource.clip != currentItem.LoopSound) {
            loopingSource.Stop();
            loopingSource.clip = currentItem.LoopSound;
            loopingSource.loop = true; // make sure this is always on.
            loopingSource.Play();
        }

        // Apply one-shot sound.
        if (oneShotSource != null && currentItem.EntrySound != null) {
            oneShotSource.PlayOneShot(currentItem.EntrySound);
        }

        // Apply background information.
        // In the event there is no background, hide the image component entirely.
        if (BackgroundElement != null) {
            BackgroundElement.enabled = (currentItem.BackgroundPicture != null);
            BackgroundElement.sprite = currentItem.BackgroundPicture;
        }

        // Record this item to history
        storyHistory.Add(currentItem);
    }

    void RefreshStory(int choice) {
        // Store this for later
        bool useCascades = (currentItem.choices[choice].cascades != null);

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
            // No item branching if we're here.
            if (useCascades) {
                // If this branch has cascades, we should increment the cascade value.
                // Cache it first
                HistoryCascade cascades = currentItem.choices[choice].cascades;
                cascades.clickCounter++;

                // Bounds checking so it never exceeds sizeof(cascades.clickCounter)
                if (cascades.clickCounter >= cascades.historyCascades.Length) {
                    cascades.clickCounter = cascades.historyCascades.Length - 1;
                }

                // Jump to the next story bit
                currentItem = cascades.historyCascades[cascades.clickCounter];
            } else {
                // No cascades, simply proceed to the next one.
                currentItem = currentItem.choices[choice].pointsTo;
            }
        }
        
        // Set up the UI to reflect the change.
        SetupElements();
    }
	
    // Static wrapper for RefreshStory() so we don't have to GetComponent or Find this object
	public static void Refresh(int choice) {
        self.RefreshStory(choice);
    }
}
