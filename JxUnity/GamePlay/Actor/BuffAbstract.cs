

public abstract class BuffAbstract
{
    public ActorAbstract Character { get; private set; }
    /// <summary>
    /// Buff����ʱ��
    /// </summary>
    public int Duration { get; private set; }

    public int EndTime { get; private set; }

    public void Initialize(ActorAbstract character, int duration, int endTime)
    {
        this.Character = character;
        this.Duration = duration;
        this.EndTime = endTime;
    }
    /// <summary>
    /// ��Ϣ��Buff��ʼʱ
    /// </summary>
    public virtual void OnEnter() { }
    /// <summary>
    /// ��Ϣ��Buff����ʱ
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// ��Ϣ�����£�66֡Ϊһ��Ĺ̶�֡��
    /// </summary>
    /// <param name="frame"></param>
    public virtual void FixedUpdate(int frame) { }
}
