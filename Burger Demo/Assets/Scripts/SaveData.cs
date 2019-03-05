[System.Serializable]

public class SaveData
{
    // Add to this as we need more things that get saved.
    public int currentHealth;
    public int healthMax;
    public UnityEngine.Vector2 currentPos;
    public bool[] convoDone;
    public string currentScene;

    public int checkpointIndex;
}
