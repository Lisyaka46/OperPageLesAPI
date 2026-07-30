using System.Net.NetworkInformation;

namespace OPLAPI.CORE.Internet
{
    /// <summary>
    /// Класс управления мониторингом подключения к интернету
    /// </summary>
    public static class Connection
    {
        /// <summary>
        /// Максимальный допустимый пинг при проверке интернета
        /// </summary>
        /// <remarks>
        /// Данное свойство можно изменять даже при запущенном процессе проверки подключения
        /// </remarks>
        public static ushort MaxPing { get; set; } = 3000;

        /// <summary>
        /// Хост использующийся для проверки интернета
        /// </summary>
        /// <remarks>
        /// Данное свойство можно изменять даже при запущенном процессе проверки подключения
        /// </remarks>
        public static string Host { get; set; } = "ya.ru";

        /// <summary>
        /// Текущее подключение к интернету
        /// </summary>
        public static bool StateConnect { get; private set; } = false;

        /// <summary>
        /// Событие изменения подключения к интернету
        /// </summary>
        public static event EventHandler<bool>? ConnectionChanged;

        /// <summary>
        /// Событие изменения потраченных милликенунд на подключение к интернету
        /// </summary>
        public static event EventHandler<ushort>? PingChanged;

        /// <summary>
        /// Запустить процесс проверки подключения к интернету
        /// </summary>
        /// <remarks>
        /// Процесс является цикличным, желательно указывать <see cref="CancellationToken"/>
        /// <code>
        /// while (!Token.IsCancellationRequested) { ... }
        /// </code>
        /// </remarks>
        /// <param name="Token">Токен для отмены операции проверки</param>
        public static async Task StartRunTimeCheckInternetConnection(CancellationToken Token = default)
        {
            bool? OldValue = default;
            using Ping PingData = new();
            ConnectStatus SourceStatus;
            while (!Token.IsCancellationRequested)
            {
                try
                {
                    SourceStatus = ConnectStatus.GetInternetConnectionStatus(PingData, Host, MaxPing);
                    if (OldValue == null || OldValue != SourceStatus.Connection)
                    {
                        OldValue = SourceStatus.Connection;
                        StateConnect = SourceStatus.Connection;
                        ConnectionChanged?.Invoke(null, SourceStatus.Connection);
                    }
                    PingChanged?.Invoke(null, SourceStatus.CurrentPing);
                    await Task.Delay(4000, Token);
                }
                catch (OperationCanceledException) { break; }
                catch { throw; }
            }
        }
    }
}
