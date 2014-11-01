using System;
using System.Collections;
using System.Collections.Generic;

public interface IElement
{
    bool TestContain(float fMinX, float fMaxX, float fMinZ, float fMaxZ); // 这个元素被包含在QuadTreeNode2节点中
};

public interface ITestIntersect
{
    bool TestIntersect(float fMinX, float fMaxX, float fMinZ, float fMaxZ);	// 本对象是否和一个QuadTreeNode2相交
    bool TestIntersect(IElement element);		// 测试是否和TreeNode中的一个元素相交
};


public interface ITraversal
{
    void Visit(IElement element);
};

public class QuadTreeNode
{
    public float m_fMinX;
    public float m_fMaxX;
    public float m_fMinZ;
    public float m_fMaxZ;

    protected float m_fTLMinX;
    protected float m_fTLMaxX;
    protected float m_fTLMinZ;
    protected float m_fTLMaxZ;

    protected float m_fTRMinX;
    protected float m_fTRMaxX;
    protected float m_fTRMinZ;
    protected float m_fTRMaxZ;

    protected float m_fBLMinX;
    protected float m_fBLMaxX;
    protected float m_fBLMinZ;
    protected float m_fBLMaxZ;

    protected float m_fBRMinX;
    protected float m_fBRMaxX;
    protected float m_fBRMinZ;
    protected float m_fBRMaxZ;

    protected int m_nDepth;
    protected QuadTreeNode m_pParent;
    protected QuadTree m_pTree;

    protected QuadTreeNode m_pTopLeft;
    protected QuadTreeNode m_pTopRight;
    protected QuadTreeNode m_pBottomLeft;
    protected QuadTreeNode m_pBottomRight;

    List<IElement> m_lstElement = new List<IElement>();

    public	QuadTreeNode()
    {
        m_pTopLeft = null;
        m_pTopRight = null;
        m_pBottomLeft = null;
        m_pBottomRight = null;
    }

	public void Init( float fMinX, float fMaxX, float fMinZ, float fMaxZ, int nDepth, QuadTreeNode parent, QuadTree pTree )
    {
        m_pTopLeft = null;
        m_pTopRight = null;
        m_pBottomLeft = null;
        m_pBottomRight = null;
        m_pParent = parent;
        m_pTree = pTree;
        m_nDepth = nDepth;
        m_fMinX = fMinX;
        m_fMaxX = fMaxX;
        m_fMinZ = fMinZ;
        m_fMaxZ = fMaxZ;

        float x = Math.Abs(fMaxX - fMinX);
        float z = Math.Abs(fMaxZ - fMinZ);
        float fCenterX = fMinX + x / 2;
        float fCenterZ = fMinZ + z / 2;

        m_fTLMinX = fCenterX - x / 2;
        m_fTLMaxX = fCenterX;
        m_fTLMinZ = fCenterZ;
        m_fTLMaxZ = fCenterZ + z / 2;

        m_fTRMinX = fCenterX;
        m_fTRMaxX = fCenterX + x / 2;
        m_fTRMinZ = fCenterZ;
        m_fTRMaxZ = fCenterZ + z / 2;

        m_fBLMinX = fCenterX - x / 2;
        m_fBLMaxX = fCenterX;
        m_fBLMinZ = fCenterZ - z / 2;
        m_fBLMaxZ = fCenterZ;

        m_fBRMinX = fCenterX;
        m_fBRMaxX = fCenterX + x / 2;
        m_fBRMinZ = fCenterZ - z / 2;
        m_fBRMaxZ = fCenterZ;
    }

