using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pseudo;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingElementUI : Selectable
{
	[EntityRequires(typeof(BuildingPreview))]
	public PEntity Preview;
	public Text Text;

	PEntity currentPreview;

	bool isSelected;

	void Update()
	{
		if (!Application.isPlaying)
			return;

		UpdateText();
		UpdateSelection();
	}

	void UpdateText()
	{
		var scale = Preview.GetComponent<BuildingPreview>().Building.CachedTransform.localScale * LevelManager.Instance.CurrentLevel.Modifier;
		Text.text = string.Format("{0}x{1}", scale.x, scale.y);
	}

	void UpdateSelection()
	{
		bool currentSelected = isSelected;
		currentSelected |= currentSelectionState == SelectionState.Pressed;
		currentSelected &= currentSelectionState != SelectionState.Disabled && currentSelectionState != SelectionState.Normal;
		SetSelected(currentSelected);
	}

	void CreatePreview()
	{
		currentPreview = Instantiate(Preview);
		currentPreview.Transform.parent = BuildingManager.Instance.CachedTransform;
	}

	void SetSelected(bool selected)
	{
		if (selected != isSelected)
		{
			isSelected = selected;
			OnSelectedChanged(selected);
		}
	}

	void OnSelectedChanged(bool selected)
	{
		if (selected)
			CreatePreview();
		else if (currentPreview != null)
			currentPreview.SendMessage(EntityMessages.OnDeselected);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);

		SetSelected(false);
	}
}
