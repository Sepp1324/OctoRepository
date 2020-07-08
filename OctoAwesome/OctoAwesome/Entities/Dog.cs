namespace OctoAwesome.Basics.Entities
{
    public class Dog : Entity
    {
        public Dog(Coordinate coordinate)
        {
            Position = coordinate;
            Radius = 3;
            Height = 4;
            Mass = 100;
        }
    }
}   
