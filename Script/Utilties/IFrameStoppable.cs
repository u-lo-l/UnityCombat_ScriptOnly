public interface IFrameStoppable
{
	void Freeze(float speed = 0);
	void Release();
	void FastMode();
	void RelaseFastMode();
}