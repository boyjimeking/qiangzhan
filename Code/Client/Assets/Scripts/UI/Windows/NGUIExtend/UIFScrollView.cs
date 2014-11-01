//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// This script, when attached to a panel turns it into a scroll view.
/// You can then attach UIDragScrollView to colliders within to make it draggable.
/// </summary>

public class UIFScrollView : UIScrollView
{
    protected override bool shouldMove
    {
        get
        {
            if (!disableDragIfFits) return true;

            if (mPanel == null) mPanel = GetComponent<UIPanel>();
            Vector4 clip = mPanel.finalClipRegion;         
            Bounds b = bounds;
           

            if (canMoveHorizontally)
            {
                return (b.max.x - b.min.x) > clip.z;
            }

            if (canMoveVertically)
            {
                return (b.max.y - b.min.y) > clip.w;
            }

            return false;
        }
    }
}
