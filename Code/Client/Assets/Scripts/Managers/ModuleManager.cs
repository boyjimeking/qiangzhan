
using System;
using System.Collections;
using System.Collections.Generic;


public class ModuleManager 
{
    private Hashtable mModuleHashs = new Hashtable();

    private List<ModuleBase> mModuleList = new List<ModuleBase>();

	private static ModuleManager instance;
    public ModuleManager()
	{
		instance = this;

        RegisterModule<ChatModule>();
		RegisterModule<SkillModule>();
        RegisterModule<ChallengeModule>();
        RegisterModule<PlayerDataModule>();
        RegisterModule<FunctionModule>();
        RegisterModule<WeaponModule>();
		RegisterModule<StageListModule>();
		RegisterModule<StageEndModule>();
		RegisterModule<StageReliveModule>();
        RegisterModule<DouBiMaoStageModule>();
        RegisterModule<QuestModule>();
        RegisterModule<ZombiesStageModule>();
		RegisterModule<StageBalanceModule>();
        RegisterModule<MallFormModule>();
        RegisterModule<MonsterFloodModule>();
        RegisterModule<DefenceModule>();
        RegisterModule<PlayerPropertyModule>();
        RegisterModule<ArenaModule>();
        RegisterModule<RankingModule>();
        RegisterModule<ShopModule>();
		RegisterModule<QualifyingModule>();
		RegisterModule<WingModule>();
        RegisterModule<EggModule>();
        RegisterModule<CreateRoleModule>();
        RegisterModule<GuideModule>();
        RegisterModule<WorldMapModule>();
        RegisterModule<FashionModule>();
        RegisterModule<MailModule>();
        RegisterModule<CropsModule>();
        RegisterModule<BigBagModle>();
        RegisterModule<BuyGameCoinsModule>();
        RegisterModule<FundModule>();
        RegisterModule<PlayerPlanModule>();
		RegisterModule<BattleUIModule>();
        RegisterModule<TitleModule>();
	}

    public static ModuleManager Instance
	{
		get
		{
			return instance;
		}
	}

    private void RegisterModule<T>()
    {
        Type type = typeof(T);
        if (mModuleHashs.ContainsKey(type))
        {
            return;
        }
        ModuleBase module = System.Activator.CreateInstance(type) as ModuleBase;
        module.Enable();            //todo
        mModuleHashs.Add(type, module);

        mModuleList.Add(module);
    }

    public T FindModule<T>()
    {
        Type type = typeof(T);
        if (!mModuleHashs.ContainsKey(type))
        {
            return default(T);
        }
        return (T)mModuleHashs[type];
    }
   
    public void Update(uint elapsed)
    {
        for( int i = 0 ; i < mModuleList.Count ; ++i )
        {
            ModuleBase module = mModuleList[i];
            if (module != null)
            {
                module.Update(elapsed);
            }
        }
    }
}
