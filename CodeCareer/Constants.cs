namespace CodeCareer
{
    public class Constants
    {
        private static IConfiguration _configuration;

        // Инициализация при старте приложения
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static readonly int PlUS_RATING_FOR_POST = 5; // начисление рейтинга за публикацию
        public static readonly int PlUS_RATING_FOR_SUBSCRIBE = 10; // начисление рейтинга за подписчика
        public static readonly int PlUS_RATING_FOR_SUBSCRIPTION = 2; // начисление рейтинга за подписку
        public static readonly string DEFAULT_TAG_IMG_PATH = ""; // путь к изображению тега по-умолчанию

        public static string CONNECTION_STRING => _configuration.GetConnectionString("DefaultConnection"); // строка подключения к базе данных MySql

    }
}
