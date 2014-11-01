using UnityEngine;
using System.Collections;

public class UIBloodStains : UIWindow
{
    public UISprite mSprite = null;

    private float mBeginAlpha = 0.5f;

    private const float mOpenScalse = 0.2f;

    private const float mFlickerScale = 0.1f;

    private float mHpScale = 1.0f;

    //private float mLastAlpha = 0.0f;
    private float mCurAlpha = 0.0f;

    private float mFlickerValue = 0.01f;

    private bool mBeginFlicker = false;

    private const float DamageTime = 0.8f;
    //private bool mDamageTimeBegin = false;
    //private float mDamageTime = 0.0f;

    private bool mEnable = true;


    //界面加载完成
    protected override void OnLoad()
    {
        mSprite = this.GetComponent<UISprite>();
    }
    //界面打开
    protected override void OnOpen(object param = null)
    {
        mEnable = true;
        mSprite.alpha = 0.0f;
        mCurAlpha = 0.0f;
        EventSystem.Instance.addEventListener(PropertyEvent.FIGHT_PROPERTY_CHANGE, onPlayerFightPropChange);
        EventSystem.Instance.addEventListener(BattleUIEvent.BATTLE_UI_PLAYER_DAMAGE, onBattleDamage);
        EventSystem.Instance.addEventListener(StagePassServerEvent.STAGE_PASS_SERVER_EVENT, onPassStage);
    }
    //界面关闭
    protected override void OnClose()
    {
        EventSystem.Instance.removeEventListener(PropertyEvent.FIGHT_PROPERTY_CHANGE, onPlayerFightPropChange);
        EventSystem.Instance.removeEventListener(BattleUIEvent.BATTLE_UI_PLAYER_DAMAGE, onBattleDamage);
		EventSystem.Instance.removeEventListener(StagePassServerEvent.STAGE_PASS_SERVER_EVENT, onPassStage);
    }
    public override void Update(uint elapsed)
    {
        if (!mEnable)
            return;

        if (mBeginFlicker)
        {
            mCurAlpha += mFlickerValue;
            if (mCurAlpha >= /*mLastAlpha*/1.0f || mCurAlpha <= mBeginAlpha)
            {
                mFlickerValue = -mFlickerValue;
            }
            mSprite.alpha = mCurAlpha;
        }
    }

    void onBattleDamage(EventBase e)
    {
        if (!mEnable)
            return;

        BattleUIEvent evt = (BattleUIEvent)e;
        if (!mBeginFlicker && evt.damage.Value < 0)
        {
            mSprite.alpha = 0.8f;
            TweenAlpha.Begin(mSprite.gameObject, 0.5f, 0.0f);

            //             mDamageTime = DamageTime;
            //             mDamageTimeBegin = true;
        }
    }

    void onPlayerFightPropChange(EventBase e)
    {
        if (!mEnable)
            return;

        PropertyEvent evt = (PropertyEvent)e;

		BattleUnit unit = PlayerController.Instance.GetControlObj() as BattleUnit;
		if (unit.GetMaxHP() <= 0)
        {
            mSprite.alpha = 0.0f;
        }
        else
        {
			mHpScale = (float)unit.GetHP() / (float)unit.GetMaxHP();

            if (mHpScale <= mOpenScalse)
            {
                float alpha = (mOpenScalse - mHpScale) / mOpenScalse + mBeginAlpha;

                if (!mBeginFlicker)
                {
                    mSprite.alpha = mCurAlpha = mBeginAlpha;
                    mBeginFlicker = true;
                }
                //mLastAlpha = 1.0f;
            }
            else
            {
                mSprite.alpha = 0.0f;
                mBeginFlicker = false;
            }
        }
    }

    void onPassStage(EventBase e)
    {
        mSprite.alpha = 0.0f;
        mEnable = false;
    }
}
