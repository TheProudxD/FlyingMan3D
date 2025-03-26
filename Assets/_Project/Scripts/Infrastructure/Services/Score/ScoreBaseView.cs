using _Project.Scripts.Infrastructure.Services.Factories;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Views
{
    public abstract class ScoreBaseView : BaseView
    {
        [Inject] protected GameFactory GameFactory;

        protected abstract void DisplayDefaultScore();

        public abstract void Initialize();
    }
}