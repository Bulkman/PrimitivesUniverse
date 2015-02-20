using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class CNButton : CNAbstractController
{
	private SpriteRenderer spriteRenderer;

	public Sprite NormalState;
	public Sprite PressedState;

	public bool Pressed;
	
	public override void OnEnable()
	{
		base.OnEnable();
		
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	protected override void ResetControlState()
	{
		base.ResetControlState();
		Pressed = false;
		spriteRenderer.sprite = NormalState;
		CurrentAxisValues = Vector2.zero;
	}
	
	protected override void OnFingerLifted()
	{
		base.OnFingerLifted();
		Pressed = false;
		spriteRenderer.sprite = NormalState;
	}
	
	protected override void OnFingerTouched()
	{
		base.OnFingerTouched();
		Pressed = true;
		spriteRenderer.sprite = PressedState;
	}

	protected virtual void Update()
	{
		// Check for touches
		if (TweakIfNeeded())
			return;

		Touch currentTouch;
		if (IsTouchCaptured(out currentTouch));
	}

	protected override void TweakControl(Vector2 touchPosition)
	{
		CurrentAxisValues = Pressed ? Vector2.one : Vector2.zero;
	}
}

