using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GuideHelper
{
    public static Vector3 GetControlPos(GameObject control)
    {
        if (control == null)
            return Vector3.zero;
        //在场景里未显示
        if (!control.activeInHierarchy)
            return Vector3.zero;
        //WorldToScreenPoint 效率如何?  考虑不在Update里处理
        return WindowManager.current2DCamera.WorldToScreenPoint(control.transform.position);
    }

    public static int GetControlHeight(GameObject control)
    {
        if (control == null)
            return 0;
        UIWidget widget = control.gameObject.GetComponent<UIWidget>();
        if (widget != null)
        {
            return widget.height;
        }
        return 0;
    }

    public static int GetControlWidth(GameObject control)
    {
        if (control == null)
            return 0;
        UIWidget widget = control.gameObject.GetComponent<UIWidget>();
        if (widget != null)
        {
            return widget.width;
        }
        return 0;
    }

    public static void UpdateUIPos(GameObject control,ArrowRot rot,ArrawUI arraw)
    {
      var pos=  GetControlPos(control);

      //计算二维坐标的时候 必须加入SpectRatio 做屏幕适应
      float spectRatio = WindowManager.GetSpectRatio();
      int ctrlWidth = GetControlWidth(control);
      int ctrlHeight =GetControlHeight(control);

      if (rot == ArrowRot.ArrowRot_Invaild)
      {
          //控件在屏幕左边
          if (pos.x < Screen.width / 2)
          {
              rot = ArrowRot.ArrowRot_RIGHT;
          }
          else
          {
              rot = ArrowRot.ArrowRot_LEFT;
          }
      }


      Vector2 arrowPos = new Vector2();
      if (rot == ArrowRot.ArrowRot_LEFT)
      {
          arrowPos.y = pos.y;
          arrowPos.x = pos.x - (ctrlWidth / 2 + arraw.GetArrowWidth() / 2) / spectRatio;
      }
      else if (rot == ArrowRot.ArrowRot_RIGHT)
      {
          arrowPos.y = pos.y;
          arrowPos.x = pos.x + (ctrlWidth / 2 + arraw.GetArrowWidth() / 2) / spectRatio;
      }
      else if (rot == ArrowRot.ArrowRot_UP)
      {
          arrowPos.x = pos.x;
          arrowPos.y = pos.y + (ctrlHeight / 2 + arraw.GetArrowHeight() / 2) / spectRatio;
      }
      else if (rot == ArrowRot.ArrowRot_DOWN)
      {
          arrowPos.x = pos.x;
          arrowPos.y = pos.y - (ctrlHeight / 2 + arraw.GetArrowHeight() / 2) / spectRatio;
      }
     
      arraw.SetArrowRot(rot);
      arraw.SetEffectPos(WindowManager.current2DCamera.ScreenToWorldPoint(pos));
      arraw.SetEffectSize(ctrlWidth, ctrlHeight);
      arraw.SetArrowPos(WindowManager.current2DCamera.ScreenToWorldPoint(arrowPos));

    }
}
