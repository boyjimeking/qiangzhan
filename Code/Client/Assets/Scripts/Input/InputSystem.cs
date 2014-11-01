
using UnityEngine;


public class InputSystem  
{
    //private float m_fJoystickAxisY = 0.0f;
    //private float m_fJoystickAxisX = 0.0f;

    private bool mLockMove = false;

	private static InputSystem instance = null;

	public static InputSystem Instance
	{
		get
		{
			return instance;
		}
	}
    public InputSystem()
	{
		instance = this;
	}

    public void SetLockMove(bool _lock)
    {
        mLockMove = _lock;
    }

    public void Update()
    {
        BaseScene scn = SceneManager.Instance.GetCurScene();
        if (scn == null)
            return;

		//当有输入控件被激活时，屏蔽按键;
		if(WindowManager.Instance.HasInputActive())
		{
			return;
		}

        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

		//正在摇杆操作
        if (MyJoystick.joystickAxis != Vector2.zero)
        {
            v = MyJoystick.joystickAxis.y;
            h = MyJoystick.joystickAxis.x;
        }

        if (!mLockMove && (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f) )
        {
            Camera  camera = CameraController.Instance.CurCamera;
            //当前控制的摄像机
            if( camera == null )
            {
                return;
            }
            Vector3 forward = camera.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;

            Vector3 right = new Vector3(forward.z, 0, -forward.x);

            Vector3 targetDirection = h * right + v * forward;

            if (targetDirection != Vector3.zero)
            {
                Vector3 moveDirection = targetDirection.normalized;

                Quaternion rot = Quaternion.LookRotation(moveDirection);

                PlayerController.Instance.MoveDir(rot.eulerAngles.y * Mathf.Deg2Rad);
            }
        }
        else
        {
            PlayerController.Instance.StopDir();
        }



        //移动
        if(Input.GetKeyDown(KeyCode.M))
        {
            
            SoundManager.Instance.Play(93);
        }else if( Input.GetKeyDown(KeyCode.Q) )
        {
            //PopTipManager.Instance.AddNewTip("aaaaaaaaaaaa--" + UnityEngine.Random.Range(0, 10).ToString());
            //PromptUIManager.Instance.AddNewPrompt("DDDDDDDDDDDDD_");

        }
	}

    public void Termination()
    {
    }
}
