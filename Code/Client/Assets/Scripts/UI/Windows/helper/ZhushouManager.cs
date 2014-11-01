using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//这是个比较器
public class NameComparer : IComparer<ZhushouTableItem>
{ 
    //实现num升序
    public int Compare(ZhushouTableItem x,ZhushouTableItem y)
    {
        return(x.resID - y.resID);
    }
}

class ZhushouManager
{
    private static ZhushouManager instance;

    private List<ZhushouTableItem> mQuestList = new List<ZhushouTableItem>();

    private Queue<ZhushouTableItem> mItemsQueue = null;

    private PaoPaoNode mPaoPao = null;
    private ZhushouTableItem mCurItem = null;

    private int mDepth = -1;

    public ZhushouManager()
	{
        instance = this;
    }

    public static ZhushouManager Instance
    {
        get
        {
            return instance;
        }
    }

    public bool Init(DataTable table)
    {
        IDictionaryEnumerator itr = table.GetEnumerator();
        while (itr.MoveNext())
        {
            ZhushouTableItem item = itr.Value as ZhushouTableItem;

            mQuestList.Add(item);
        }
//         foreach (DictionaryEntry v in table)
//         {
//             ZhushouTableItem item = v.Value as ZhushouTableItem;
// 
//             mQuestList.Add(item);
//         }
        mQuestList.Sort(new NameComparer());

        return true;
    }



    public Queue<ZhushouTableItem> CheckZhushou()
    {
        Queue<ZhushouTableItem> result = new Queue<ZhushouTableItem>();

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        //List<Quest> list = module.GetQuestList();

        for (int i = 0; i < mQuestList.Count; ++i )
        {
            if( module.IsQuestAccepted( mQuestList[i].questid ) )
            {
                result.Enqueue(mQuestList[i]);
            }
        }
        return result;
    }

    public void Begin()
    {
        UICityForm city = WindowManager.Instance.GetUI("city") as UICityForm;

        if( city != null )
        {
            BeginPaoPao(city.GetDepth());
        }
    }

    private void BeginPaoPao(int depth)
    {
        mDepth = depth;
        GameDebug.Log("BeginPaoPao");
        mItemsQueue = CheckZhushou();

        if (mItemsQueue != null && mItemsQueue.Count > 0)
        {
            ZhushouTableItem  item = mItemsQueue.Dequeue();

            GuideManager.Instance.OnBeginHelper(item.questid);

            if( mPaoPao == null )
            {
                mPaoPao = PaoPaoManager.Instance.CreatePaoPaoUI(true, 0, 0, (int)UI_LAYER_TYPE.UI_LAYER_WINDOWS);
                mPaoPao.SetOffset(-9.1f);
                mPaoPao.SetGap(50, 50);
                mPaoPao.SetColor(0xFEd514FF);
            }
            NextPaoPao(item);
        }
    }

    public void EndPaoPao()
    {
        if (mPaoPao != null)
        {
            mPaoPao.Hide();
//             PaoPaoManager.Instance.ReleasePaoPaoUI(mPaoPao);
//             mPaoPao = null;
        }
    }

    void NextPaoPao(ZhushouTableItem item)
    {
        mCurItem = item;

        mPaoPao.Talk(mCurItem.talk, -1, mDepth + 1);
    }

    public void Update(Vector3 pos, uint elapsed)
    {
        if( mPaoPao != null )
        {
            if (PlayerController.Instance.IsAutoMoving)
            {
                mPaoPao.Hide();
            }else
            {
                if( mCurItem != null )
                {
                    mPaoPao.Show();
                    mPaoPao.Update(pos, elapsed);
                }
            }
        }
    }

    public void Execute()
    {
        if (mCurItem != null)
        {
            WindowManager.Instance.OpenUI(mCurItem.param, mCurItem.questid);
            
            if( mCurItem.force )
            {
                return;
            }

            mCurItem = null;

            if (mPaoPao != null)
            {
                mPaoPao.Hide();
            }

            if (mItemsQueue.Count == 0)
            {
                return;
            }
        }

        if (mItemsQueue.Count <= 0)
        {
            WindowManager.Instance.OpenUI("assister");
            return;
        }
        ZhushouTableItem item = mItemsQueue.Dequeue();
        NextPaoPao(item);
    }
}

