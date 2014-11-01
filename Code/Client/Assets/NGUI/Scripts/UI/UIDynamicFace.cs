
using UnityEngine;
using System.Collections.Generic;

public class UIDynamicFace
{
    private UIAtlas mAtlas = null;
    private List<BMSymbol> mSymbols = new List<BMSymbol>();
    private GameObject mParant = null;

    private List<UISprite> mFaces = new List<UISprite>();

    public void SetParant(GameObject parant)
    {
        mParant = parant;

       
    }
    public void SetAtlas(UIAtlas atlas)
    {
        mAtlas = atlas;
    }

    public void AddSymbol(string sequence, string spriteName)
    {
        BMSymbol symbol = GetSymbol(sequence, true);
        symbol.spriteName = spriteName;
    }

    public void RemoveSymbol(string sequence)
    {
        BMSymbol symbol = GetSymbol(sequence, false);
        if (symbol != null) mSymbols.Remove(symbol);
    }

    BMSymbol GetSymbol(string sequence, bool createIfMissing)
    {
        for (int i = 0, imax = mSymbols.Count; i < imax; ++i)
        {
            BMSymbol sym = mSymbols[i];
            if (sym.sequence == sequence) return sym;
        }

        if (createIfMissing)
        {
            BMSymbol sym = new BMSymbol();
            sym.sequence = sequence;
            mSymbols.Add(sym);
            return sym;
        }
        return null;
    }

    public void HideSymbol()
    {
        for( int i = 0 ; i < mFaces.Count ; ++i )
        {
            NGUITools.SetActive(mFaces[i].gameObject, false);
        }
    }

    public void DrawSymbol(int index , BMSymbol symbol , float x , float y)
    {
        if (index < 0)
            return;

        UISprite sprite = null;
        if (index >= mFaces.Count)
        {
            GameObject obj = new GameObject(index.ToString());
            sprite = obj.AddMissingComponent<UISprite>();
            obj.gameObject.transform.parent = mParant.transform;
            obj.gameObject.transform.localScale = Vector3.one;
            sprite.atlas = mAtlas;
            sprite.spriteName = symbol.spriteName;
            sprite.width = 38;
            sprite.height = 38;
            mFaces.Add(sprite);
        }else
        {
            sprite = mFaces[index];
        }
        NGUITools.SetActive(sprite.gameObject, sprite);

        sprite.spriteName = symbol.spriteName;
        sprite.gameObject.transform.localPosition = new Vector3(x + sprite.width / 2.0f, -y - sprite.width / 2.0f);
    }

    public BMSymbol MatchSymbol(string text, int offset, int textLength)
    {
        // No symbols present
        int count = mSymbols.Count;
        if (count == 0) return null;
        textLength -= offset;

        // Run through all symbols
        for (int i = 0; i < count; ++i)
        {
            BMSymbol sym = mSymbols[i];

            // If the symbol's length is longer, move on
            int symbolLength = sym.length;
            if (symbolLength == 0 || textLength < symbolLength) continue;

            bool match = true;

            // Match the characters
            for (int c = 0; c < symbolLength; ++c)
            {
                if (text[offset + c] != sym.sequence[c])
                {
                    match = false;
                    break;
                }
            }

            // Match found
            if (match && sym.Validate(mAtlas)) return sym;
        }
        return null;
    }
}
