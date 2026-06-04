namespace StudentManagement
{
    interface IPerson
    {
        void DisplayInformation();
    }

    class Student : IPerson
    {
        protected int id;
        protected string name;
        protected int age;

        public Student(int id, string name, int age)
        {
            this.id = id;
            this.name = name;
            this.age = age;
        }

        public int GetId() { return id; }
        public string GetName() { return name; }
        public int GetAge() { return age; }

        public void SetId(int value) { id = value; }
        public void SetName(string value) { name = value; }
        public void SetAge(int value) { age = value; }

        public Tuple<int, string, int> GetStudentInfo()
        {
            return new Tuple<int, string, int>(id, name, age);
        }

        public virtual void DisplayInformation()
        {
            Console.WriteLine($"Student #{id}: {name}, Age {age}");
        }

        public virtual List<string> GetFields()
        {
            return ["name", "age"];
        }

        public virtual bool UpdateFields(Dictionary<string, string> fieldsValues)
        {
            try
            {
                foreach (var field in fieldsValues.Keys)
                {
                    string value = fieldsValues[field];
                    switch (field)
                    {
                        case "name":
                            SetName(value.ToString()!);
                            break;
                        case "age":
                            SetAge(int.Parse(value));
                            break;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    class CollegeStudent : Student
    {
        protected string subject;
        protected int gradeAverage;

        public CollegeStudent(int id, string name, int age, string subject, int gradeAverage) : base(id, name, age)
        {
            this.subject = subject;
            this.gradeAverage = gradeAverage;
        }

        public string GetSubject() { return subject; }
        public int GetGradeAverage() { return gradeAverage; }

        public void SetSubject(string value) { subject = value; }
        public void SetGradeAverage(int value) { gradeAverage = value; }

        public override void DisplayInformation()
        {
            Console.WriteLine($"College Student #{id}: Studying {subject} with grade average of {gradeAverage}");
        }

        public override List<string> GetFields()
        {
            List<string> fields = base.GetFields();
            fields.AddRange(["subject", "gradeAverage"]);
            return fields;
        }

        public override bool UpdateFields(Dictionary<string, string> fieldsValues)
        {
            bool result = true;
            try
            {
                foreach (var field in fieldsValues.Keys)
                {
                    string value = fieldsValues[field];
                    switch (field)
                    {
                        case "subject":
                            SetSubject(value.ToString()!);
                            break;
                        case "gradeAverage":
                            SetGradeAverage(int.Parse(value));
                            break;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return base.UpdateFields(fieldsValues) && result;
        }
    }

    class StudentManager
    {
        private List<Student> students;
        private int currentStudentId;

        public StudentManager()
        {
            students = [];
            currentStudentId = 1;
        }

        public void AddStudent(Student student)
        {
            student.SetId(currentStudentId);
            currentStudentId++;
            students.Add(student);
        }

        public int DisplayStudents()
        {
            foreach (var student in students)
            {
                student.DisplayInformation();
            }
            return students.Count;
        }

        public bool DisplayStudent(int id)
        {
            var student = students.FirstOrDefault(s => s.GetId() == id);
            if (student == null) return false;

            student.DisplayInformation();
            return true;
        }

        public bool DisplayStudent(string name)
        {
            var studentsToDisplay = students.FindAll(s => s.GetName() == name);

            if (studentsToDisplay.Count == 0)
                return false;

            foreach (var student in studentsToDisplay)
            {
                student.DisplayInformation();
            }

            return true;
        }

        public bool UpdateStudent(int id, Dictionary<string, string> fieldsToChange)
        {
            var student = students.FirstOrDefault(s => s.GetId() == id);
            if (student == null) return false;
            return student.UpdateFields(fieldsToChange);
        }

        public bool DeleteStudent(int id)
        {
            int studentIndex = students.FindIndex(s => s.GetId() == id);
            if (studentIndex == -1) return false;

            students.RemoveAt(studentIndex);
            return true;
        }

        public bool DoesStudentExist(int id)
        {
            return students.FindIndex(s => s.GetId() == id) != -1;
        }

        public Student? GetStudent(int id)
        {
            return students.FirstOrDefault(s => s.GetId() == id);
        }
    }

    class Program
    {
        static StudentManager manager = new();

        public static int ShowMenu()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Student Management System.\n");
                Console.WriteLine("What operation would you want to make?");
                Console.WriteLine("1) Create Student");
                Console.WriteLine("2) Display Students");
                Console.WriteLine("3) Update Student");
                Console.WriteLine("4) Delete Student");
                Console.WriteLine("5) Exit");

                try
                {
                    int op = Console.ReadKey().KeyChar - '0';
                    Console.WriteLine();
                    if (op >= 1 && op <= 5)
                        return op;
                    else
                    {
                        Console.WriteLine("Invalid operation code. Press Enter to try again.");
                        Console.ReadKey();
                    }
                }
                catch { }
            } while (true);

        }

        // Create a student from user input
        public static void CreateStudent()
        {
            Console.Clear();

            try
            {
                Console.WriteLine("Do you want to add a regular/college student (r/c)?");
                char type = Console.ReadKey().KeyChar;
                Console.WriteLine();

                if (type != 'r' && type != 'c')
                    throw new Exception("Invalid student type.");

                Console.Write("Enter name: ");
                var name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                    throw new Exception("Name cannot be empty.");

                Console.Write("Enter age: ");
                int age = int.Parse(Console.ReadLine() ?? "");
                if (age <= 0)
                    throw new Exception("Age cannot be less than 0.");

                if (type == 'r')
                {
                    // id is 0 because manager takes care of automatic id assignment
                    manager.AddStudent(new Student(0, name, age));
                }
                else
                {
                    Console.Write("Enter subject: ");
                    var subject = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(subject))
                        throw new Exception("Subject cannot be empty.");

                    Console.Write("Enter grade average: ");
                    int gradeAverage = int.Parse(Console.ReadLine() ?? "");
                    if (gradeAverage <= 0)
                        throw new Exception("Grade average cannot be less than 0.");

                    manager.AddStudent(new CollegeStudent(0, name, age, subject, gradeAverage));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid data.");
                if (!string.IsNullOrEmpty(ex.Message))
                    Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press Enter to continue.");
            Console.ReadKey();

        }

        // Display 1 or all students from user input
        public static void DisplayStudents()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("What would you want to display?");
                Console.WriteLine("1) All students");
                Console.WriteLine("2) A student by ID");
                Console.WriteLine("3) A student by name");

                int op = Console.ReadKey().KeyChar - '0';
                Console.WriteLine();

                if (op < 1 || op > 3)
                    throw new Exception("Invalid operation code.");

                switch (op)
                {
                    case 1:
                        if (manager.DisplayStudents() == 0)
                            Console.WriteLine("No students in database. Add students!");
                        break;
                    case 2:
                        int id = int.Parse(Console.ReadLine() ?? "");
                        if (!manager.DisplayStudent(id))
                            Console.WriteLine($"Student with ID {id} doesn't exist.");
                        break;
                    case 3:
                        string name = Console.ReadLine() ?? "";
                        if (string.IsNullOrWhiteSpace(name))
                            throw new Exception("Student name is invalid.");
                        if (!manager.DisplayStudent(name))
                            Console.WriteLine($"Student with name {name} doesn't exist.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid data.");
                if (!string.IsNullOrEmpty(ex.Message))
                    Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press Enter to continue.");
            Console.ReadKey();
        }

        // Update 1 student by ID from user input
        public static void UpdateStudent()
        {
            try
            {
                Console.Clear();
                Console.Write("Enter ID of student to update: ");
                int id = int.Parse(Console.ReadLine() ?? "");
                if (!manager.DoesStudentExist(id))
                    throw new Exception($"Student with ID {id} doesn't exist.");

                Dictionary<string, string> fieldsToChange = [];
                var fields = manager.GetStudent(id)!.GetFields();
                foreach (var field in fields)
                {
                    Console.Write($"Enter value for {field} (leave empty to not change): ");
                    string value = Console.ReadLine() ?? "";
                    if (string.IsNullOrWhiteSpace(value))
                        continue;
                    fieldsToChange[field] = value;
                }
                if (!manager.UpdateStudent(id, fieldsToChange))
                    Console.WriteLine("Update failed. One or more values were invalid.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid data.");
                if (!string.IsNullOrEmpty(ex.Message))
                    Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press Enter to continue.");
            Console.ReadKey();
        }

        // Delete 1 stduent by ID from user input
        public static void DeleteStudent()
        {
            try
            {
                Console.Clear();
                Console.Write("Enter student ID to delete: ");
                int id = int.Parse(Console.ReadLine() ?? "");
                if (!manager.DeleteStudent(id))
                {
                    Console.WriteLine($"Student with ID {id} doesn't exist.");
                }
                else
                {
                    Console.WriteLine("Student deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid data.");
                if (!string.IsNullOrEmpty(ex.Message))
                    Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press Enter to continue.");
            Console.ReadKey();
        }

        public static void Main()
        {
            while (true)
            {
                int op = ShowMenu();

                switch (op)
                {
                    case 1:
                        CreateStudent();
                        break;
                    case 2:
                        DisplayStudents();
                        break;
                    case 3:
                        UpdateStudent();
                        break;
                    case 4:
                        DeleteStudent();
                        break;
                    default:
                        return;
                }
            }
        }
    }
}