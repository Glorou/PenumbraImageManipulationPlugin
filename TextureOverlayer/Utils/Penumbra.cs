using System.Collections.Generic;

namespace TextureOverlayer.Utils;

public class PenumbraMeta {
    public int FileVersion = 3;
    public string Name = "";
    public string Author = "";
    public string Description = "";
    public string Version = "";
    public string Website = "";
    public List<string> ModTags = [];
}
public class SelectedPenumbraMod {
    public Dictionary<string, List<string>> SourceFiles;
    public Dictionary<string, List<string>> ReplaceFiles;
    public PenumbraMeta Meta;
}
public class PenumbraModStruct : PenumbraItemStruct {
    public Dictionary<string, string> Files = [];
    public Dictionary<string, string> FileSwaps = [];
    public List<object> Manipulations = [];
}
public class PenumbraItemStruct {
    public string Name = "";
    public string Description = "";
    public int Priority = 0;
}
