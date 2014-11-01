using UnityEngine;
using System.Collections;

public class TitleGroupUI
{
    public enum Dir
    {
        Up,
        Down,
    }

    protected UILabel mName;
    protected UISprite mDirSp;
    protected UISprite mSelectSp;
    protected UIGrid mGrid;

    private GameObject mGo;
    private bool mIsOpen = false;

    public TitleGroupUI(GameObject go)
    {
        mName = ObjectCommon.GetChildComponent<UILabel>(go, "name");
        mSelectSp = ObjectCommon.GetChildComponent<UISprite>(go, "selectSp");
        mDirSp = ObjectCommon.GetChildComponent<UISprite>(go, "dirSp");
        mGrid = ObjectCommon.GetChildComponent<UIGrid>(go, "Grid");
    }


    public void SetData(TitleGroupTableItem item)
    {
        if (item == null)
            return;

        mName.text = item.name;

        Init();
    }

    void Init()
    {
        //SetIsOpen(false);
    }

    void SetDir(Dir dir)
    {
        switch (dir)
        {
            case Dir.Up:
                mDirSp.spriteName = "up";
                break;
            case Dir.Down:
                mDirSp.spriteName = "down";
                break;
        }
    }

    void SetIsOpen(bool isOpen)
    {
        mSelectSp.gameObject.SetActive(isOpen);

        Dir dir = isOpen ? Dir.Down : Dir.Up;
        SetDir(dir);
    }

    public void OpenOrClose()
    {
        mIsOpen = !mIsOpen;

        SetIsOpen(mIsOpen);
    }

    public bool IsOpen()
    {
        return mIsOpen;
    }

    public void AddTitle(TitleItemTableItem item)
    {
 
    }

    public void AddChild(GameObject go)
    {
        if (go == null)
            return;

        go.transform.parent = mGrid.transform;
        go.transform.localScale = Vector3.one;

        mGrid.repositionNow = true;
    }
}
