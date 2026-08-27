using CodeCareer.Areas.User.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeCareer.Areas.User.Data;

/// <summary>Development seed data for learning curriculum. Not run in production automatically.</summary>
public static class LearningDatabaseInitializer
{
    public static void SeedCurriculumOnly(ApplicationDbContext db)
    {
        SeedCurriculum(db);
        LinkTasksToTopics(db);
        SeedDemoCourse(db);
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

После конспекта решите задачи темы — это закрепит материал.
""";

    private const string BinarySearchNote = """
# Бинарный поиск

Бинарный поиск работает на **отсортированной** последовательности: каждый шаг отбрасывает половину диапазона.

## Сложность

- Время: **O(log n)**.
- Память: **O(1)** (итеративно).
""";

    private const string HtmlNote = """
# HTML основы

HTML описывает **структуру** страницы: заголовки, параграфы, ссылки, формы.

```html
<!DOCTYPE html>
<html lang="ru">
<head><meta charset="UTF-8" /><title>Страница</title></head>
<body><h1>Привет</h1></body>
</html>
```
""";
}
