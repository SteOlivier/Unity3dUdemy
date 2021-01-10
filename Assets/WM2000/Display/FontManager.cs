
using UnityEngine;
using System.Collections;

public class FontManager : MonoBehaviour
{
    public Font[] fonts;

    private FontManager instance;
    public FontManager get
    {
        get
        {
            return instance;
        }
    }

    public void Awake()
    {
        instance = this;
    }

    public Font GetFont(string fontName)
    {
        foreach (Font font in fonts)
            if (font.name == fontName)
                return font;

        return null;
    }
}

