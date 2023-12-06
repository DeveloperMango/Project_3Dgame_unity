using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    public abstract class BaseState
    {
        //���¿� ���������� ����
        public abstract void OnEnterState();

        // ���� ���¿��� ��� ���ŵǾ���ϴ� ���� ������Ʈ
        public abstract void OnUpdateState();

        //���� ���¿��� ������ �����Ͽ� ������Ʈ
        public abstract void OnFixedUpdateState();

        //���� ���¸� ������ ��
        public abstract void OnExitState();
    }
}