	public void AddElement( IElement pElement )
    {
        if (pElement == null) 
            return;

        if (m_nDepth > 0)
        {
            if (pElement.TestContain(m_fTLMinX, m_fTLMaxX, m_fTLMinZ, m_fTLMaxZ))
            {
                if (m_pTopLeft == null)
                {
                    m_pTopLeft = m_pTree.CreateTreeNode();
                    m_pTopLeft.Init(m_fTLMinX, m_fTLMaxX, m_fTLMinZ, m_fTLMaxZ, m_nDepth - 1, this, m_pTree);
                }
                m_pTopLeft.AddElement(pElement);
                return;
            }
            else if (pElement.TestContain(m_fTRMinX, m_fTRMaxX, m_fTRMinZ, m_fTRMaxZ))
            {
                if (m_pTopRight == null)
                {
                    m_pTopRight = m_pTree.CreateTreeNode();
                    m_pTopRight.Init(m_fTRMinX, m_fTRMaxX, m_fTRMinZ, m_fTRMaxZ, m_nDepth - 1, this, m_pTree);
                }
                m_pTopRight.AddElement(pElement);
                return;
            }
            else if (pElement.TestContain(m_fBLMinX, m_fBLMaxX, m_fBLMinZ, m_fBLMaxZ))
            {
                if (m_pBottomLeft == null)
                {
                    m_pBottomLeft = m_pTree.CreateTreeNode();
                    m_pBottomLeft.Init(m_fBLMinX, m_fBLMaxX, m_fBLMinZ, m_fBLMaxZ, m_nDepth - 1, this, m_pTree);
                }
                m_pBottomLeft.AddElement(pElement);
                return;
            }
            else if (pElement.TestContain(m_fBRMinX, m_fBRMaxX, m_fBRMinZ, m_fBRMaxZ))
            {
                if (m_pBottomRight == null)
                {
                    m_pBottomRight = m_pTree.CreateTreeNode();
                    m_pBottomRight.Init(m_fBRMinX, m_fBRMaxX, m_fBRMinZ, m_fBRMaxZ, m_nDepth - 1, this, m_pTree);
                }
                m_pBottomRight.AddElement(pElement);
                return;
            }
        }

        m_lstElement.Add(pElement);
	
    }

	public void RemoveElement( IElement pElement )
    {
    }

	public void CollectIntersect( ITestIntersect pTestIntersect, ref List<IElement> rst )
    {
        if (pTestIntersect == null)
            return;

        if (m_pTopLeft != null && pTestIntersect.TestIntersect(m_pTopLeft.m_fMinX, m_pTopLeft.m_fMaxX,
                m_pTopLeft.m_fMinZ, m_pTopLeft.m_fMaxZ))
        {
            m_pTopLeft.CollectIntersect(pTestIntersect, ref rst);
        }

        if (m_pTopRight != null && pTestIntersect.TestIntersect(m_pTopRight.m_fMinX, m_pTopRight.m_fMaxX,
                m_pTopRight.m_fMinZ, m_pTopRight.m_fMaxZ))
        {
            m_pTopRight.CollectIntersect(pTestIntersect, ref rst);
        }

        if (m_pBottomLeft != null && pTestIntersect.TestIntersect(m_pBottomLeft.m_fMinX, m_pBottomLeft.m_fMaxX,
                m_pBottomLeft.m_fMinZ, m_pBottomLeft.m_fMaxZ))
        {
            m_pBottomLeft.CollectIntersect(pTestIntersect, ref rst);
        }

        if (m_pBottomRight != null && pTestIntersect.TestIntersect(m_pBottomRight.m_fMinX, m_pBottomRight.m_fMaxX,
                m_pBottomRight.m_fMinZ, m_pBottomRight.m_fMaxZ))
        {
            m_pBottomRight.CollectIntersect(pTestIntersect, ref rst);
        }

        foreach(IElement element in m_lstElement)
        {
            if(pTestIntersect.TestIntersect(element))
            {
                rst.Add(element);
            }
        }

    }

	public bool IntersectElement( ITestIntersect pTestIntersect, IElement pIntersectElement)
    {
        if (pTestIntersect == null) 
            return false;

        if (!pTestIntersect.TestIntersect(m_fMinX, m_fMaxX, m_fMinZ, m_fMaxZ)) 
            return false;

        if (m_pTopLeft != null && m_pTopLeft.IntersectElement(pTestIntersect, pIntersectElement))
            return true;

        if (m_pTopRight != null && m_pTopRight.IntersectElement(pTestIntersect, pIntersectElement)) 
            return true;

        if (m_pBottomLeft != null && m_pBottomLeft.IntersectElement(pTestIntersect, pIntersectElement)) 
            return true;

        if (m_pBottomRight != null && m_pBottomRight.IntersectElement(pTestIntersect, pIntersectElement))
            return true;

        foreach(IElement element in m_lstElement)
        {
            if(pTestIntersect.TestIntersect(element))
            {
                pIntersectElement = element;
                return true;
            }
        }

        return false;
    }

