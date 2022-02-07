using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
	public BackgroundMaterialChanger BackgroundMaterialChanger;
	public PauseMenu PauseMenu;
	public SceneTransitionController SceneTransitionController;
	public RewardPanelAnimationEvents RewardPanelAnimationEvents;
	public InfoHolder_IGM InfoHolder;
	public BossHealthBar BossHealthBar;
	public UIAddTransparentToSection UITransparentSection;
	public UIPlayerHUDController UiPlayerHudController;
	
	[SerializeField] EventSystem eventSystem;

	public EventSystem EventSystem => eventSystem;
	
	protected override void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(gameObject);
	}

	void Update()
	{
		Cursor.visible = JP_MenuManager.instance.useController == false;
	}
	
	public void ActivateRewardPanel()
	{
		InfoHolder.rewardPanel.SetActive(!InfoHolder.rewardPanel.activeSelf);
		//GameManager.instance.loot.RewardsNumber();
		InfoHolder.rewardPanel.GetComponentInChildren<RewardPanelAnimationEvents>().StartRewards();
	}
}
