using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ArrawManager
{
    private static ArrawManager mInstance = null;

    private ArrawUI mArrawUI = null;
    private bool IsCanShow = false;
    private int mGuideId;
    public static ArrawManager Instance
    {
        get { return mInstance ?? (mInstance = new ArrawManager()); }
    }

    private ArrawManager()
    {
        EventSystem.Instance.addEventListener(StagePassServerEvent.STAGE_PASS_SERVER_EVENT, OnPassStage);
    }
    public void HideUI(string uiName)
    {
        var stepItem = GetItem();
        if (IsCanShow&&stepItem != null && uiName == stepItem.window)
        {
            if (mArrawUI != null && mArrawUI.gameObject.activeSelf)
            {
                mArrawUI.gameObject.SetActive(false);
            }
        }
     
    }

    public void OnPassStage(EventBase evt)
    {
		var spe = evt as StagePassServerEvent;
		if(spe.mStageData == null)
		{
			return;
		}

		var sst= DataManager.Scene_StageSceneTable[spe.mStageData.stageid] as Scene_StageSceneTableItem;
		if (IsCanShow)
		{
			var stepItem = GetItem();
			if (stepItem == null)
			{
				IsCanShow = false;
				return;
			}

			if (stepItem.id == sst.mArrow)
			{
				IsCanShow = false;
			}
		}
    }

    public void ShowUI(string uiName)
    {
        if (GuideManager.Instance.IsGuideShow()) return;
        var stepItem = GetItem();
        if (IsCanShow && stepItem != null && uiName == stepItem.window)
        {
            if (mArrawUI == null)
            {
                //GameObject obj = WindowManager.Instance.CreateUI("UI/Guide/ArrowUI");
                //if (obj != null)
                //{
                //    mArrawUI = obj.GetComponent<ArrawUI>();
                //    WindowManager.Instance.SetDepth(obj, 1000);
                   
                //}
            }
            if (mArrawUI != null)              
            {
                mArrawUI.SetData(stepItem);
                if (!mArrawUI.SetControl())
                {
                     mArrawUI.gameObject.SetActive(false);
                }
                else
                {
                    if (!mArrawUI.gameObject.activeSelf)
                    {
                        mArrawUI.gameObject.SetActive(true);
                    }
                }               
               
            }
              
        }
        else
        {
            if (mArrawUI != null)
            {
                mArrawUI.gameObject.SetActive(false);
            }
          
        }
        
    }

    private GuideStepTableItem GetItem()
    {
        var stepItem = DataManager.GuideStepTable[mGuideId] as GuideStepTableItem;

        return stepItem;
    }

    public void SetGuide(int id)
    {
        Debug.Log("设置指引id"+id);
        mGuideId = id;
        IsCanShow = true;
    }
}


