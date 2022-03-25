using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ͻ��˺ͷ���˴��ڵ��߼�ʵ��
public class SyncEntity
{
    private int netId;
    private bool isDirty;

#if !SERVER
    private GameObject gameObject;
#endif
    protected virtual void OnTick() { }
    protected virtual void ModifyData() { }
    protected void Sync() { }
    
}

//�ͻ��˺ͷ���˴��ڵ��߼�ʵ��
public class BlockEntity : SyncEntity
{

    protected override void OnTick()
    {
        base.OnTick();

    }
}