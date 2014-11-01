using UnityEngine;
using System.Collections;

public class StoryTableItem
{
	// StoryID
	public int id;

	// 备注
	public string desc;

    // 下一个StoryID
    public int nextId;

	// 是否关闭另一对话
	public int closeOther;

    // 头像Atlas
	public string headAtlas;

	// 头像位置(0左上 1右上 2左下 3右下)
    public int headPos;

	// 头像左右旋转
	public int headMirror;

	// 时长(单位毫秒，-1表示不自动跳过)
	public int keepTime;

	// 允许跳过步骤
	public bool canSkipStep;

	// 允许全部跳过
	public bool canSkipAll;

	// 标题
	public string title;

    // 内容
	public string content;

	// 跳过功能Trigger(若在某步剧情被跳过，之后的所有步骤中该列Trigger会被执行，正常不会被执行)
	public string skipTrigger;

	// 正常功能Trigger(该Trigger只会在正常流程展现，若剧情被跳过，之后的所有步骤中该列Trigger[不会]被执行)
	public string normalTrigger;
}
