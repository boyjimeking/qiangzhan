using UnityEngine;

public delegate void FTyeWritterEffectCallBack();
[RequireComponent(typeof (UILabel))]
[AddComponentMenu("NGUI/Examples/FTypewriter Effect")]
public class FTypewriterEffect : MonoBehaviour
{
    
    public int charsPerSecond = 40;

    private UILabel mLabel;
    private string mText;
    private int mOffset = 0;
    private float mNextChar = 0f;
    [HideInInspector]
    public FTyeWritterEffectCallBack CallBack;

    public void SetPText(string processedText)
    {
        mText = processedText;
    }
    private void Update()
    {
        if (mLabel == null)
        {
            mLabel = GetComponent<UILabel>();
           // mText = mLabel.processedText;
        }

        if (mOffset < mText.Length)
        {
            if (mNextChar <= RealTime.time)
            {
                charsPerSecond = Mathf.Max(1, charsPerSecond);

                // Periods and end-of-line characters should pause for a longer time.
                float delay = 1f/charsPerSecond;
                char c = mText[mOffset];
                if (c == '.' || c == '\n' || c == '!' || c == '?') delay *= 4f;

                // Automatically skip all symbols
                NGUIText.ParseSymbol(mText, ref mOffset);

                mNextChar = RealTime.time + delay;
                mLabel.text = mText.Substring(0, ++mOffset);
            }
        }
        else
        {
            CallBack();
            enabled = false;
        }
    }
}

