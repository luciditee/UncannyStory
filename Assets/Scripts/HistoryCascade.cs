
using UnityEngine;

[CreateAssetMenu(fileName = "History", menuName = "History Cascade", order = 2)]
public sealed class HistoryCascade : ScriptableObject {
    public InteractiveStoryItem[] historyCascades;
    public int clickCounter = 0;
}