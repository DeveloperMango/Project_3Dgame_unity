namespace CharacterController
{
    public interface IEffect
    {
        public void PlayComboAttackEffects();
        public void DestroyEffect();
        /*        public void PlayDashAttackEffect();
                public void PlayCharingEffect();
                public void DestroyEffect();
                public void PlayChargingAt  tackEffect();
                public void PlaySkillEffect();*/
    }

    public interface IHittable
    {
        void Damaged(float damage);
    }

    public interface ISound
    {
        public void PlayComboAttackSound();
        public void PlayDashAttackSound();
        public void PlayChargingSound();
        public void PlayChargingAttackSound();
        public void PlaySkillSound();
    }
}