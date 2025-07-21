[System.Serializable]
public class PlayerCharacter
{
    public string kind;
    public Data data;
}

[System.Serializable]
public class Data
{
    public string name;
    public int initiative;
    public string externalUrl;
    public string iconUrl;
    public string commands;
    public Status status;
    public Params param;
}

[System.Serializable]
public class Status
{
    public string label;
    public int value;
    public int max;
    public Status(string label, int value, int max)
    {
        this.label = label;
        this.value = value;
        this.max = max;
    }
}

[System.Serializable]
public class Params
{
    public string label;
    public string value;
}