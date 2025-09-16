using UnityEngine;
public class ReadJson : MonoBehaviour
{
    public PlayerCharacter ReadJsonFile(string jsontext)
    {
        PlayerCharacter obj = JsonUtility.FromJson<PlayerCharacter>(jsontext);

        return obj;
    }
}
