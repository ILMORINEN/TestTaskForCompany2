using PTMK;

if (args.Length == 0)
{
    Console.WriteLine("Отсутствуют параметры запуска.");
    return;
}

switch (args[0])
{   // 1. Создание таблицы с полями представляющими ФИО, дату рождения, пол
    case "1":
        TaskFunctions.CreateTableHuman();
        break;

    // 2. Создание записи.
    case "2":
        TaskFunctions.AddRow(args[1], DateTime.Parse(args[2]), int.Parse(args[3]) == 1);
        break;

    // 3. Вывод всех строк с уникальным значением ФИО+дата, отсортированным по ФИО,
    // вывести ФИО, Дату рождения, пол, кол-во полных лет.
    case "3":
        TaskFunctions.SelectUniqueNameBirthPair();
        break;

    // 4. Заполнение автоматически 1000000 строк. Распределение пола в них должно
    // быть относительно равномерным, начальной буквы ФИО также. Заполнение автоматически
    // 100 строк в которых пол мужской и ФИО начинается с "F".
    case "4":
        TaskFunctions.FillTable();
        break;

    // 5. Результат выборки из таблицы по критерию: пол мужской, ФИО  начинается с "F".
    // Сделать замер времени выполнения.
    case "5":
        TaskFunctions.GetFilteredHumans();
        break;

    default:
        break;
}
