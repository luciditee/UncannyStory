
using UnityEngine;

[CreateAssetMenu(fileName = "StoryItem", menuName = "Story Item", order = 1)]
public sealed class InteractiveStoryItem : ScriptableObject {

    public bool EndGame = false;
    public bool GiveItem = false;
    public string ItemName = "Item";

    [TextArea(3, 20)]
    public string StoryInfo = "";

    public StoryChoice[] choices;

    [System.Serializable]
    public sealed class StoryChoice {
        public string ChoiceString = "";
        public InteractiveStoryItem pointsTo;
        public bool UseItemBranch = false;
        public ItemBranchedChoice ItemBranch;
    }

    [System.Serializable]
    public sealed class ItemBranchedChoice {
        public string RequiredItem = "Item";
        public bool ConsumeItem = false;
        public InteractiveStoryItem withItem;
        public InteractiveStoryItem withoutItem;
    }
}