
using System;
using System.Collections;
using System.Collections.Generic;


public class PAHT_FINDER_ENUM
{
    public static uint INVAILD_INDEX = 0x7FFFFFFF;
};

public interface IAStarGraph
{
	uint getCellCount();
    uint getCellIndex(Vector2f pos);

    float getHValue(uint cellIndex, Vector2f targetPoint);

	List<uint> getAdjanceCells(uint cellIndex);

	float getGValue(uint cellIndex_0, uint cellIndex_1, uint cellIndex_2);

    bool smoothPath(Vector2f startPoint,
                    Vector2f endPoint, 
				    List<uint> cellIndexs,
                    ref List<Vector2f> path);
	bool sameArea(uint cellIndex_1, uint cellIndex_2);
};

// 索引优先队列

public class IndexedPriorityQLow
{
  private float[]   m_vecKeys = null;
  private int[]     m_Heap = null;
  private int[]     m_invHeap = null;
  private int           m_iSize;
  private int           m_iMaxSize;

  void Swap(int a, int b)
  {
    int temp = m_Heap[a]; m_Heap[a] = m_Heap[b]; m_Heap[b] = temp;

    m_invHeap[m_Heap[a]] = a; m_invHeap[m_Heap[b]] = b;
  }

  void ReorderUpwards(int nd)
  {
    while ( (nd>1) && (m_vecKeys[m_Heap[nd/2]] > m_vecKeys[m_Heap[nd]]) )
    {      
      Swap(nd/2, nd);

      nd /= 2;
    }
  }

  void ReorderDownwards(int nd, int HeapSize)
  {
    while (2*nd <= HeapSize)
    {
      int child = 2 * nd;

      if ((child < HeapSize) && (m_vecKeys[m_Heap[child]] > m_vecKeys[m_Heap[child+1]]))
      {
        ++child;
      }

      if (m_vecKeys[m_Heap[nd]] > m_vecKeys[m_Heap[child]])
      {
        Swap(child, nd);
        nd = child;
      }

      else
      {
        break;
      }
    }
  }

  public IndexedPriorityQLow()
  {

  }

  public bool empty()
  {
      return m_iSize == 0;
  }

  public void insert(int idx)
  {
    ++m_iSize;

    m_Heap[m_iSize] = idx;

    m_invHeap[idx] = m_iSize;

    ReorderUpwards(m_iSize);
  }
 
  public int Pop()
  {
    Swap(1, m_iSize);

    ReorderDownwards(1, m_iSize-1);

    return m_Heap[m_iSize--];
  }

  public void ChangePriority(int idx)
  {
    ReorderUpwards(m_invHeap[idx]);
  }

  public void Reset()
  {
	m_iSize = 0;
  }

  public void Init(float[] keys)
  {
	  if(keys == null)
		return;
	  
      m_iMaxSize = keys.Length;
      m_vecKeys = keys;
      m_Heap    = new int[m_iMaxSize + 1];
      m_invHeap = new int[m_iMaxSize + 1];

      m_iSize = 0;

      for(int i = 0; i <= m_iMaxSize; i++)
      {
	      m_Heap[i]     = 0;
	      m_invHeap[i]  = 0;
      }
  }
};

public class pathFinder
{
    private IAStarGraph mGraph;

	private uint mSessionId;

	private float[] mFValues = null; 
	private float[] mGValues = null;
	private uint[]  mOpenTable = null;
	private uint[]  mCloseTable = null;
	private uint[]  mShortestPathTree = null;

	IndexedPriorityQLow mPriorityQueue = new IndexedPriorityQLow();

	public pathFinder()
	{
	}

	public bool init(IAStarGraph graph)
	{
		if(graph == null)
			return false;

		mGraph = graph;

        mFValues = new float[mGraph.getCellCount()];
        mGValues = new float[mGraph.getCellCount()];
        mOpenTable = new uint[mGraph.getCellCount()];
        mCloseTable = new uint[mGraph.getCellCount()];
        mShortestPathTree = new uint[mGraph.getCellCount()];

        for(int i = 0; i < mGraph.getCellCount(); i++)
        {
		    mFValues[i] = 0.0f;
            mGValues[i] = 0.0f;
		    mOpenTable[i] = 0;
		    mCloseTable[i] = 0;

            mShortestPathTree[i] = 0;
        }

		mPriorityQueue.Init(mFValues);
		
		mSessionId = 0;
		return true;
	}

