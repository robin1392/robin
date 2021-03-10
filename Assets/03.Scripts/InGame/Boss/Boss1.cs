using System.Collections;
using System.Collections.Generic;
using ED;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss1 : Minion
{
    public GameObject pref_Dust;
    
    private float _skillCastedTime;
    private bool _isSkillCasting;

    public override void Initialize(DestroyCallback destroy)
    {
        base.Initialize(destroy);
        _skillCastedTime = -effectCooltime;
        PoolManager.instance.AddPool(pref_Dust, 1);
    }

    private Minion GetLongDistanceTarget()
    {
        var minions = InGameManager.Get().GetBottomMinions();
        List<BaseStat> list = new List<BaseStat>();
        float min = 0f;
        float max = 0f;
            
        foreach (var minion in minions)
        {
            float sqrDistance = Vector3.SqrMagnitude(minion.transform.position - transform.position);
            
            if (list.Count == 0)
            {
                list.Add(minion);
                min = sqrDistance;
                max = sqrDistance;
                continue;
            }
            
            if (sqrDistance > max)
            {
                list.Add(minion);
                max = sqrDistance;
            }
            else if (sqrDistance < min)
            {
                list.Insert(0, minion);
                min = sqrDistance;
            }
        }

        if (list.Count > 0)
            return list[Random.Range(list.Count / 2, list.Count)] as Minion;
        else
            return null;
    }

    public void Skill()
    {
        if (_spawnedTime >= _skillCastedTime + effectCooltime)
        {
            var m = GetLongDistanceTarget();
            if (m != null)
            {
                _skillCastedTime = _spawnedTime;
                StartCoroutine(SkillCoroutine(m));
                
                controller.NetSendPlayer(GameProtocol.SEND_MESSAGE_PARAM1_RELAY, id, E_ActionSendMessage.JumpTarget, m.id);
            }
        }
    }

    public void JumpTarget(uint id)
    {
        var m = InGameManager.Get().GetBottomMinion(id);

        if (m != null)
        {
            StartCoroutine(SkillCoroutine(m));
        }
    }
    
    private IEnumerator SkillCoroutine(Minion m)
    {
        //var m = GetLongDistanceTarget();
        
        //스킬을 SendMessage로 처리할지.. 단독으로 처리하고 릴레이 할지..

        if (m == null)
        {
            _collider.enabled = true;
            
            yield break;
        }
        
        _isSkillCasting = true;
        SetControllEnable(false);

        animator.SetTrigger(_animatorHashSkill);
        
        yield return new WaitForSeconds(1f);

        var lookPos = m.transform.position;
        lookPos.y = 0;
        transform.LookAt(lookPos);
        
        yield return null;
        
        var ts = transform;
        var startPos = ts.position;
        var targetPos = m.transform.position;
        var t = 0f;

        float fV_x;
        float fV_y;
        float fV_z;

        float fg;
        float fEndTime;
        float fMaxHeight = 2f;
        float fHeight;
        float fEndHeight;
        float fTime = 0f;
        float fMaxTime = 0.5f;

        fEndHeight = targetPos.y - startPos.y;
        fHeight = fMaxHeight - startPos.y;
        fg = 2 * fHeight / (fMaxTime * fMaxTime);
        fV_y = Mathf.Sqrt(2 * fg * fHeight);

        float a = fg;
        float b = -2 * fV_y;
        float c = 2 * fEndHeight;

        fEndTime = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

        fV_x = -(startPos.x - targetPos.x) / fEndTime;
        fV_z = -(startPos.z - targetPos.z) / fEndTime;

        var currentPos = new Vector3();
        while (t < fEndTime)
        {
            t += Time.deltaTime;

            currentPos.x = startPos.x + fV_x * t;
            currentPos.y = startPos.y + (fV_y * t) - (0.5f * fg * t * t);
            currentPos.z = startPos.z + fV_z * t;

            ts.position = currentPos;
            
            yield return null;
        }

        SetControllEnable(true);
        _isSkillCasting = false;
        _collider.enabled = true;
        var pos = transform.position;
        pos.y = 0;
        transform.position = pos;
        
        if (isMine || controller.isPlayingAI) SplashDamage();
        
        var effect = PoolManager.instance.ActivateObject("Effect_Support", pos);
        effect.rotation = Quaternion.identity;
        effect.localScale = Vector3.one * 0.8f;
    }

    private void SplashDamage()
    {
        var targets = Physics.OverlapSphere(transform.position, 1f, targetLayer);

        for (int i = 0; i < targets.Length; i++)
        {
            var bs = targets[i].GetComponentInParent<BaseStat>();
            if (bs != null)
            {
                DamageToTarget(bs);
            }
        }
    }
}
