using UnityEngine;
using System.Collections;

public class ArrawUI : MonoBehaviour
{

    // Use this for initialization
    public UISprite mArrow = null;
    public UISprite mEffect = null;
    private ArrowRot mArrowRot = ArrowRot.ArrowRot_Invaild;
    private GuideStepTableItem mStep;
    private GameObject mControl;
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

        if (mControl == null) return;
       GuideHelper.UpdateUIPos(mControl,(ArrowRot)mStep.arrow,this);
      
    }


    public void SetData(GuideStepTableItem data)
    {
        mStep = data;
      
    }

    public bool SetControl()
    {
        //GameObject window = WindowManager.Instance.GetUI(mStep.window);
        ////Debug.Log("window");
        //if (window != null)
        //{
        //    mControl = ObjectCommon.GetChild(window, mStep.ctrl);
        //    if (mControl == null)
        //    {
        //       // Debug.Log("mControl");
        //        //GameDebug.LogError("获取" + mStep.window + "下的" + mStep.ctrl + "失败");
        //        return false;
        //    }
        //    //Debug.Log("mStep.arrow");
        //    GuideHelper.UpdateUIPos(mControl, (ArrowRot)mStep.arrow, this);
        //}

        return true;
    }
  

    public void SetEffectPos(Vector3 pos)
    {
        mEffect.transform.position = pos;
    }

    public void SetEffectSize(int width, int height)
    {
        //美术做的这个图 有16个像素边框
        mEffect.width = width + 16;
        mEffect.height = height + 16;
    }


    public void SetArrowPos(Vector3 pos)
    {
        if (mArrow.transform.position != pos)
        {
            mArrow.transform.position = pos;
        }
    }

    public int GetArrowWidth()
    {
        if (mArrowRot == ArrowRot.ArrowRot_UP || mArrowRot == ArrowRot.ArrowRot_DOWN)
        {
            return mArrow.width;
        }
        return mArrow.height;
    }

    public int GetArrowHeight()
    {
        if (mArrowRot == ArrowRot.ArrowRot_UP || mArrowRot == ArrowRot.ArrowRot_DOWN)
        {
            return mArrow.height;
        }
        return mArrow.width;
    }

    public void SetArrowRot(ArrowRot rot)
    {
        if (mArrowRot == rot)
        {
            return;
        }
        mArrowRot = rot;
        float rotAngle = 0.0f;
        switch (rot)
        {
            case ArrowRot.ArrowRot_UP:
                rotAngle = 180.0f;
                break;
            case ArrowRot.ArrowRot_DOWN:
                rotAngle = 0.0f;
                break;
            case ArrowRot.ArrowRot_LEFT:
                rotAngle = 270.0f;
                break;
            case ArrowRot.ArrowRot_RIGHT:
                rotAngle = 90.0f;
                break;
        }
        if (Mathf.Abs((mArrow.transform.eulerAngles.z - rotAngle)) > 0.00001f)
        {
            Quaternion qua = Quaternion.Euler(new Vector3(0.0f, 0.0f, rotAngle));
            mArrow.transform.rotation = qua;
        }
    }

    
}
