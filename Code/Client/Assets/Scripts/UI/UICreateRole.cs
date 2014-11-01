using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UICreateRole : UIWindow
{

    public GameObject loginButtonOK;
    public GameObject loginButtonReturn;
    public GameObject randNameButton;
    public GameObject createRoleButton;
    public GameObject createName;
    public GameObject manButton;
    public GameObject girlButton;
    public UIInput mUserName;
    public UILabel mMessage;
    public UILabel mMessage1;
    public UIButton manBut;
    public UIButton girlBut;
    public UILabel roleText;
    public UISprite playerIcon;
    public UISprite playerModel;
    public UISprite manSpecial;
    public UISprite girlSpecial;
    public UISprite IconEffects;
    protected override void OnLoad()
    {
        base.OnLoad();

        createRoleButton = this.FindChild("CreateRoleBut");
        loginButtonOK = this.FindChild("ButtonOK");
        loginButtonReturn = this.FindChild("ButtonReturn");
        randNameButton = this.FindChild("randomname");
        mUserName = this.FindComponent<UIInput>("CreateName/CreateNameBK/Username");
        mMessage = this.FindComponent<UILabel>("CreateName/CreateNameBK/message");
        mMessage1 = this.FindComponent<UILabel>("BK/errormessage");
        createName = this.FindChild("CreateName");
        manButton = this.FindChild("ManButton");
        girlButton = this.FindChild("GirlButton");
        manBut = this.FindComponent<UIButton>("ManButton");
        girlBut = this.FindComponent<UIButton>("GirlButton");
        roleText = this.FindComponent<UILabel>("BK/RoleTextBK/RoleText");
        playerIcon = this.FindComponent<UISprite>("CreateName/CreateNameBK/Username/Sprite/PlayerIcon");
        IconEffects = this.FindComponent<UISprite>("CreateName/CreateNameBK/Username/Sprite/IconEffects");
        playerModel = this.FindComponent<UISprite>("roleModel");
        girlSpecial = this.FindComponent<UISprite>("BK/choserole/GirlButton/Girl");
        manSpecial = this.FindComponent<UISprite>("BK/choserole/ManButton/Man");
        UIEventListener.Get(loginButtonOK).onClick = onLoginButtonClickOK;
        UIEventListener.Get(randNameButton).onClick = onRandomNameClick;
        UIEventListener.Get(createRoleButton).onClick = onCreateRoleClick;
        UIEventListener.Get(manButton).onClick = onManButtonClick;
        UIEventListener.Get(girlButton).onClick = onGirlButtonClick;
        UIEventListener.Get(loginButtonReturn).onClick = onLoginButtonClickReturn;
        RoleModelScript script = playerModel.gameObject.GetComponent<RoleModelScript>();
        if (script == null)
        {
            script = playerModel.gameObject.AddComponent<RoleModelScript>();
        }
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen();

        EventSystem.Instance.addEventListener(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE_RST, onRst);
        EventSystem.Instance.addEventListener(CreateRoleEvent.LOGIN_EVENT_GET_RANDOM_NAME_RST, onRandomNameRst);

        CreateRoleModule module = ModuleManager.Instance.FindModule<CreateRoleModule>();
        module.SetRoleID(1);
        PlayerTableItem items = DataManager.PlayerTable[module.GetRoleID()] as PlayerTableItem;
        if (items != null)
        {
            roleText.text = items.desc;
        }
        UIAtlasHelper.SetButtonImage(girlBut, "login:girlan", true);
        NGUITools.SetActive(girlSpecial.gameObject, false);
    }

    protected override void OnClose()
    {
        base.OnClose();

        EventSystem.Instance.removeEventListener(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE_RST, onRst);
        EventSystem.Instance.removeEventListener(CreateRoleEvent.LOGIN_EVENT_GET_RANDOM_NAME_RST, onRandomNameRst);
    }

	void Start ()
    {
	}

    void onLoginButtonClickOK(GameObject target)
    {
        Environment.Operation = 0;

        string username = NGUIText.StripSymbols(mUserName.value);
  
        if (string.IsNullOrEmpty(username))
        {
            mMessage.text = "[ff0000]用户名不可为空";
            return;
        }
        if (StrFilterManager.Instance.CheckBlacklist(username) == true)
        {
            mMessage.text = "[ff0000]您的用户名包含敏感字";
            return;
        }
        CreateRoleModule module = ModuleManager.Instance.FindModule<CreateRoleModule>();
        uint Roleid = module.GetRoleID();
        if (Roleid !=1 && Roleid !=2)
        {
            mMessage.text = "[ff0000]请返回重新选择人物角色";
            return;
        }
        NGUITools.SetActive(IconEffects.gameObject, true);
        CreateRoleEvent e = new CreateRoleEvent(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE);
        e.UserName = username;
        e.id = Roleid;

        EventSystem.Instance.PushEvent(e);
    }

    void onRandomNameClick(GameObject target)
    {
        randNameButton.GetComponent<UIButton>().isEnabled = false;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_RANDOM_NAME, null);
    }

    void onRst(EventBase evt)
    {
        CreateRoleEvent ce = evt as CreateRoleEvent;
        if (ce == null)
            return;

        mMessage.text = "[ff0000]" + ce.Message;
    }

    void onRandomNameRst(EventBase evt)
    {
        CreateRoleEvent ce = evt as CreateRoleEvent;
        if (ce == null)
            return;

        mUserName.value = ce.UserName;

        randNameButton.GetComponent<UIButton>().isEnabled = true;
    }

	void Update () 
    {
	
	}
    void onCreateRoleClick(GameObject target)
    {
        CreateRoleModule module = ModuleManager.Instance.FindModule<CreateRoleModule>();
        uint Roleid = module.GetRoleID();
        if (Roleid != 1 && Roleid != 2)
        {
            mMessage1.text = "[ff0000]请选择一个人物";
            return;
        }
        NGUITools.SetActive(createName.gameObject, true);
        PlayerTableItem items = DataManager.PlayerTable[Roleid] as PlayerTableItem;
        if (items == null)
        {
            return;
        }
        UIAtlasHelper.SetSpriteImage(playerIcon, items.face, true);

    }
   void onManButtonClick(GameObject target)
    {
        CreateRoleModule module = ModuleManager.Instance.FindModule<CreateRoleModule>();
        uint Roleid = module.GetRoleID();
        if (Roleid == 2)
        {
            UIAtlasHelper.SetButtonImage(girlBut, "login:girlan", true);
            NGUITools.SetActive(girlSpecial.gameObject, false);
        }
        if (Roleid != 1)
        {
            module.SetRoleID(1);
            NGUITools.SetActive(manSpecial.gameObject, true);
            PlayerTableItem items = DataManager.PlayerTable[module.GetRoleID()] as PlayerTableItem;
            if (items == null)
            {
                return;
            }
            roleText.text = items.desc;
        }
    }
   void onGirlButtonClick(GameObject target)
   {
       CreateRoleModule module = ModuleManager.Instance.FindModule<CreateRoleModule>();
       uint Roleid = module.GetRoleID();
       if (Roleid == 1)
       {
           UIAtlasHelper.SetButtonImage(manBut, "login:manan", true);
           NGUITools.SetActive(manSpecial.gameObject, false);
       }
       if (Roleid != 2)
       {
           module.SetRoleID(2);
           NGUITools.SetActive(girlSpecial.gameObject, true);
           PlayerTableItem items = DataManager.PlayerTable[module.GetRoleID()] as PlayerTableItem;
           if (items == null)
           {
               return;
           }
           roleText.text = items.desc;
       }
   }
   void onLoginButtonClickReturn(GameObject target)
   {
       CreateRoleModule module = ModuleManager.Instance.FindModule<CreateRoleModule>();
       mUserName.value = "";
       module.SetRoleID(1);
       NGUITools.SetActive(manSpecial.gameObject, true);
       PlayerTableItem items = DataManager.PlayerTable[module.GetRoleID()] as PlayerTableItem;
       if (items == null)
       {
           return;
       }
       roleText.text = items.desc;
       mMessage1.text = "";
       mMessage.text = "";
       UIAtlasHelper.SetButtonImage(girlBut, "login:girlan", true);
       NGUITools.SetActive(girlSpecial.gameObject, false);
       NGUITools.SetActive(createName.gameObject, false);
   }
}
