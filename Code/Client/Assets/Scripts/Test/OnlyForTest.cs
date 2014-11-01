using UnityEngine;
using System.Collections;
using System.IO;
using FantasyEngine;

public class OnlyForTest : MonoBehaviour
{

    public Player player;
    // Use this for initialization
    void Start()
    {
       //string strMD5 =  MD5Utils.Encrypt("123242");

       //string url = "http://autopatch.bh.173.com/binghuo/webres/776fb97c4fb9da80382593fcb125db79.bundle";

       //UpdateTool tool = new UpdateTool();
       //tool.BeginHttpDownload(url);

        //string streampath = Application.dataPath + "/../../../Bin/client/StreamingAssets/filelist.ab";

        //string streampathwithfile = "file://" + streampath;
        //if(File.Exists(streampath))
        //{
        //    Debug.Log(true);
        //}
        //if(File.Exists(streampathwithfile))
        //{
        //    Debug.Log(true);
        //}

        btnCreatePlayer();
    }

    public void btnCreatePlayer()
    {
        //PlayerInitParam param = new PlayerInitParam();
        //param.player_data = new PlayerData();
        //param.player_data.classs = 1;
        //player = new Player();

        //player.Init(param);
        //player.LoadModel();

        MeshVisual visual = new MeshVisual();
        visual.CreateWithConfig("role_male_roledefault",null,null);
    }

    public void btnPlayAnimation()
    {
        //AnimAction action = AnimActionFactory.Create(AnimActionFactory.E_Type.Idle);
        //player.GetAnimFSM().DoAction(action);


        //AnimActionPlayAnim animAction = AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;
        //animAction.AnimName = "Base Layer.die01";
        //player.GetAnimFSM().DoAction(animAction);

            AnimActionUseSkill animAction = AnimActionFactory.Create(AnimActionFactory.E_Type.UseSkill) as AnimActionUseSkill;
        animAction.AnimName = "pingchi.huandan";
        player.GetStateController().DoAction(animAction);
        //player.GetAnimFSM().DoAction(animAction);
    }


    public void Copy()
    {
        //UnityEngine.Object.Instantiate(player.__test__GetVisual().Visual);
    }
    // Update is called once per frame
    void Update()
    {
        if (player != null)
			player.Update((uint)Time.unscaledDeltaTime * 1000);
    }
}
