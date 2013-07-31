namespace NDragDrop.TestApplication
{
    public abstract class Fruit
    {
        public string Name { get; set; }
    }

    public class Apple : Fruit
    {
        public Apple()
        {
            Name = "Apple";
        }
    }

    public class Banana : Fruit
    {
        public Banana()
        {
            Name = "Banana";
        }
    }
}
