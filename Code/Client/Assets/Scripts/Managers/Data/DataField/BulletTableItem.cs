/// <summary>
/// �ӵ�����.
/// </summary>
public class BulletTableItem
{
	public int resID;
	public string desc;

    public BulletType type;

	// ��ЧID.
	public uint bulletFigureID;

	// Ŀ��λ�õ�Ԥ����Ч.
	public uint effectOnTargetPosition;

	// �����ٶ�.
	public float flySpeed;

    // ���ٶ�.
    public float accelerateSpeed;

	// ���ٶ��ӳ�.
	public uint accelerateDelay;

	// ���.
	public uint flyRange;

	// ���ж����˺���ʧ.
	public uint flyHitCount;

	// ��ײʱ����Ӫʶ��.
	public LeagueSelection leagueSelection;

	// ����ʱ����ײ�뾶(�����ӵ�����ĵ�λ��Ч).
	public float radiusOnCollide;

	// �Ա������߲�����Ч��(skilleffect.txt).
	public uint skilleffectOnFlyCollide;

	// ��ըʱ�ڳ����ڲ��ű�ը��Ч.
	public uint threeDEffectOnArrive;

	// �ڱ�ըλ�ý���Ŀ��ѡ��(targetselection.txt).
	public uint targetSelectionOnArrive;
	
	// ������Ŀ�������Ч��.
	public uint skilleffectOnArrive;

	// ��ըʱ�����ٻ���.
	public uint creationOnArrive;

	// ׷�ٵ���׷���ӳ�.
    public float bulletFlyParam_0;

	// ׷�ٳ���ʱ��.
	public float bulletFlyParam_1;
}
