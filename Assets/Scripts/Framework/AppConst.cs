using System.Collections;
using System.Collections.Generic;
public enum GameMode
{
    EditorMode,
    PackgeBundle,
    UpdateMode,
}

public enum GameEvent
{
    StartLua,
    GameInit = 10000,
}
public class AppConst
{
    public const string BundleExtension = ".ab";
    public const string FileListName = "filelist.txt";
    public static GameMode GameMode = GameMode.EditorMode;
    public const string ResourceUrl = "http://127.0.0.1/AssetBundles";

    public static bool OpenLog = true;
}
