using Castaway.Core;

namespace Castaway.Levels
{
    public class LevelModule : Module
    {
        protected override void PreUpdate()
        {
            base.PreUpdate();
            Lighting.Reset();
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();
            Lighting.Finish();
        }
    }
}