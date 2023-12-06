using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDieState : CharacterController.BaseState
{
    public readonly int dieAnimation = Animator.StringToHash("doDie");
    private Monster monster;
    private const float DISABLE_TIME = 5f;
    private float timer;

    public MonsterDieState(Monster monster)
    {
        this.monster = monster;
    }

    public override void OnEnterState()
    {
        for (int i = 0; i < monster.skinnedMeshRenderers.Length; i++)
        {
            monster.skinnedMeshRenderers[i].material.color = Color.black;
        }

        monster.animator.SetTrigger(dieAnimation);
        timer = 0f;
        //UIManager.EventHandler.PostNotification(UI_EventHandler.UIEventType.DieEnemy, null);
    }

    public override void OnExitState()
    {
        for (int i = 0; i < monster.skinnedMeshRenderers.Length; i++)
        {
            monster.skinnedMeshRenderers[i].material.color = monster.originColors[i];
        }
    }

    public override void OnFixedUpdateState()
    {
    }

    public override void OnUpdateState()
    {
        timer += Time.deltaTime;

        if (timer >= DISABLE_TIME)
            monster.gameObject.SetActive(false);
    }


}
