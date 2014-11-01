using UnityEngine;
using System.Collections;

public class ScrollNumEffect : MonoBehaviour 
{
    public delegate void OnFinish();
    public OnFinish onFinish;

    private int mNumPerSecond = 100;

    UILabel mLabel;

    bool needStart = false;
    bool ignoreAtFirst = true;

    private bool needInitOldNum = true;
    private bool mAutoDestroy = true;

    int mOldNum;
    int mNewNum;
    int mStepNum;
    float mOneTicker = 0f;

    //config data;
    string mHead = "";
    bool mShowPositiveFlag = false; // 是否显示正号;
    bool mShowNegativeFlag = false; // 是否显示负号;
    string mNumPrefix = "";
    string mEnd = "";
    bool isPostive = true;//是否是正数;

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        needStart = false;
        mOldNum = 0;
        mNewNum = 0;
    }
    
    void Update()
    {
        if (!needStart)
            return;

        if (mLabel == null)
        {
            mLabel = GetComponent<UILabel>();

            if (mLabel == null) return;
        }

        if (mNewNum != mOldNum)
        {
            mNumPerSecond = Mathf.Max(1, mNumPerSecond);

            mOneTicker += mNumPerSecond * Time.deltaTime;
            if (mOneTicker <= 1f)
                return;
            
            mStepNum = System.Convert.ToInt32(mOneTicker);
            mOneTicker = 0f;
            
            int num = 0;
            
            if (mNewNum > mOldNum)
            {
                num = Mathf.Min(mNewNum , mOldNum + mStepNum);
            }
            else if (mNewNum < mOldNum)
            {
                num = Mathf.Max(mNewNum , mOldNum - mStepNum);
            }
            mOldNum = num;

            mLabel.text = NumToStr(num);
        }
        else
        {
            if (onFinish != null)
            {
                onFinish();
            }
            
            needStart = false;
        }
    }

    string NumToStr(int num)
    {
        string content = "";
        if (isPostive)
        {
            content += mHead + (mShowPositiveFlag ? "+" : "");
        }
        else
        {
            content += mHead + (mShowNegativeFlag ? "-" : "");
        }

        string numStr = num.ToString();
        string tmpNumStr = "";
        if (!isPostive)
        {
            for (int i = 0, j = numStr.Length; i < j; i++)
            {
                tmpNumStr += mNumPrefix + numStr[i];
            }
        }
        else
        {
            tmpNumStr = numStr;
        }
        content += tmpNumStr + mEnd;

        return content;
    }

    /// <summary>
    /// 设置显示格式;
    /// </summary>
    /// <param name="head">文字头</param>
    /// <param name="showSymbol1">是否显示正号</param>
    /// <param name="showSymbol2">是否显示负号</param>
    /// <param name="numPrefix">显示符号后，数字前缀标志</param>
    /// <param name="end">文字尾</param>
    public void SetShowType(string head = "", bool showSymbol1 = false, bool showSymbol2 = false, string numPrefix = "", string end = "")
    {
        mHead = head;
        mShowPositiveFlag = showSymbol1;
        mShowNegativeFlag = showSymbol2;
        mNumPrefix = numPrefix;
        mEnd = end;
    }

    /// <summary>
    /// 将要变换的数字;
    /// </summary>
    /// <param name="num"></param>
    public void Play(int oldNum , int newNum , int numPerSecond = 100 , bool autoDestroy = true)
    {
        if (mLabel == null)
        {
            mLabel = GetComponent<UILabel>();

            if (mLabel == null)
            {
                Debug.LogError("ScrollNumEffect 需要加在UILabel上才可以正常运行！");
                return;
            }
        }

        if (oldNum == newNum)
        {
            mLabel.text = NumToStr(oldNum);

            //InvokeRepeating("finish", 0.5f, 0f);
            Invoke("finish", 0.5f);
            
            return;
        }

        mNumPerSecond = numPerSecond;
        mAutoDestroy = autoDestroy;


        mLabel.text = oldNum.ToString();
        mOldNum = oldNum;
        mNewNum = newNum;
        isPostive = (newNum - oldNum) >= 0;
        needStart = true;
    }

    void finish()
    {
        if (onFinish != null)
        {
            onFinish();
        }

        //CancelInvoke();
    }
}
