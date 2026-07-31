using CodeCareer.Areas.User.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Data
{
    public static class LearningDatabaseInitializer
    {
        public static void Initialize()
        {
            using var db = new ApplicationDbContext();
            db.Database.EnsureCreated();
            EnsureLearningTables(db);
            TryAlterTasksTopicId(db);
            SeedCurriculum(db);
            LinkTasksToTopics(db);
            SeedDemoCourse(db);
        }

        private static void EnsureLearningTables(ApplicationDbContext db)
        {
            var statements = new[]
            {
                @"CREATE TABLE IF NOT EXISTS sections (
                    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    title VARCHAR(512) NOT NULL,
                    sort_order INT NOT NULL
                )",
                @"CREATE TABLE IF NOT EXISTS topics (
                    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    section_id INT NOT NULL,
                    title VARCHAR(512) NOT NULL,
                    slug VARCHAR(256) NOT NULL,
                    sort_order INT NOT NULL,
                    is_published TINYINT(1) NOT NULL,
                    UNIQUE INDEX IX_topics_slug (slug),
                    CONSTRAINT FK_topics_sections FOREIGN KEY (section_id) REFERENCES sections(Id) ON DELETE CASCADE
                )",
                @"CREATE TABLE IF NOT EXISTS notes (
                    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    topic_id INT NOT NULL,
                    title VARCHAR(512) NOT NULL,
                    body_markdown LONGTEXT NOT NULL,
                    sort_order INT NOT NULL,
                    updated_at DATETIME(6) NOT NULL,
                    CONSTRAINT FK_notes_topics FOREIGN KEY (topic_id) REFERENCES topics(Id) ON DELETE CASCADE
                )",
                @"CREATE TABLE IF NOT EXISTS courses (
                    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    title VARCHAR(512) NOT NULL,
                    description LONGTEXT NOT NULL,
                    sort_order INT NOT NULL,
                    is_published TINYINT(1) NOT NULL
                )",
                @"CREATE TABLE IF NOT EXISTS course_topics (
                    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    course_id INT NOT NULL,
                    topic_id INT NOT NULL,
                    sort_order INT NOT NULL,
                    CONSTRAINT FK_course_topics_courses FOREIGN KEY (course_id) REFERENCES courses(Id) ON DELETE CASCADE,
                    CONSTRAINT FK_course_topics_topics FOREIGN KEY (topic_id) REFERENCES topics(Id) ON DELETE CASCADE
                )",
                @"CREATE TABLE IF NOT EXISTS user_topic_progress (
                    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    user_email VARCHAR(256) NOT NULL,
                    topic_id INT NOT NULL,
                    note_id INT NULL,
                    note_read_at DATETIME(6) NULL,
                    INDEX IX_user_topic_progress_lookup (user_email, topic_id, note_id)
                )",
                @"CREATE TABLE IF NOT EXISTS user_task_progress (
                    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    user_email VARCHAR(256) NOT NULL,
                    task_id INT NOT NULL,
                    status VARCHAR(64) NOT NULL,
                    solved_at DATETIME(6) NOT NULL,
                    UNIQUE INDEX IX_user_task_progress (user_email, task_id)
                )",
                @"CREATE TABLE IF NOT EXISTS chat_histories (
                    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                    user_email VARCHAR(256) NOT NULL,
                    note_id INT NOT NULL,
                    role VARCHAR(32) NOT NULL,
                    content LONGTEXT NOT NULL,
                    created_at DATETIME(6) NOT NULL
                )"
            };

            foreach (var sql in statements)
            {
                try { db.Database.ExecuteSqlRaw(sql); }
                catch { /* already exists or FK order */ }
            }
        }

        private static void TryAlterTasksTopicId(ApplicationDbContext db)
        {
            try
            {
                db.Database.ExecuteSqlRaw(
                    "ALTER TABLE tasks ADD COLUMN topic_id INT NULL");
            }
            catch
            {
                // column may already exist
            }
        }

        private static void SeedCurriculum(ApplicationDbContext db)
        {
            if (db.Sections.Any())
            {
                return;
            }

            var sections = new List<(string Title, int Order, List<(string Title, string Slug, bool Published, string? NoteMd)> Topics)>
            {
                ("Алгоритмы поиска", 1, new()
                {
                    ("Линейный поиск", "linear-search", true, LinearSearchNote),
                    ("Бинарный поиск", "binary-search", true, BinarySearchNote),
                }),
                ("Алгоритмы сортировки", 2, new()
                {
                    ("Квадратичные сортировки", "quadratic-sorts", true, null),
                    ("Быстрые сортировки", "fast-sorts", true, null),
                    ("Линейные сортировки", "linear-sorts", true, null),
                }),
                ("Структуры данных", 3, new()
                {
                    ("Массивы и списки", "arrays-lists", true, null),
                    ("Стек и очередь", "stack-queue", true, null),
                    ("Хеш-таблицы", "hash-tables", true, null),
                    ("Связные списки", "linked-lists", true, null),
                    ("Деревья", "trees", true, null),
                }),
                ("Алгоритмы на графах", 4, new()
                {
                    ("Графы — введение", "graphs-intro", false, null),
                }),
                ("Динамическое программирование", 5, new()
                {
                    ("ДП — введение", "dp-intro", false, null),
                }),
                ("Front-end", 6, new()
                {
                    ("HTML основы", "html-basics", true, HtmlNote),
                    ("CSS основы", "css-basics", true, null),
                    ("JavaScript основы", "js-basics", true, null),
                }),
            };

            int sectionOrder = 0;
            foreach (var (title, order, topics) in sections)
            {
                var section = new SectionModel
                {
                    Title = title,
                    SortOrder = order > 0 ? order : ++sectionOrder,
                };
                db.Sections.Add(section);
                db.SaveChanges();

                int topicOrder = 0;
                foreach (var (topicTitle, slug, published, noteMd) in topics)
                {
                    var topic = new TopicModel
                    {
                        SectionId = section.Id,
                        Title = topicTitle,
                        Slug = slug,
                        SortOrder = ++topicOrder,
                        IsPublished = published,
                    };
                    db.Topics.Add(topic);
                    db.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(noteMd))
                    {
                        db.Notes.Add(new NoteModel
                        {
                            TopicId = topic.Id,
                            Title = $"Конспект: {topicTitle}",
                            BodyMarkdown = noteMd,
                            SortOrder = 1,
                            UpdatedAt = DateTime.UtcNow,
                        });
                        db.SaveChanges();
                    }
                }
            }
        }

        private static void LinkTasksToTopics(ApplicationDbContext db)
        {
            var topics = db.Topics.AsNoTracking().ToList();
            var tasks = db.Tasks.Where(t => t.TopicId == null).ToList();
            foreach (var task in tasks)
            {
                var topic = topics.FirstOrDefault(t =>
                    string.Equals(t.Title, task.Type, StringComparison.OrdinalIgnoreCase));
                if (topic != null)
                {
                    task.TopicId = topic.Id;
                    if (string.IsNullOrWhiteSpace(task.Type))
                    {
                        task.Type = topic.Title;
                    }
                }
            }
            db.SaveChanges();
        }

        private static void SeedDemoCourse(ApplicationDbContext db)
        {
            if (db.Courses.Any())
            {
                return;
            }

            var course = new CourseModel
            {
                Title = "Старт: поиск и основы",
                Description = "Первый трек: линейный и бинарный поиск, затем HTML.",
                SortOrder = 1,
                IsPublished = true,
            };
            db.Courses.Add(course);
            db.SaveChanges();

            var slugs = new[] { "linear-search", "binary-search", "html-basics" };
            int order = 0;
            foreach (var slug in slugs)
            {
                var topic = db.Topics.FirstOrDefault(t => t.Slug == slug);
                if (topic == null) continue;
                db.CourseTopics.Add(new CourseTopicModel
                {
                    CourseId = course.Id,
                    TopicId = topic.Id,
                    SortOrder = ++order,
                });
            }
            db.SaveChanges();
        }

        private const string LinearSearchNote = """
# Линейный поиск

Линейный (последовательный) поиск — алгоритм, который просматривает элементы коллекции **по одному**, пока не найдёт искомое значение или не закончатся элементы.

## Идея

1. Начинаем с первого элемента.
2. Сравниваем с целевым значением.
3. Если совпало — возвращаем индекс (или сам элемент).
4. Иначе переходим к следующему.
5. Если элементы закончились — элемент не найден.

## Сложность

- Время: **O(n)** в худшем и среднем случае.
- Память: **O(1)** дополнительной памяти.

## Когда применять

- Несортированные данные.
- Небольшие массивы.
- Когда простота важнее скорости.

## Пример (псевдокод)

```
function linearSearch(arr, target):
    for i from 0 to length(arr) - 1:
        if arr[i] == target:
            return i
    return -1
```

После конспекта решите задачи темы — это закрепит материал.
""";

        private const string BinarySearchNote = """
# Бинарный поиск

Бинарный поиск работает на **отсортированной** последовательности: каждый шаг отбрасывает половину диапазона.

## Идея

1. Смотрим на средний элемент.
2. Если он равен цели — готово.
3. Если цель меньше — ищем в левой половине.
4. Если больше — в правой.
5. Повторяем, пока диапазон не пуст.

## Сложность

- Время: **O(log n)**.
- Память: **O(1)** (итеративно).

## Важно

Массив (или другая структура с индексом) должен быть отсортирован, иначе результат неверен.
""";

        private const string HtmlNote = """
# HTML основы

HTML описывает **структуру** страницы: заголовки, параграфы, ссылки, формы.

## Минимальный документ

```html
<!DOCTYPE html>
<html lang="ru">
<head>
  <meta charset="UTF-8" />
  <title>Страница</title>
</head>
<body>
  <h1>Привет</h1>
  <p>Текст</p>
</body>
</html>
```

Дальше изучайте CSS и JavaScript в соседних темах Front-end трека.
""";
    }
}
