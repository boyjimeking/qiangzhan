using System.Collections.Generic;
public class AIFactory
{
    private static AIFactory instance;
    private Dictionary<int, BattleUnitAI> aiList = new Dictionary<int, BattleUnitAI>();
    public enum AIType : int
    {
        AI_TYPE_INVALID = -1,

     
        AI_TYPE_COMMON_BEGIN = 0,
        // 块内派生于CommonAI

        AI_TYPE_TEST = AI_TYPE_COMMON_BEGIN,       // 用于测试的AI

        AI_TYPE_KANDAO = 1,
        AI_TYPE_SHEJI  = 2,
        AI_TYPE_3 = 3,
        AI_TYPE_4 = 4,
        AI_TYPE_5 = 5,
        AI_TYPE_6 = 6,
        AI_TYPE_7 = 7,
        AI_TYPE_8 = 8,
        AI_TYPE_9 = 9,
        AI_TYPE_10 = 10,
        AI_TYPE_11 = 11,
        AI_TYPE_12 = 12,
        AI_TYPE_13 = 13,
        AI_TYPE_14 = 14,
        AI_TYPE_15 = 15,
        AI_TYPE_FOLLOW = 16, 
        AI_TYPE_17 = 17,
        AI_TYPE_18 = 18,
        AI_TYPE_19 = 19,
        AI_TYPE_XUNLUO = 20,
        AI_TYPE_21 = 21,
        AI_TYPE_22 = 22,
        AI_TYPE_23 = 23,
        AI_TYPE_24 = 24,
        AI_TYPE_25 = 25,
        AI_TYPE_26 = 26,
        AI_TYPE_27 = 27,
        AI_TYPE_28 = 28,
        AI_TYPE_29 = 29,
        AI_TYPE_30 = 30,
        AI_TYPE_31 = 31,
        AI_TYPE_32 = 32,
        AI_TYPE_33 = 33,
        AI_TYPE_34 = 34,
        AI_TYPE_35 = 35,
        AI_TYPE_36 = 36,
        AI_TYPE_MAO= 37,//逗比猫狂暴
        AI_TYPE_38 = 38,
        AI_TYPE_39 = 39,
        AI_TYPE_40 = 40,
        AI_TYPE_41 = 41,
        AI_TYPE_42 = 42,
        AI_TYPE_43 = 43,
        FortuneMaoAI = 44,
        XunLuoNoLoop = 45,
        AI_TYPE_46 = 46,
        AI_TYPE_47 = 47,
        AI_TYPE_48 = 48,
        
        AI_TYPE_200 = 200,
        AI_TYPE_201 = 201,
        AI_TYPE_202 = 202,

		AI_TYPE_1000= 1000,
		AI_TYPE_1001 = 1001,
        AI_TYPE_1002 = 1002,
        AI_TYPE_1003 = 1003,
        AI_TYPE_1004 = 1004,
        AI_TYPE_1005 = 1005,

        AI_TYPE_1100 = 1100,
        AI_TYPE_1101 = 1101,
        AI_TYPE_1102 = 1102,
        AI_TYPE_1103 = 1103,
        AI_TYPE_1104 = 1104,
        AI_TYPE_1105 = 1105,

        AI_TYPE_COMMON_END = 100000,
    };