	public bool findPath(Vector2f startPosition, 
                    Vector2f targetPosition,
                    out List<Vector2f> path)
	{
        path = new List<Vector2f>();

		if(mGraph == null)
			return false;

		if(mSessionId == 0xFFFFFFFF)
		{
			reset();
		}

		mSessionId++;

		mPriorityQueue.Reset();

		uint startCellIndex = mGraph.getCellIndex(targetPosition);
		uint endCellIndex	= mGraph.getCellIndex(startPosition);

		if(startCellIndex == (uint)PAHT_FINDER_ENUM.INVAILD_INDEX || endCellIndex == (uint)PAHT_FINDER_ENUM.INVAILD_INDEX)
			return false;

		if(startCellIndex == endCellIndex)
		{
			path.Add(startPosition);
			path.Add(targetPosition);
			return true;
		}

		if(!mGraph.sameArea(startCellIndex, endCellIndex))
			return false;

		mShortestPathTree[(int)startCellIndex] = (uint)PAHT_FINDER_ENUM.INVAILD_INDEX;
		mGValues[(int)startCellIndex] = 0;
		mFValues[(int)startCellIndex] = 0;
		mOpenTable[(int)startCellIndex] = mSessionId;
		mPriorityQueue.insert((int)startCellIndex);

		while(!mPriorityQueue.empty())
		{
			uint currentIndex = (uint)mPriorityQueue.Pop();
			mCloseTable[(int)currentIndex] = mSessionId;

			if(currentIndex == endCellIndex)
			{
				List<uint> cells = new List<uint>();
			
				uint cellIndex = (uint)currentIndex; 
				uint maxCycleTime = mGraph.getCellCount();

				for(uint i = 0; i < maxCycleTime; i++)
				{
					cells.Add(cellIndex);

					if(mShortestPathTree[(int)cellIndex] == (uint)PAHT_FINDER_ENUM.INVAILD_INDEX)
					{
						return mGraph.smoothPath(startPosition, targetPosition, cells, ref path);
					}
					else
					{
						cellIndex = mShortestPathTree[(int)cellIndex];
					}
				}

				return false;
			}
			else
			{
				List<uint> adjanceCells = mGraph.getAdjanceCells(currentIndex);
                for (int i = 0; adjanceCells != null && i < adjanceCells.Count; i++)
				{
                    uint adjanceIndex = adjanceCells[i];
					if(mShortestPathTree[(int)currentIndex] == adjanceIndex)
						continue;

					float hValue =  mGraph.getHValue(adjanceIndex, startPosition);
					float gValue =  mGValues[(int)currentIndex] + mGraph.getGValue(mShortestPathTree[(int)currentIndex], currentIndex, adjanceIndex);

					if(mOpenTable[(int)adjanceIndex] != mSessionId)
					{
						mGValues[(int)adjanceIndex] = gValue;
						mFValues[(int)adjanceIndex] = gValue + hValue;

						mShortestPathTree[(int)adjanceIndex] = currentIndex;
						mOpenTable[(int)adjanceIndex] = mSessionId;
						mPriorityQueue.insert((int)adjanceIndex);

					}
					else if(mCloseTable[(int)adjanceIndex] != mSessionId && gValue < mGValues[(int)adjanceIndex])
					{
						mGValues[(int)adjanceIndex] = gValue;
						mFValues[(int)adjanceIndex] = gValue + hValue;

						mShortestPathTree[(int)adjanceIndex] = currentIndex;
						mPriorityQueue.ChangePriority((int)adjanceIndex);
					}
				}
			}
		}

		return false;
	}

	public void reset()
	{
		mSessionId = 0;

        for(int i = 0; i < mOpenTable.Length; i++)
        {
		    mOpenTable[i] = 0;
        }

        for(int i = 0; i < mCloseTable.Length; i++)
        {
            mCloseTable[i] = 0;
        }
	}
	
};
