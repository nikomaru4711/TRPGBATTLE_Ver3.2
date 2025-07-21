using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ReadJson : MonoBehaviour
{
    public PlayerCharacter ReadJsonFile(string jsontext)
    {
        PlayerCharacter obj = JsonUtility.FromJson<PlayerCharacter>(jsontext);

        return obj;
    }
}
