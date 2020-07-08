namespace OctoAwesome.Basics.Entities
{
    public class Dog : Entity
    {
        public Dog(Coordinate coordinate)
        {
            Position = coordinate;
            Radius = 0.5f;
            Height = 1f;
            Mass = 100;
        }
    }
}   
