
using FantasyEngine;
using UnityEngine;
public sealed class ParticleUtility
{

    public static uint AddEffect2MV(MeshVisual visual, int effectid, string mount,SceneParticleManager mng)
    {
        if (!DataManager.EffectTable.ContainsKey(effectid))
            return uint.MaxValue;
        if (!string.IsNullOrEmpty(mount))
        {
            mount = mount.Replace("%", "");
        }
        EffectTableItem item = DataManager.EffectTable[effectid] as EffectTableItem;
        if (item == null)
            return uint.MaxValue;

        Transform trans = visual.GetBoneByName(mount);
        if (trans == null)
            trans = visual.VisualTransform;
        TransformData data = new TransformData();
        data.notFollow = item.notFollow;
        data.Scale = new Vector3(item.scale, item.scale, item.scale);

        //不跟随释放者的特效，取挂点的方向
        if (item.notFollow && trans != null)
        {
            if (trans != null)
                data.Rot = trans.rotation.eulerAngles;
            else
                data.Rot = Vector3.zero;
        }

		if (!string.IsNullOrEmpty(item.soundId))
		{
			string[] array = item.soundId.Split('|');
			SoundManager.Instance.Play(
				int.Parse(array[UnityEngine.Random.Range(0, array.Length)]),
				item.soundDelay
				);
		}

        return mng.AddParticle(item.effect_name, item.loop, trans, data, item.limitry);
    }
}

 