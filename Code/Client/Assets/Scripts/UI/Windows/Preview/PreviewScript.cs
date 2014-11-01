
using UnityEngine;
public class PreviewScript : MonoBehaviour
{
    public UIPreviewBase mPreview;

    public static PreviewScript sScript;
    void Awake()
    {
        sScript = this;
    }

    void OnEnable()
    {
        if (mPreview != null)
            mPreview.Enable = true;
    }
    void OnDisable()
    {
        if (mPreview != null)
            mPreview.Enable = false;
    }
    void OnDrag(Vector2 delta)
    {

        mPreview.Scroll(delta.x);
    }
}

