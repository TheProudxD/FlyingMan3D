using _Project.Scripts.Infrastructure.Services.Localization;
using _Project.Scripts.Infrastructure.Services.Resources;
using Reflex.Attributes;
using TS.LocalizationSystem;

namespace _Project.Scripts.UI.Views
{
    public class LevelView : BaseView
    {
        [Inject] private LocalizationService _localizationService;
        [Inject] private LevelResourceService _levelResourceService;

        private void OnEnable()
        {
            _levelResourceService.Current.ChangedWithOld += Change;
            _localizationService.LocaleChanged += Localize;
            Localize(_localizationService.Current, _localizationService.UpdateNumber);
        }

        private void Change(int old, int @new) =>
            AnimationService.ResourceChanged(transform, old, @new, IncrementDuration,
                x => Localize(x.ToString()));

        private void OnDisable() => _localizationService.LocaleChanged -= Localize;

        private void Localize(LocaleConfig config, int number) => Localize(_levelResourceService.Current.Value.ToString());

        private void Localize(string level)
        {
            string text = $"{_localizationService.Localize(LocalizationKeys.Level)} {level}";
            Text.SetText(text);
        }
    }
}