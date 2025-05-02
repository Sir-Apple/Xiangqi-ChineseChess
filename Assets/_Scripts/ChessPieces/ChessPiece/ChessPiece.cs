using UnityEngine;

public enum ChessPieceType
{
	None = 0,
	Red_Soldier = 1,
	Red_Cannon = 2,
	Red_Chariot = 3,
	Red_Horse = 4,
	Red_Elephant = 5,
	Red_Advisor = 6,
	Red_General = 7,
	Blue_Soldier = 8,
	Blue_Cannon = 9,
	Blue_Chariot = 10,
	Blue_Horse = 11,
	Blue_Elephant = 12,
	Blue_Advisor = 13,
	Blue_General = 14
}

public class ChessPiece : MonoBehaviour
{
	public int team;
	public int currentX;
	public int currentY;
	public ChessPieceType type;

	private Vector3 desiredPosition;
	private Vector3 desiredScale = Vector3.one;

	private void Update()
	{
		transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);

		transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
	}

	public virtual void SetPosition(Vector3 position, bool force = false)
	{
		desiredPosition = position;
		if(force)
			transform.position = desiredPosition;
	}

	public virtual void SetScale(Vector3 scale, bool force = false)
	{
		desiredScale = scale;
		if (force)
			transform.localScale = desiredScale;
	}
}
