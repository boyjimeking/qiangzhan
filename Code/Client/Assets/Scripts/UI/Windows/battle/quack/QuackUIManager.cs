using UnityEngine;
using System.Collections;


//冒字管理器
public class QuackUIManager
{
    //缓存列表
    private Queue mCacheQueue = new Queue();

    //工作列表
    private ArrayList mAllList = new ArrayList();

    private int namen = 0;
    public QuackUIManager()
    {
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_DAMAGE, onDamage);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PLAYER_DAMAGE, onPlayerDamage);
		EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PLAYER_MANA_CHANGED, onPlayerManaChanged);
		EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_BUFF_ADDED, onBuffAdded);
		EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_GHOST_DAMAGE, onGhostDamage);
    }

    private void onPlayerDamage(EventBase e)
    {
        BattleUIEvent evt = (BattleUIEvent)e;
        AddNewDamage(evt.pos, evt.damage.Value,false, true, evt.damage.Critical, evt.dead);
    }

	private void onPlayerManaChanged(EventBase e)
	{
		BattleUIEvent evt = (BattleUIEvent)e;

		AddNewDamage(evt.pos, evt.deltaMana, true, true, false, evt.dead);
	}

	private void onGhostDamage(EventBase e)
	{
		BattleUIEvent evt = (BattleUIEvent)e;
		AddNewDamage(evt.pos, evt.damage.Value, false, true, evt.damage.Critical, evt.dead);
	}

    private void onDamage(EventBase e)
    {
        BattleUIEvent evt = (BattleUIEvent)e;
        AddNewDamage(evt.pos, evt.damage.Value, false, false, evt.damage.Critical, evt.dead);
    }

	void onBuffAdded(EventBase e)
	{
		BattleUIEvent evt = (BattleUIEvent)e;
		AddNewBuff(evt.pos, evt.bmpPath);
	}

	public void AddNewBuff(Vector3 pos, string bmpPath)
	{
		//等待新机制修改
		QuackUI node = null;
		if (mCacheQueue.Count > 0)
		{
			node = mCacheQueue.Dequeue() as QuackUI;
		}
		else
		{
			GameObject clone = WindowManager.Instance.CloneCommonUI("QuackUI");
			GameObject.DontDestroyOnLoad(clone);
			node = new QuackUI(clone);
			mAllList.Add(node);
		}

		NGUITools.SetActive(node.gameObject, true);

		node.Reset(this, pos, 0, false, false, false, false, bmpPath);
	}

    public void AddNewDamage(Vector3 pos, int val, bool isMana, bool isPlayer , bool critical, bool dead)
    {
        //等待新机制修改
        QuackUI node = null;
        if (mCacheQueue.Count > 0)
        {
            node = mCacheQueue.Dequeue() as QuackUI;
        }
        else
        {
            GameObject clone = WindowManager.Instance.CloneCommonUI("QuackUI");
            GameObject.DontDestroyOnLoad(clone);
            node = new QuackUI(clone);
            mAllList.Add(node);
        }
        NGUITools.SetActive(node.gameObject, true);

		node.Reset(this, pos, val, isMana, dead, isPlayer, critical, null);
    }

    public void Update(uint elapsed)
    {
        for( int i = 0 ; i < mAllList.Count ; ++i )
        {
            QuackUI ui = mAllList[i] as QuackUI;
            if( !ui.IsEnd())
            {
                ui.Update(elapsed);
            }
        }
    }

    public void FreeUI(QuackUI node)
    {
        NGUITools.SetActive(node.gameObject, false);

        mCacheQueue.Enqueue(node);
    }
    public void Destory()
    {
		EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_BUFF_ADDED, onBuffAdded);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_DAMAGE, onDamage);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PLAYER_DAMAGE, onPlayerDamage);
		EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PLAYER_MANA_CHANGED, onPlayerManaChanged);
		EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_GHOST_DAMAGE, onGhostDamage);
    }
}
