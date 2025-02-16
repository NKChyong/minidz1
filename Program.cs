using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ZooERP
{
    // Интерфейсы
    public interface IAlive
    {
        int Food { get; set; }
    }

    public interface IInventory
    {
        int Number { get; set; }
        string Name { get; set; }
    }

    // Базовый класс для животных
    public abstract class Animal : IAlive, IInventory
    {
        public string Name { get; set; }
        public int Food { get; set; }
        public int Number { get; set; }

        protected Animal(string name, int food, int number)
        {
            Name = name;
            Food = food;
            Number = number;
        }

        public override string ToString()
        {
            return $"{Name} (Инв. № {Number})";
        }
    }

    // Абстрактный класс для травоядных животных (Herbo)
    public abstract class Herbo : Animal
    {
        // Дополнительное свойство для травоядных – уровень доброты
        public int Kindness { get; set; }

        protected Herbo(string name, int food, int number, int kindness)
            : base(name, food, number)
        {
            Kindness = kindness;
        }
    }

    // Абстрактный класс для хищников (Predator)
    public abstract class Predator : Animal
    {
        protected Predator(string name, int food, int number)
            : base(name, food, number)
        {
        }
    }

    // Конкретные классы животных
    public class Monkey : Herbo
    {
        public Monkey(string name, int food, int number, int kindness)
            : base(name, food, number, kindness)
        {
        }
    }

    public class Rabbit : Herbo
    {
        public Rabbit(string name, int food, int number, int kindness)
            : base(name, food, number, kindness)
        {
        }
    }

    public class Tiger : Predator
    {
        public Tiger(string name, int food, int number)
            : base(name, food, number)
        {
        }
    }

    public class Wolf : Predator
    {
        public Wolf(string name, int food, int number)
            : base(name, food, number)
        {
        }
    }

    // Базовый класс для инвентаризационных вещей
    public abstract class Thing : IInventory
    {
        public string Name { get; set; }
        public int Number { get; set; }

        protected Thing(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public override string ToString()
        {
            return $"{Name} (Инв. № {Number})";
        }
    }

    public class Table : Thing
    {
        public Table(string name, int number)
            : base(name, number)
        {
        }
    }

    public class Computer : Thing
    {
        public Computer(string name, int number)
            : base(name, number)
        {
        }
    }

    // Интерфейс ветеринарной клиники
    public interface IVeterinaryClinic
    {
        bool CheckAnimal(Animal animal);
    }

    // Реализация ветеринарной клиники
    public class VeterinaryClinic : IVeterinaryClinic
    {
        public bool CheckAnimal(Animal animal)
        {
            Console.WriteLine($"\nПроводится медосмотр животного \"{animal.Name}\".");
            Console.Write("Введите '1', если животное здорово, или '0', если нет: ");
            string input = Console.ReadLine();
            return input == "1";
        }
    }

    // Класс Зоопарка
    public class Zoo
    {
        private readonly IVeterinaryClinic _vetClinic;
        public List<Animal> Animals { get; set; } = new List<Animal>();
        public List<Thing> Things { get; set; } = new List<Thing>();

        public Zoo(IVeterinaryClinic vetClinic)
        {
            _vetClinic = vetClinic;
        }

        // Добавление животного с проверкой состояния здоровья
        public void AddAnimal(Animal animal)
        {
            if (_vetClinic.CheckAnimal(animal))
            {
                Animals.Add(animal);
                Console.WriteLine($"Животное \"{animal.Name}\" успешно принято в зоопарк.\n");
            }
            else
            {
                Console.WriteLine($"Животное \"{animal.Name}\" не прошло медосмотр и не было принято.\n");
            }
        }

        // Подсчёт общего количества кг еды, необходимого животным в сутки
        public int GetTotalFoodConsumption()
        {
            int total = 0;
            foreach (var animal in Animals)
            {
                total += animal.Food;
            }
            return total;
        }

        // Получение списка животных для контактного зоопарка (травоядные с добротой > 5)
        public List<Herbo> GetContactZooAnimals()
        {
            var result = new List<Herbo>();
            foreach (var animal in Animals)
            {
                if (animal is Herbo herbo && herbo.Kindness > 5)
                {
                    result.Add(herbo);
                }
            }
            return result;
        }

        // Вывод списка всех инвентаризационных объектов (животных и вещей)
        public void ListInventoryItems()
        {
            Console.WriteLine("\nИнвентаризационные объекты зоопарка:");
            foreach (var animal in Animals)
            {
                Console.WriteLine(animal.ToString());
            }
            foreach (var thing in Things)
            {
                Console.WriteLine(thing.ToString());
            }
            Console.WriteLine();
        }
    }

    // Класс Program с точкой входа и реализацией консольного меню
    class Program
    {
        static void Main(string[] args)
        {
            // Настройка DI-контейнера
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Получение экземпляра зоопарка через DI
            var zoo = serviceProvider.GetService<Zoo>();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Добавить животное");
                Console.WriteLine("2. Вывести отчёт по животным (кол-во и кг еды)");
                Console.WriteLine("3. Показать животных для контактного зоопарка");
                Console.WriteLine("4. Вывести список инвентаризационных объектов");
                Console.WriteLine("5. Добавить инвентаризационный объект");
                Console.WriteLine("6. Выход");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddAnimal(zoo);
                        break;
                    case "2":
                        ShowReport(zoo);
                        break;
                    case "3":
                        ShowContactZooAnimals(zoo);
                        break;
                    case "4":
                        zoo.ListInventoryItems();
                        break;
                    case "5":
                        AddInventoryItem(zoo);
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.\n");
                        break;
                }
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Регистрация зависимостей
            services.AddSingleton<IVeterinaryClinic, VeterinaryClinic>();
            services.AddSingleton<Zoo>();
        }

        private static void AddAnimal(Zoo zoo)
        {
            Console.WriteLine("\nВыберите тип животного для добавления:");
            Console.WriteLine("1. Обезьяна");
            Console.WriteLine("2. Кролик");
            Console.WriteLine("3. Тигр");
            Console.WriteLine("4. Волк");
            Console.Write("Ваш выбор: ");
            string typeChoice = Console.ReadLine();

            Console.Write("Введите имя животного: ");
            string name = Console.ReadLine();
            Console.Write("Введите количество кг еды в сутки: ");
            int food = int.Parse(Console.ReadLine());
            Console.Write("Введите инвентаризационный номер: ");
            int number = int.Parse(Console.ReadLine());

            Animal animal = null;
            switch (typeChoice)
            {
                case "1":
                case "2":
                    // Для травоядных запрашиваем уровень доброты
                    Console.Write("Введите уровень доброты (от 0 до 10): ");
                    int kindness = int.Parse(Console.ReadLine());
                    if (typeChoice == "1")
                        animal = new Monkey(name, food, number, kindness);
                    else
                        animal = new Rabbit(name, food, number, kindness);
                    break;
                case "3":
                    animal = new Tiger(name, food, number);
                    break;
                case "4":
                    animal = new Wolf(name, food, number);
                    break;
                default:
                    Console.WriteLine("Неверный тип животного.\n");
                    return;
            }
            zoo.AddAnimal(animal);
        }

        private static void ShowReport(Zoo zoo)
        {
            Console.WriteLine($"\nОбщее количество животных: {zoo.Animals.Count}");
            Console.WriteLine($"Общее количество кг еды в сутки: {zoo.GetTotalFoodConsumption()}\n");
        }

        private static void ShowContactZooAnimals(Zoo zoo)
        {
            var contactAnimals = zoo.GetContactZooAnimals();
            if (contactAnimals.Count == 0)
            {
                Console.WriteLine("\nНет животных, подходящих для контактного зоопарка.\n");
            }
            else
            {
                Console.WriteLine("\nЖивотные, подходящие для контактного зоопарка:");
                foreach (var animal in contactAnimals)
                {
                    Console.WriteLine($"{animal.ToString()} | Уровень доброты: {animal.Kindness}");
                }
                Console.WriteLine();
            }
        }

        private static void AddInventoryItem(Zoo zoo)
        {
            Console.WriteLine("\nВыберите тип инвентаризационного объекта:");
            Console.WriteLine("1. Стол");
            Console.WriteLine("2. Компьютер");
            Console.Write("Ваш выбор: ");
            string choice = Console.ReadLine();

            Console.Write("Введите название объекта: ");
            string name = Console.ReadLine();
            Console.Write("Введите инвентаризационный номер: ");
            int number = int.Parse(Console.ReadLine());

            Thing thing = null;
            switch (choice)
            {
                case "1":
                    thing = new Table(name, number);
                    break;
                case "2":
                    thing = new Computer(name, number);
                    break;
                default:
                    Console.WriteLine("Неверный выбор.\n");
                    return;
            }
            zoo.Things.Add(thing);
            Console.WriteLine($"Объект \"{thing.Name}\" успешно добавлен.\n");
        }
    }
}
