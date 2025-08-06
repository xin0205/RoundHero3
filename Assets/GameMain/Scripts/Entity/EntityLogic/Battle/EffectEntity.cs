namespace RoundHero
{
    public class EffectEntity : Entity
    {
        
        public bool AutoHide = false;
        public float HideTime = 3f;
        
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            if (AutoHide)
            {
                GameUtility.DelayExcute(HideTime, () =>
                {
                    GameEntry.Entity.HideEntity(this);
                });
            }

        }
    }
}