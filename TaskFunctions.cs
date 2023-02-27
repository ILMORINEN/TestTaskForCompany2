using System.Configuration;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PTMK
{
    public static class TaskFunctions
    {
        // Создание таблицы с именем Human с полями представляющими ФИО, Дату рождения, Пол.
        public static void CreateTableHuman()
        {
            using (var context = new TestTaskContext())
            {
                try
                {
                    context.Database.ExecuteSqlCommand(@"
            CREATE TABLE[dbo].[Human](
                [Id] INT IDENTITY(1, 1) NOT NULL,
                [FullName] NVARCHAR(MAX) NOT NULL,
                [BirthDate] DATE NOT NULL,
                [Sex] BIT NOT NULL
            );");
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    return;
                }
            }
            Console.WriteLine("Таблица создана.");
        }

        // Создание записи
        public static void AddRow(string fullName, DateTime birthday, bool sex)
        {
            using (var context = new TestTaskContext())
            {
                context.Humans.Add(new Human() { FullName = fullName, BirthDate = birthday, Sex = sex });
                context.SaveChanges();
            }
            Console.WriteLine("Строка добавлена.");
        }

        // Вывод всех строк с уникальным значением ФИО + дата, отсортированным по ФИО, вывести ФИО, Дату рождения, пол, кол-во полных лет.
        public static void SelectUniqueNameBirthPair()
        {
            using (var context = new TestTaskContext())
            {
                var result = context.Humans
            .GroupBy(h => new { h.FullName, h.BirthDate })
            .Select(g => new
            {
                FullName = g.Key.FullName,
                BirthDate = g.Key.BirthDate,
                Sex = g.FirstOrDefault().Sex,
                Age = SqlFunctions.DateDiff("year", g.Key.BirthDate, DateTime.Now)
            })
            .OrderBy(h => h.FullName)
            .ToList();

                Console.WriteLine($"Обработано {result.Count()} строк.\nПервые 100 строк запроса:");

                for (int i = 0; i < ((result.Count > 100) ? 100 : result.Count); i++)
                {
                    Console.WriteLine($"{result[i].FullName}\t{result[i].BirthDate}\t{result[i].Sex}\t{result[i].Age}");
                }
            }
        }

        // Заполнение автоматически 1000000 строк. Распределение пола в них должно быть относительно равномерным, начальной буквы ФИО также.
        // Заполнение автоматически  100 строк в которых пол мужской и ФИО начинается с "F".
        public static void FillTable()
        {
            var rnd = new Random();

            var firstNames = new string[] { "Adam", "Bob", "Charlie", "David", "Ethan", "Frank", "Gary", "Henry", "Isaac", "Jack", "Kevin", "Leo", "Mark", "Nathan", "Oliver", "Peter", "Quentin", "Robert", "Steve", "Tom", "Ulrich", "Victor", "William", "Xavier", "Yan", "Zach" };
            var lastNames = new string[] { "Adams", "Black", "Carter", "Davis", "Edwards", "Ford", "Gates", "Harris", "Ingram", "Johnson", "King", "Lee", "Morgan", "Nash", "Owens", "Peterson", "Quinn", "Richards", "Smith", "Taylor", "Underwood", "Valentine", "Williams", "Xu", "Yilmaz", "Zhang" };
            var humans = new List<Human>();

            for (var i = 0; i < 1000000; i++)
            {
                var human = new Human();
                var firstName = firstNames[rnd.Next(firstNames.Length)];
                var lastName = lastNames[rnd.Next(lastNames.Length)];

                human.FullName = $"{firstName} {lastName}";
                human.BirthDate = DateTime.Now.AddYears(-rnd.Next(18, 80)).AddDays(-rnd.Next(365));
                human.Sex = rnd.Next(2) == 0;
                humans.Add(human);
            }
            for (var i = 0; i < 100; i++)
            {
                var human = new Human();
                var firstName = firstNames[rnd.Next(firstNames.Length)];
                var lastName = lastNames[rnd.Next(lastNames.Length)];

                human.FullName = $"F{firstName.Substring(1)} {lastName}";
                human.BirthDate = DateTime.Now.AddYears(-rnd.Next(18, 80)).AddDays(-rnd.Next(365));
                human.Sex = true;
                humans.Add(human);
            }

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLExpress"].ConnectionString);
            connection.Open();
            SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);
            bulkCopy.DestinationTableName = "Human";

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("FullName", typeof(string));
            dataTable.Columns.Add("BirthDate", typeof(DateTime));
            dataTable.Columns.Add("Sex", typeof(bool));

            foreach (var human in humans) // humans - это коллекция объектов Human
            {
                dataTable.Rows.Add(null, human.FullName, human.BirthDate, human.Sex);
            }

            bulkCopy.WriteToServer(dataTable);

            connection.Close();
        }

        // Результат выборки из таблицы по критерию: пол мужской, ФИО  начинается с "F". Сделать замер времени выполнения.
        public static void GetFilteredHumans()
        {
            Stopwatch sw = new();
            using (var context = new TestTaskContext())
            {
                var sql = @"
            SELECT [Id], [FullName], [Birthdate], [Sex]
            FROM [dbo].[Human]
            WHERE [Sex] = 1 AND [FullName] LIKE 'F%'";

                
                var result = context.Database.SqlQuery<Human>(sql);
                
                sw.Start();
                var output = result.ToList();
                sw.Stop();

                Console.WriteLine($"Запрос завершился за {sw.ElapsedMilliseconds} мс.\nОбработано { result.Count()} строк\nПервые 100 строк запроса:");

                for (int i = 0; i < ((output.Count > 100) ? 100 : output.Count); i++)
                    Console.WriteLine($"{output[i].Id}\t{output[i].FullName}\t{output[i].BirthDate}\t{output[i].Sex}");
            }
        }
    }
}