	public void Traversal( ITraversal pTraversal )
    {
        if (pTraversal == null) 
            return;

        if (m_pTopLeft != null) 
            m_pTopLeft.Traversal(pTraversal);

        if (m_pTopRight != null) 
            m_pTopRight.Traversal(pTraversal);

        if (m_pBottomLeft != null) 
            m_pBottomLeft.Traversal(pTraversal);

        if (m_pBottomRight != null) 
            m_pBottomRight.Traversal(pTraversal);

        foreach(IElement element in m_lstElement)
        {
            pTraversal.Visit(element);
        }
    }

	public void SetDepth( int nDepth ) 
    {
        m_nDepth = nDepth; 
    }

	public int GetDepth()
    {
        return m_nDepth;
    }
};


public class QuadTree
{
    private float m_fMinX;
    private float m_fMaxX;
    private float m_fMinZ;
    private float m_fMaxZ;

    private int m_nMaxDepth;
    private QuadTreeNode m_pRoot;
    private List<QuadTreeNode> m_Nodes = new List<QuadTreeNode>();

    private List<IElement> m_lstElement = new List<IElement>();

    public QuadTree()
    {

    }

    public void Clear()
	{
		if ( m_pRoot == null ) 
            return;

		m_pRoot = null;
		m_nMaxDepth = 0;
		
		m_Nodes.Clear();
		m_lstElement.Clear();
	}

    public QuadTreeNode CreateTreeNode()
	{
		QuadTreeNode pNode = new QuadTreeNode();
		m_Nodes.Add( pNode );
		return pNode;
	}

    public void CreateTree( float fMinX, float fMaxX, float fMinZ, float fMaxZ, int nMaxDepth = 5)
	{
		Clear();

		m_fMinX = fMinX;
		m_fMaxX = fMaxX;
		m_fMinZ = fMinZ;
		m_fMaxZ = fMaxZ;

		m_nMaxDepth = nMaxDepth;
		m_pRoot = CreateTreeNode();
		if ( m_pRoot != null )
		{
			m_pRoot.Init( m_fMinX, m_fMaxX, m_fMinZ, m_fMaxZ, m_nMaxDepth, null, this );
		}
		else
		{
			Clear();
		}
	}

	public void CollectIntersect( ITestIntersect pTestIntersect, ref List<IElement> rst )
	{
		if ( m_pRoot == null ) 
            return;

        if (pTestIntersect == null)
            return;

        rst.Clear();

		if ( pTestIntersect.TestIntersect( m_pRoot.m_fMinX, m_pRoot.m_fMaxX,
				m_pRoot.m_fMinZ, m_pRoot.m_fMaxZ) )
		{
			m_pRoot.CollectIntersect( pTestIntersect, ref rst );
		}

        foreach(IElement element in m_lstElement)
        {
            if(pTestIntersect.TestIntersect(element))
            {
                rst.Add(element);
            }
        }
	}


	public void AddElement( IElement pElement )
	{
		if ( pElement == null ) 
            return;

		if ( pElement.TestContain( m_pRoot.m_fMinX, m_pRoot.m_fMaxX, m_pRoot.m_fMinZ, m_pRoot.m_fMaxZ ) )
		{
			m_pRoot.AddElement( pElement );
		}
		else
		{
			m_lstElement.Add( pElement );
		}
	}

	public void RemoveElement( IElement pElement )
	{
		if ( m_pRoot == null ) 
            return;

		if ( pElement.TestContain( m_pRoot.m_fMinX, m_pRoot.m_fMaxX, m_pRoot.m_fMinZ, m_pRoot.m_fMaxZ ) )
		{
			m_pRoot.RemoveElement( pElement );
		}
		else
		{
			m_lstElement.Remove( pElement );
		}
	}

	public bool IntersectElement( ITestIntersect pTestIntersect, IElement pIntersectElement = null) 
	{
		if ( m_pRoot == null ) 
			return false;

		if ( m_pRoot.IntersectElement( pTestIntersect, pIntersectElement ) ) 
			return true;
		
        foreach(IElement element in m_lstElement)
        {
            if(pTestIntersect.TestIntersect(element))
            {
                if(pIntersectElement != null)
                {
                   pIntersectElement = element; 
                }
            }
        }

		return false;
	}

	public void Traversal( ITraversal pTraversal )
	{
		if ( m_pRoot == null ) 
            return;

        if (pTraversal == null)
            return;

		m_pRoot.Traversal( pTraversal );

        foreach(IElement element in m_lstElement)
        {
            pTraversal.Visit(element);
        }
	}

	public int GetMaxDepth()
    { 
        return m_nMaxDepth;
    }
}
