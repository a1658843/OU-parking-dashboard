namespace OuStudentPortal.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Major { get; set; } = string.Empty;
    public double Gpa { get; set; }
}
