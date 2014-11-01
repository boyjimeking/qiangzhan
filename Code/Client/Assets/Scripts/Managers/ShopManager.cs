using UnityEngine;
using System.Collections;

public class ShopManager : Singleton<ShopManager>
{
    private static ShopManager instance = null;

    private PlayerDataModule mModule = null;

    private uint mLastMillionSec = 0;
    private uint mOneSecNum = 0;

    PlayerDataModule Module
    {
        get
        {
            if (mModule == null)
                mModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

            return mModule;
        }
    }

    public ShopManager()
    {
        //instance = this;
    }
    public void Update(uint elapsed)
    {
        mLastMillionSec = (mLastMillionSec + elapsed) % uint.MaxValue;

        if ((mLastMillionSec - mOneSecNum) >= 1000)
        {
            mOneSecNum = mLastMillionSec;
            
            PlayerShopData data = Module.GetPlayerShopData();
            if(data == null)
            {
                GameDebug.LogError("商店数据错误！");
                return;
            }

            if (data.Seconds < 0)
                return;

            if (data.Seconds == 0)
            {
                ShopFreeRefreshAction param = new ShopFreeRefreshAction();
                param.OpType = (int)ShopOpType.FreeRefresh;

                Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SHOP, param);
            }

            data.Seconds--;

            //Debug.Log(data.Seconds);
        }
    }

}
