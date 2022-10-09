using System.Collections.Generic;
using UnityEngine;

namespace Amnabi;

public class RecycleableTexture
{
    public static List<RecycleableTexture> recyclableStatic;

    public Texture2D flagTextureCompiled;

    public Material materialCompiiled;

    public bool needsRefreshNextCall = false;

    static RecycleableTexture()
    {
        recyclableStatic = new List<RecycleableTexture>();
    }

    private RecycleableTexture()
    {
    }

    public static RecycleableTexture nextRecycleableTexture(bool pop = true)
    {
        if (recyclableStatic.Count <= 0)
        {
            return new RecycleableTexture();
        }

        var result = recyclableStatic[recyclableStatic.Count - 1];
        recyclableStatic.RemoveAt(recyclableStatic.Count - 1);
        return result;
    }
}