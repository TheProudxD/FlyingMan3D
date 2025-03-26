namespace _Project.Scripts.Tools.Extensions
{
    public static class BoolExtensions
    {
        public static float ToFloat(this bool boolean) => boolean ? 1f : 0f;

        public static int ToInt(this bool boolean) => boolean ? 1 : 0;
    }
}