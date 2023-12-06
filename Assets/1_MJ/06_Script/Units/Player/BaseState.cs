using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterController
{
    public abstract class BaseState
    {
        //상태에 진입했을때 실행
        public abstract void OnEnterState();

        // 현재 상태에서 계속 갱신되어야하는 정보 업데이트
        public abstract void OnUpdateState();

        //현재 상태에서 물리와 관련하여 업데이트
        public abstract void OnFixedUpdateState();

        //현재 상태를 종료할 때
        public abstract void OnExitState();
    }
}