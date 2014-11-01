using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class QuestPageUI
{
    public UIGrid QuestBtnGrid;
    
    public int mCurShowId;

    public List<Quest> mQuestList; 

	//页数索引
	public int  mPageType;

    public QuestPageUI(UIGrid grid,int pageIndex)
    {
        QuestBtnGrid = grid;
        mPageType = pageIndex;
    }
  
}