    public AIFactory()
    {
        instance = this;
        aiList.Add((int)AIType.AI_TYPE_TEST, new TestAI(null));
        aiList.Add((int)AIType.AI_TYPE_KANDAO, new KanDaoAI(null));
        aiList.Add((int)AIType.AI_TYPE_SHEJI, new SheJiAI(null));
        aiList.Add((int)AIType.AI_TYPE_3, new LianShe2(null));
        aiList.Add((int)AIType.AI_TYPE_4, new SanShe(null));
        aiList.Add((int)AIType.AI_TYPE_5, new LianShe(null));
        aiList.Add((int)AIType.AI_TYPE_6, new QuanPao(null));
        aiList.Add((int)AIType.AI_TYPE_7, new ZhiZhu(null));
        aiList.Add((int)AIType.AI_TYPE_8, new MuNaiYi(null));
        aiList.Add((int)AIType.AI_TYPE_9, new AIType_9(null));
        aiList.Add((int)AIType.AI_TYPE_10, new AIType_10(null));
        aiList.Add((int)AIType.AI_TYPE_11, new AIType_11(null));
        aiList.Add((int)AIType.AI_TYPE_12, new AIType_12(null));
        aiList.Add((int)AIType.AI_TYPE_13, new AIType_13(null));
        aiList.Add((int)AIType.AI_TYPE_14, new AIType_14(null));
        aiList.Add((int)AIType.AI_TYPE_15, new AIType_15(null));
        aiList.Add((int)AIType.AI_TYPE_FOLLOW, new FollowAI(null));
        aiList.Add((int)AIType.AI_TYPE_17, new AIType_17(null));
        aiList.Add((int)AIType.AI_TYPE_18, new AIType_18(null));
        aiList.Add((int)AIType.AI_TYPE_19, new AIType_19(null));
        aiList.Add((int)AIType.AI_TYPE_XUNLUO, new XunLuo(null));
        aiList.Add((int)AIType.AI_TYPE_21, new AIType_21(null));
        aiList.Add((int)AIType.AI_TYPE_22, new AIType_22(null));
        aiList.Add((int)AIType.AI_TYPE_23, new AIType_23(null));
        aiList.Add((int)AIType.AI_TYPE_24, new AIType_24(null));
        aiList.Add((int)AIType.AI_TYPE_25, new AIType_25(null));
        aiList.Add((int)AIType.AI_TYPE_26, new AIType_26(null));
        aiList.Add((int)AIType.AI_TYPE_27, new AIType_27(null));
        aiList.Add((int)AIType.AI_TYPE_28, new AIType_28(null));

        aiList.Add((int)AIType.AI_TYPE_29, new AIType_29(null));
        aiList.Add((int)AIType.AI_TYPE_30, new AIType_30(null));
        aiList.Add((int)AIType.AI_TYPE_31, new AIType_31(null));

        aiList.Add((int)AIType.AI_TYPE_32, new AIType_32(null));
        aiList.Add((int)AIType.AI_TYPE_33, new AIType_33(null));
        aiList.Add((int)AIType.AI_TYPE_34, new AIType_34(null));
        aiList.Add((int)AIType.AI_TYPE_35, new AIType_35(null));
        aiList.Add((int)AIType.AI_TYPE_36, new AIType_36(null));
		aiList.Add((int)AIType.AI_TYPE_MAO, new MaoAI(null));
        aiList.Add((int)AIType.AI_TYPE_38, new AIType_38(null));
        aiList.Add((int)AIType.AI_TYPE_39, new AIType_39(null));
        aiList.Add((int)AIType.AI_TYPE_40, new AIType_40(null));
        aiList.Add((int)AIType.AI_TYPE_41, new AIType_41(null));
        aiList.Add((int)AIType.AI_TYPE_42, new AIType_42(null));
        aiList.Add((int)AIType.AI_TYPE_43, new AIType_43(null));
        aiList.Add((int)AIType.FortuneMaoAI, new FortuneMaoAI(null));
        aiList.Add((int)AIType.XunLuoNoLoop, new XunLuoNoLoop(null));
        aiList.Add((int)AIType.AI_TYPE_46, new AIType_46(null));
        aiList.Add((int)AIType.AI_TYPE_47, new AIType_47(null));
        aiList.Add((int)AIType.AI_TYPE_48, new AIType_48(null));

        aiList.Add((int)AIType.AI_TYPE_200, new SheJiShouWei(null));
        aiList.Add((int)AIType.AI_TYPE_201, new QuanDaZhuang(null));
        aiList.Add((int)AIType.AI_TYPE_202, new PaoDaZhuang(null));

        aiList.Add((int)AIType.AI_TYPE_1000, new AIType_1000(null));
        aiList.Add((int)AIType.AI_TYPE_1001, new AIType_1001(null));
        aiList.Add((int)AIType.AI_TYPE_1002, new AIType_1002(null));
        aiList.Add((int)AIType.AI_TYPE_1003, new AIType_1003(null));
        aiList.Add((int)AIType.AI_TYPE_1004, new AIType_1004(null));
        aiList.Add((int)AIType.AI_TYPE_1005, new AIType_1005(null));

        aiList.Add((int)AIType.AI_TYPE_1100, new AIType_1100(null));
        aiList.Add((int)AIType.AI_TYPE_1101, new AIType_1101(null));
        aiList.Add((int)AIType.AI_TYPE_1102, new AIType_1102(null));
        aiList.Add((int)AIType.AI_TYPE_1103, new AIType_1103(null));
        aiList.Add((int)AIType.AI_TYPE_1104, new AIType_1104(null));
        aiList.Add((int)AIType.AI_TYPE_1105, new AIType_1105(null));
    }

    public static AIFactory Instance
    {
        get
        {
            return instance;
        }
    }

    public BattleUnitAI CreateAIObject(BattleUnit battleUnit, int resID)
    {
        if (!DataManager.AITable.ContainsKey(resID))
            return null;

        AITableItem aiItem = DataManager.AITable[resID] as AITableItem;
        if (aiItem == null)
            return null;

        if (!aiList.ContainsKey(aiItem.type))
            return null;

        BattleUnitAI ai = aiList[aiItem.type].CreateAIType(battleUnit);
        if (ai == null || !ai.Init(resID))
            return null;

        return ai;
    }
};