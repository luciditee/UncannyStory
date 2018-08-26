
using UnityEngine;

[CreateAssetMenu(fileName = "StoryItem", menuName = "Story Item", order = 1)]
public sealed class InteractiveStoryItem : ScriptableObject {

    public bool EndGame = false;
    public bool GiveItem = false;
    public string ItemName = "Item";
    public bool OneTimeVisit = false;
    public AudioClip LoopSound = null;
    public AudioClip EntrySound = null;
    public Sprite BackgroundPicture = null;

    [TextArea(3, 20)]
    public string StoryInfo = "";

    public StoryChoice[] choices;

    [System.Serializable]
    public sealed class StoryChoice {
        public string ChoiceString = "";
        public InteractiveStoryItem pointsTo;
        public bool UseItemBranch = false;
        public ItemBranchedChoice ItemBranch;
        public HistoryCascade cascades;
        public bool Disabled = false;
    }

    [System.Serializable]
    public sealed class ItemBranchedChoice {
        public string RequiredItem = "Item";
        public bool ConsumeItem = false;
        public InteractiveStoryItem withItem;
        public InteractiveStoryItem withoutItem;
    }
}