namespace StcRoutrt
{
    public interface IRouter
    {
        /// <summary>
        /// регистрирует маршрут
        /// </summary>
        /// <typeparam name="T1">тип переменной</typeparam>
        /// <param name="template">шаблон</param>
        /// <param name="action">обработчик</param>
        void RegisterRoute<T1>(string template, Action<T1> action)
            where T1 : IParsable<T1>;

        /// <summary>
        /// регистрирует маршрут
        /// </summary>
        /// <typeparam name="T1">тип первой переменной</typeparam>
        /// <typeparam name="T2">тип второй переменной</typeparam>
        /// <param name="template">шаблон</param>
        /// <param name="action">обработчик</param>
        void RegisterRoute<T1, T2>(string template, Action<T1, T2> action)
            where T1 : IParsable<T1> where T2 : IParsable<T2>;

        /// <summary>
        /// вызывает обработчик соответствующий маршруту
        /// </summary>
        /// <param name="route">маршрут</param>
        void Route(string route);
    }
}
