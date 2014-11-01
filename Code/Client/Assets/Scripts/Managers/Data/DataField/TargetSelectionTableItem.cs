public class TargetSelectionTableItem
{
	public int resID;
	public string desc;

	public uint maxTargetCount;

	public LeagueSelection leagueSel;

    public ShapeType shape;

	// 不要直接访问shapeArg.
	public float shapeArg0;
	public float shapeArg1;

	// 取形状参数的属性, 避免混淆.
	public float CircleRadius
	{
		get
		{
			AssertShape(ShapeType.ShapeType_Round);
			return shapeArg0;
		}
		set
		{
			AssertShape(ShapeType.ShapeType_Round); 
			shapeArg0 = value;
		}
	}

	public float RectLength
	{
		get
		{
			AssertShape(ShapeType.ShapeType_Rect);
			return shapeArg0;
		}
		set
		{
			AssertShape(ShapeType.ShapeType_Rect);
			shapeArg0 = value;
		}
	}

	public float RectWidth
	{
		get
		{
			AssertShape(ShapeType.ShapeType_Rect);
			return shapeArg1;
		}
		set
		{
			AssertShape(ShapeType.ShapeType_Rect);
			shapeArg1 = value;
		}
	}

	void AssertShape(ShapeType argument) {
		if (shape != argument)
			throw new System.ArgumentException("形状类型不匹配");
	}
}
