[System.Serializable]
public class PlayerCharacter
{
    public string kind;
    public Data data;
    public PlayerCharacter(string kind, Data data)
    {
        this.kind = kind;
        this.data = data;
    }
}

[System.Serializable]
public class Data
{
    public string name;
    public int initiative;
    public string externalUrl;
    public string iconUrl;
    public string commands;
    public Status[] status = new Status[4];
    public Params[] param = new Params[8];

    public Data(string name, int initiative, string externalUrl, string iconUrl, string commands, Status[] status, Params[] param)
    {
        this.name = name;
        this.initiative = initiative;
        this.externalUrl = externalUrl;
        this.iconUrl = iconUrl;
        this.commands = commands;
        this.status = status;
        this.param = param;
    }
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
    public Params(string label, string value)
    {
        this.label=label;
        this.value=value;
    }
}