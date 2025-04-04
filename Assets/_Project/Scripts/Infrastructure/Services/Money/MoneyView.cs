using _Project.Scripts.Infrastructure.Services.Resources;
using Reflex.Attributes;

namespace _Project.Scripts.UI.Views
{
    public class MoneyView : BaseView
    {
        [Inject] private MoneyResourceService _moneyResourceService;

        private void OnEnable()
        {
            _moneyResourceService.ObservableValue.ChangedWithOld += Change;
            Change(_moneyResourceService.ObservableValue.Value, _moneyResourceService.ObservableValue.Value);
        }

        private void Change(int old, int @new) =>
            AnimationService.ResourceChanged(transform, old, @new, IncrementDuration, x => Text.SetText(x.ToString()));

        private void OnDisable() => _moneyResourceService.ObservableValue.ChangedWithOld -= Change;
    }
}